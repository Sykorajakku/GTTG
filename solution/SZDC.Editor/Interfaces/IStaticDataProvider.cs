using System.Collections.Generic;

using GTTG.Model.Model.Events;
using SZDC.Model.Infrastructure;
using SZDC.Model.Infrastructure.Trains;

namespace SZDC.Editor.Interfaces {

    /// <summary>
    /// Contract for provider of data which are readonly and represents default tine-independent timetable organization. 
    /// </summary>
    public interface IStaticDataProvider {

        /// <summary>
        /// All available railways denoted by railway number.
        /// </summary>
        IEnumerable<string> LoadRailwayNumbers();

        /// <summary>
        /// Loads stations and their kilometers distance in the segment.
        /// <paramref name="railwaySegmentId"/> is value from <see cref="RailwaySegmentBriefDescription"/>.
        /// </summary>
        RailwaySegmentDetailedDescription LoadDetailedSegmentDescription(long railwaySegmentId);

        /// <summary>
        /// Loads all segments of selected railway.
        /// </summary>
        /// <param name="railwayNumber">Value from <see cref="LoadRailwayNumbers"/></param>
        IEnumerable<RailwaySegmentBriefDescription> LoadRailwaySegments(string railwayNumber);

        /// <summary>
        /// Loads trains with schedules subset in selected segment.
        /// </summary>
        IEnumerable<StaticTrainDescription> LoadTrainsInRailwaySegment(RailwaySegmentDetailedDescription detailedDescription);
    }

    public struct StaticTrainMovementEvent {

        public int Hours { get; set; }
        public int Minutes { get; set; }
        public bool HasMoreThan30Seconds { get; set; }
        public StationDescription Station { get; set; }
        public TrainEventType TrainMovementEventType { get; set; }
    }

    public struct StaticTrainDescription {
        public int TrainNumber { get; set; }
        public TrainType TrainType { get; set; }
        public List<StaticTrainMovementEvent> StaticSchedule { get; set; }
        public TrainDecorationType TrainDecorationType { get; set; }
    }

    public struct StationSegmentDescription {
        public StationDescription StationDescription { get; set; }
        public double KilometersInSegment { get; set; }
        public string DrawnKilometersColumnValue { get; set; }
    }

    public struct StationDescription {
        public string Name { get; set; }
        public List<TrackDescription> Tracks { get; set; }
    }

    public struct TrackDescription {
        public TrackType TrackType { get; set; }
        public string TrackName { get; set; }
    }

    public class RailwaySegmentDetailedDescription {

        public long Id { get; set; }
        public List<StationSegmentDescription> StationsInSegment { get; set; }

        public bool IntersectsWith(List<StaticTrainMovementEvent> valueStaticSchedule) {

            // at-least two same station names --> return true
            var intersections = 0;

            foreach (var t in valueStaticSchedule) {
                foreach (var t1 in StationsInSegment) {
                    if (t1.StationDescription.Name == t.Station.Name) {
                        intersections++;
                        break;
                    }
                }

                if (intersections > 1) {
                    return true;
                }
            }
            return false;
        }
    }

    public class RailwaySegmentBriefDescription {

        public long Id { get; set; }
        public List<string> StationsInSegment { get; set; }
    }
}
