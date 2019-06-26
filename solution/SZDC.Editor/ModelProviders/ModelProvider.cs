using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using GTTG.Core.Time;
using GTTG.Model.Model.Events;
using GTTG.Model.Model.Infrastructure;
using SZDC.Editor.Interfaces;
using SZDC.Model.Events;
using SZDC.Model.Infrastructure;
using SZDC.Model.Infrastructure.StationContent;

namespace SZDC.Editor.ModelProviders {

    public class ModelProvider {

        /// <summary>
        /// Keeps instances of stations in application.
        /// </summary>
        private readonly IDictionary<string, SzdcStation> _stations;
        private readonly Random _random = new Random();

        public ModelProvider() {
            _stations = new Dictionary<string, SzdcStation>();
        }

        /// <summary>
        /// Converts segment to railway. Uses same instance of station if already exists, otherwise creates new one.
        /// </summary>
        public SzdcRailway ConvertToRailway(string railwayNumber, List<StationSegmentDescription> stationDescriptions) {

            var stationInfo = new Dictionary<Station, SzdcStationInfo>();
            var stations = new List<SzdcStation>();
            foreach (var stationDescription in stationDescriptions) {

                if (!_stations.TryGetValue(stationDescription.StationDescription.Name, out var station)) {
                    station = CreateStation(stationDescription.StationDescription);
                    _stations.Add(stationDescription.StationDescription.Name, station);
                }
                
                stations.Add(station);
                stationInfo.Add(station, new SzdcStationInfo {
                    RailwayDistance = stationDescription.KilometersInSegment,
                    RailwayDistanceStringValue = stationDescription.DrawnKilometersColumnValue });
            }

            return new SzdcRailway(railwayNumber, stations, stationInfo);
        }

        private static SzdcStation CreateStation(StationDescription stationDescription) {
            return new SzdcStation(stationDescription.Tracks.Select(t => new SzdcTrack(t.TrackType, t.TrackName, stationDescription.Name)), stationDescription.Name);
        }

        /// <summary>
        /// Creates new particular schedule with specific start date from time-independent static schedule.
        /// </summary>
        public ImmutableArray<TrainEvent> ConvertToSchedule(List<StaticTrainMovementEvent> staticSchedule, DateTime start) {

            var trainEvents = new List<TrainEvent>();
            var time = start.Date;
            var lastStationName = string.Empty;
            var trackIndex = 0;

            int index;
            for (index = 0; index < staticSchedule.Count; ++index) {

                var currentEvent = staticSchedule[index];
                var stationName = currentEvent.Station.Name;

                // over midnight --> add day
                if (currentEvent.Hours < time.Hour && currentEvent.Minutes < time.Minute) {
                    time = time.AddDays(1).AddHours(currentEvent.Hours).AddMinutes(currentEvent.Minutes);
                    time = currentEvent.HasMoreThan30Seconds ? time.AddSeconds(30) : time.AddSeconds(0);
                }
                else {
                    var newTime = time.Date.AddHours(currentEvent.Hours).AddMinutes(currentEvent.Minutes);
                    newTime = currentEvent.HasMoreThan30Seconds ? newTime.AddSeconds(30) : newTime.AddSeconds(0);

                    if (time > newTime) {
                        throw new ModelDefinitionException($"Previous event with datetime {time} is before or equal than next event with datetime {newTime}.");
                    }
                    time = newTime;
                }

                // get access to shared instance of station 
                if (!_stations.TryGetValue(stationName, out var station)) {
                    station = CreateStation(currentEvent.Station);
                    _stations.Add(stationName, station);
                }

                // if newly accessed station, select track -- otherwise use the same track for arrival / departure
                if (lastStationName != stationName) {
                    lastStationName = stationName;
                    trackIndex = _random.Next(0, station.Tracks.Length - 1);
                }

                var trainMovementEvent = new SzdcTrainEvent(time, station, station.Tracks[trackIndex], currentEvent.TrainMovementEventType);
                trainEvents.Add(trainMovementEvent);
            }
            return trainEvents.ToImmutableArray();
        }

        /// <summary>
        /// Gets <see cref="DateTimeInterval"/> scope of schedule if it would start in date of <paramref name="date"/>.
        /// </summary>
        public DateTimeInterval ConvertToDateTimeInterval(List<StaticTrainMovementEvent> valueStaticSchedule, DateTime date) {

            var firstEvent = valueStaticSchedule[0];
            var lastEvent = valueStaticSchedule.Last();

            var first = date.Date;
            var last = (firstEvent.Hours > lastEvent.Hours) ? date.Date.AddDays(1) : date.Date;
            var firstDateTime = first.AddHours(firstEvent.Hours).AddMinutes(firstEvent.Minutes);
            var lastDateTime = last.AddHours(lastEvent.Hours).AddMinutes(lastEvent.Minutes);
            return new DateTimeInterval(firstDateTime, lastDateTime);
        }
    }
}
