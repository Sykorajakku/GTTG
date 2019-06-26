using System.Collections.Generic;

namespace SZDC.JsonParser.Parser.Model {

    public struct ParsedRailway {
        public string RailwayName { get; set; }
        public List<ParsedRailwaySegment> RailwaySegments { get; set; }
    }
}
