using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Autofac;

using GTTG.Core.Time;
using GTTG.Model.Model.Events;
using SZDC.Editor.Interfaces;
using SZDC.Editor.Locator;
using SZDC.Editor.ModelProviders;
using SZDC.Editor.Services;
using SZDC.Editor.TrainTimetables;
using SZDC.Model.Infrastructure;
using SZDC.Model.Infrastructure.Traffic;
using SZDC.Model.Infrastructure.Trains;

namespace SZDC.Editor {

    public partial class ApplicationEditor {
       
        public IServiceProvider OpenStaticTrainTimetable() {

            // Create new scope of locator for this window (factories, timetable instance)
            var locator = _serviceProvider.GetService<StaticTimetableServiceLocator>().GetScopedServiceLocator();
            
            // get new timetable instance
            var trainTimetable = locator.Resolve<StaticTrainTimetable>();

            // get time interval selected by user in UI
            var dateTimeContext = TimeSelector.CurrentSelector.ToDateTimeContext();
            trainTimetable.UpdateDateTimeContext(dateTimeContext);

            // get railway selected by user in UI
            var railwayNumber = InfrastructureSelector.SelectedRailway;
            var railwaySegmentDescription = InfrastructureSelector.SelectedRailwaySection;

            // load DB data
            var detailedSegmentDescription = 
                _serviceProvider.GetService<IStaticDataProvider>()
                .LoadDetailedSegmentDescription(railwaySegmentDescription.Id);
            var trainsInRailwaySegment =
                _serviceProvider.GetService<IStaticDataProvider>()
                .LoadTrainsInRailwaySegment(detailedSegmentDescription)
                .ToList();

            // convert data to model
            var railway = ModelProvider.ConvertToRailway(railwayNumber, detailedSegmentDescription.StationsInSegment);
            var (traffic, errors) = CreateStaticTraffic(railway, trainsInRailwaySegment, dateTimeContext.ContentDateTimeInterval);

            // create with model from model
            trainTimetable.ChangeRailwayView(railway);
            trainTimetable.ChangeTrafficView(traffic);

            // report conversion errors to UI
            trainTimetable.Errors = errors.ToImmutableArray();

            trainTimetable.TimetableInfo = new TimetableInfo {

                RailwayNumber = railwayNumber,
                FirstStationName = detailedSegmentDescription.StationsInSegment.First().StationDescription.Name,
                LastStationName = detailedSegmentDescription.StationsInSegment.Last().StationDescription.Name,
                TimetableType = TrainTimetableType.Static
            };

            // wrap this ILifetimeScope to IServiceProvider instance (independent on DI framework)
            return new AutofacServiceProvider(locator);
        }
        
        public IServiceProvider OpenDynamicTrainTimetable() {

            var locator = _serviceProvider.GetService<DynamicTimetableServiceLocator>().GetScopedServiceLocator();
            var trainTimetable = locator.Resolve<DynamicTrainTimetable>();

            var railwayNumber = InfrastructureSelector.SelectedRailway;
            var railwaySegmentDescription = InfrastructureSelector.SelectedRailwaySection;

            var detailedSegmentDescription =
                _serviceProvider.GetService<IStaticDataProvider>()
                    .LoadDetailedSegmentDescription(railwaySegmentDescription.Id);

            var railway = ModelProvider.ConvertToRailway(railwayNumber, detailedSegmentDescription.StationsInSegment);
            trainTimetable.ChangeRailwayView(railway);

            trainTimetable.TimetableInfo = new TimetableInfo {
                RailwaySegmentDetailedDescription = detailedSegmentDescription,
                RailwaySegmentBriefDescription = railwaySegmentDescription,
                RailwayNumber = railwayNumber,
                FirstStationName = detailedSegmentDescription.StationsInSegment.First().StationDescription.Name,
                LastStationName = detailedSegmentDescription.StationsInSegment.Last().StationDescription.Name,
                TimetableType = TrainTimetableType.Realtime
            };

            return new AutofacServiceProvider(locator);
        }

        private (SzdcTraffic Traffic, List<string> Errors) CreateStaticTraffic(SzdcRailway railway, IEnumerable<StaticTrainDescription> trainsInRailwaySegment, DateTimeInterval contentDateTimeInterval) {

            var trains = new List<SzdcTrain>();
            var errors = new List<string>();

            foreach (var staticTrain in trainsInRailwaySegment) {
                try {

                    if (!contentDateTimeInterval.IntersectsWith(ModelProvider.ConvertToDateTimeInterval(staticTrain.StaticSchedule, contentDateTimeInterval.Start))) {
                        continue;
                    }
                    var schedule = ModelProvider.ConvertToSchedule(staticTrain.StaticSchedule, contentDateTimeInterval.Start.Date);
                    var (start, end) = SanitizeSchedule(railway, schedule, staticTrain.TrainNumber);
                    var scheduleSubset = CreateScheduleSubset(railway, start, end, schedule);
                    trains.Add(new SzdcTrain(staticTrain.TrainNumber, staticTrain.TrainType, staticTrain.TrainDecorationType, start != 0, end != schedule.Length - 1, scheduleSubset, schedule));
                }
                catch (ModelDefinitionException ex) {
                    errors.Add($"Unable to add train {staticTrain.TrainNumber}: {ex.Message}");
                }
            }
            return (new SzdcTraffic(trains), errors);
        }

        public IEnumerable<string> GetRailways() {

            var dataProvider = _serviceProvider.GetService<IStaticDataProvider>();
            return dataProvider.LoadRailwayNumbers();
        }

        public IEnumerable<RailwaySegmentBriefDescription> GetRailwaySegments(string railwayNumber) {

            var dataProvider = _serviceProvider.GetService<IStaticDataProvider>();
            return dataProvider.LoadRailwaySegments(railwayNumber);
        }

        public static IList<TrainEvent> CreateScheduleSubset(SzdcRailway railway, int start, int end, ImmutableArray<TrainEvent> events) {

            var scheduleSubset = new List<TrainEvent>();
            for (var i = start; i <= end; i++) {

                scheduleSubset.Add(events[i]);
                if (!railway.Stations.Contains(events[i].Station)) {
                    throw new ModelDefinitionException($"Station is not present in railway {railway.RailwayNumber}.");
                }
            }
            return scheduleSubset;
        }

        public static (int start, int end) SanitizeSchedule(SzdcRailway railway, ImmutableArray<TrainEvent> events, int trainNumber) {

            var railwayStations = railway.Stations.Select(s => (s as SzdcStation)?.StationName).ToList();
            var firstSimilarStation = events.FirstOrDefault(s => railwayStations.Contains((s.Station as SzdcStation)?.StationName));
            var lastSimilarStation = events.LastOrDefault(s => railwayStations.Contains((s.Station as SzdcStation)?.StationName));

            if (firstSimilarStation == null || lastSimilarStation == null) {
                throw new ModelDefinitionException($"Train {trainNumber} has no stations from railway segment of {railway.RailwayNumber} in schedule.");
            }

            if (firstSimilarStation == lastSimilarStation) {
                throw new ModelDefinitionException($"Train {trainNumber} has no stations from railway segment of {railway.RailwayNumber} in schedule.");
            }

            var firstIndex = events.IndexOf(firstSimilarStation);
            var lastIndex = events.IndexOf(lastSimilarStation);

            return (Math.Min(firstIndex, lastIndex), Math.Max(firstIndex, lastIndex));
        }
    }
}
