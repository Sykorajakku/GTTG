using System.Collections.Generic;

using SZDC.JsonParser.Parser;
using SZDC.JsonParser.Parser.Model;

namespace SZDC.Data {

    public class ParsedRailwayReceiver : IParsedRailwayReceiver {

        public Dictionary<string, ParsedRailway> Railways { get; set; }

        public ParsedRailwayReceiver() {
            Railways = new Dictionary<string, ParsedRailway>();
        }

        public void AddRailway(ParsedRailway parsedRailway) {

            if (!Railways.TryGetValue(parsedRailway.RailwayName, out var railway)) {
                Railways.Add(parsedRailway.RailwayName, parsedRailway);
            }
            else {
                foreach (var segment in parsedRailway.RailwaySegments) {
                    railway.RailwaySegments.Add(segment);
                }
            }
        }
    }
}
