using SZDC.Editor.Interfaces;

namespace SZDC.Editor.TrainTimetables {

    /// <summary>
    /// Information about timetable.
    /// </summary>
    public class TimetableInfo {

        public RailwaySegmentDetailedDescription RailwaySegmentDetailedDescription { get; set; }
        public RailwaySegmentBriefDescription RailwaySegmentBriefDescription { get; set; }
        public string RailwayNumber { get; set; }
        public string FirstStationName { get; set; }
        public string LastStationName { get; set; }
        public TrainTimetableType TimetableType { get; set; }
    }
}
