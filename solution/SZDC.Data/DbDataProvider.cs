using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using GTTG.Model.Model.Events;
using SZDC.Data.Model;
using SZDC.Editor.Interfaces;
using SZDC.Editor.ModelProviders;
using SZDC.Model.Infrastructure.Trains;

namespace SZDC.Data {

    public class DbDataProvider : IStaticDataProvider {
        
        public IEnumerable<string> LoadRailwayNumbers() {

            using (var dbContext = new SzdcContext()) {

                foreach (var railway in dbContext.Railways) {
                    yield return railway.RailwayNumber;
                }
            }
        }
       
        public RailwaySegmentDetailedDescription LoadDetailedSegmentDescription(long railwaySegmentId) {

            using (var dbContext = new SzdcContext()) {

                var railwaySegment =
                    dbContext.Railways
                        .SelectMany(r => r.RailwaySections)
                        .Where(s => s.Id == railwaySegmentId)
                        .Include(r => r.Stations)
                        .ThenInclude(s => s.Station)
                        .ThenInclude(t => t.Tracks)
                        .ThenInclude(t => t.Track)
                        .FirstOrDefault();

                if (railwaySegment == null) {
                    return null;
                }

                return new RailwaySegmentDetailedDescription {
                    Id = railwaySegment.Id,
                    StationsInSegment = railwaySegment.Stations.OrderBy(s => s.StationOrder).Select(ToDetailedDescription).ToList()
                };
            }
        }

        private static StationSegmentDescription ToDetailedDescription(RailwaySectionStation station) {

            return new StationSegmentDescription {
                KilometersInSegment = station.KilometersInSegment,
                DrawnKilometersColumnValue = station.KilometersString,
                StationDescription = new StationDescription {
                    Name = station.Station.Name,
                    Tracks = station.Station.Tracks.Select(t => new TrackDescription {
                        TrackName = t.Track.Number, TrackType = t.Track.TrackType.Value
                    }).ToList()
                }
            };
        }

        public IEnumerable<RailwaySegmentBriefDescription> LoadRailwaySegments(string railwayNumber) {

            using (var dbContext = new SzdcContext()) {

                var segmentsInRailway = dbContext.Railways
                    .Where(r => r.RailwayNumber == railwayNumber)
                    .Include(s => s.RailwaySections)
                    .ThenInclude(s => s.Stations)
                    .ThenInclude(s => s.Station)
                    .FirstOrDefault();

                if (segmentsInRailway == null) {
                    yield break;
                }

                foreach (var railwaySegment in segmentsInRailway.RailwaySections) {
                    yield return ToRailwaySegmentDescription(railwaySegment);
                }
            }
        }

        public IEnumerable<StaticTrainDescription> LoadTrainsInRailwaySegment(RailwaySegmentDetailedDescription detailedDescription) {

            var stations = detailedDescription.StationsInSegment.Select(s => s.StationDescription.Name).ToList();

            using (var dbContext = new SzdcContext()) {

                return dbContext.Trains
                    .Where(s => s.Schedule.Count(r => stations.Contains(r.StationStop.Station.Name)) >= 2)
                    .Include(t => t.Schedule)
                    .ThenInclude(schedule => schedule.StationStop)
                    .ThenInclude(s => s.Station)
                    .ThenInclude(r => r.Tracks)
                    .ThenInclude(r => r.Track)
                    .Include(t => t.Schedule)
                    .ThenInclude(schedule => schedule.StationStop)
                    .ThenInclude(s => s.Arrival)
                    .Include(t => t.Schedule)
                    .ThenInclude(schedule => schedule.StationStop)
                    .ThenInclude(s => s.Departure)
                    .AsEnumerable().Select(ToStaticTrain).ToList();
            }
        }

        private static StaticTrainDescription ToStaticTrain(Train train) {

            var orderedSchedule = train.Schedule.OrderBy(s => s.Order);
            TrainEventType? lastMovementEventType = null;

            var staticSchedule = new List<StaticTrainMovementEvent>();
            foreach (var stationStop in orderedSchedule.Select(s => s.StationStop)) {

                if (stationStop.Arrival != null) {

                    if (lastMovementEventType == null) {

                        var t = stationStop.Arrival.ToStaticTrainMovementEvent(stationStop.Station);
                        t.TrainMovementEventType = TrainEventType.Arrival;
                        staticSchedule.Add(t);
                        lastMovementEventType = TrainEventType.Arrival;
                    }
                    else if (lastMovementEventType == TrainEventType.Arrival) {
                        throw new ModelDefinitionException("Has same both arrivals.");
                    }
                    else {
                        var st = stationStop.Arrival.ToStaticTrainMovementEvent(stationStop.Station);
                        st.TrainMovementEventType = TrainEventType.Arrival;
                        staticSchedule.Add(st);
                        lastMovementEventType = TrainEventType.Arrival;
                    }
                }

                if (stationStop.Departure != null) {

                    if (lastMovementEventType == null) {

                        var t = stationStop.Departure.ToStaticTrainMovementEvent(stationStop.Station);
                        t.TrainMovementEventType = TrainEventType.Departure;
                        staticSchedule.Add(t);
                        lastMovementEventType = TrainEventType.Passage;
                    }
                    else {

                        var movementType =
                            lastMovementEventType == TrainEventType.Arrival
                                ? TrainEventType.Departure
                                : TrainEventType.Passage;

                        var st = stationStop.Departure.ToStaticTrainMovementEvent(stationStop.Station);
                        st.TrainMovementEventType = movementType;
                        staticSchedule.Add(st);
                        lastMovementEventType = movementType;
                    }
                }
            }

            return new StaticTrainDescription {
                TrainNumber = train.TrainNumber,
                TrainType = train.TrainType,
                TrainDecorationType = TrainDecorationType.FollowsValidDirection,
                StaticSchedule = staticSchedule
            };
        }

        private static RailwaySegmentBriefDescription ToRailwaySegmentDescription(RailwaySection railwaySection) {

            return new RailwaySegmentBriefDescription {
                Id = railwaySection.Id,
                StationsInSegment = railwaySection.Stations.OrderBy(s => s.StationOrder).Select(s => s.Station.Name).ToList()
            };
        }
    }
}
