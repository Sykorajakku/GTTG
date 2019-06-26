using System.Collections.Generic;
using Newtonsoft.Json.Linq;

using SZDC.JsonParser.Parser.Exceptions;
using SZDC.JsonParser.Parser.Model;

namespace SZDC.JsonParser.Parser {

    public class RailwayParser {

        public static readonly string RailwaysKey = "railways";
        public static readonly string RailwaySegmentsKey = "railwaySegments";
        public static readonly string KilometersValue = "kilometersValue";
        public static readonly string KilometersString = "kilometersString";
        public static readonly string StationName = "stationName";

        private readonly IParsedRailwayReceiver _parsedRailwayReceiver;

        public RailwayParser(IParsedRailwayReceiver parsedRailwayReceiver) {
            _parsedRailwayReceiver = parsedRailwayReceiver;
        }

        public void ParseJsonRoot(JObject jsonRoot) {

            foreach (var jsonToken in jsonRoot[RailwaysKey]) {

                if (!(jsonToken is JProperty railwayJsonProperty)) {
                    throw new ImportTypeException(jsonToken, "object",
                        "railway name property with it's segment number properties");
                }

                if (!(railwayJsonProperty.Value is JObject railwayJsonObject)) {
                    throw new ImportTypeException(jsonToken, "object",
                        "railway name property with it's segment number properties");
                }

                var railwayName = railwayJsonProperty.Name;

                _parsedRailwayReceiver.AddRailway(new ParsedRailway {
                    RailwayName = railwayName,
                    RailwaySegments = ParseRailwaySegment(railwayJsonObject)
                });
            }
        }

        public List<ParsedRailwaySegment> ParseRailwaySegment(JObject railwayJsonObject) {

            var parsedRailway = new List<ParsedRailwaySegment>();
            var segmentsArrayToken = railwayJsonObject[RailwaySegmentsKey];

            if (!(segmentsArrayToken is JArray segmentsJsonArray)) {
                throw new ImportTypeException(segmentsArrayToken, "array", "segments");
            }
            
            foreach (var segment in segmentsJsonArray) {

                var parsedSegment = new ParsedRailwaySegment {
                    ParsedStations = new List<ParsedStation>()
                };

                if (!(segment is JArray stationsArray)) {
                    throw new ImportTypeException(segment, "array", "stations");
                }

                foreach (var station in stationsArray) {

                    if (!(station is JObject stationObject)) {
                        throw new ImportTypeException(station, "object", "station");
                    }

                    if (!(stationObject.TryGetValue(KilometersValue, out var value) &&
                        value is JValue kilometersValue && (kilometersValue.Type == JTokenType.Float || kilometersValue.Type == JTokenType.Integer))) {
                        throw new ImportException($"Expected integer or float property {value} in {station}");
                    }
                    var kilometersDoubleValue = value.ToObject<float>();

                    if (!(stationObject.TryGetValue(KilometersString, out var kilometersString) &&
                          kilometersString is JValue kilometersStringValue && 
                          kilometersStringValue.Type == JTokenType.String)) {
                         throw new ImportException($"Expected string property {KilometersString} in {station}");
                    }
                    if (!(stationObject.TryGetValue(StationName, out var stationNameObject) &&
                          stationNameObject is JValue stationNameObjectValue &&
                          stationNameObjectValue.Type == JTokenType.String)) {
                        throw new ImportException($"Expected string property {StationName} in {station}");
                    }

                    var parsedStation = new ParsedStation {
                        KilometersString = (string) kilometersStringValue.Value,
                        KilometersValue = kilometersDoubleValue,
                        Name = (string) stationNameObjectValue
                    };

                    parsedSegment.ParsedStations.Add(parsedStation);
                }

                parsedRailway.Add(parsedSegment);
            }
            return parsedRailway;
        }
    }
}
