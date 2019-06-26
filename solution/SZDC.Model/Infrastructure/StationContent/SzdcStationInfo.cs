namespace SZDC.Model.Infrastructure.StationContent {

    /// <summary>Information about a <see cref="SzdcStation"/> in context of railway.</summary>
    public struct SzdcStationInfo {
        
        /// <summary>Distance of the station in kilometers in context of railway.</summary>
        public double RailwayDistance { get; set; }

        /// <summary>Textual representation equal to train timetable column (like 304.1=313).</summary>
        public string RailwayDistanceStringValue { get; set; }
    }
}
