using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

using SZDC.JsonParser.Parser.Exceptions;
using SZDC.JsonParser.Parser.Model;
using SZDC.Model.Infrastructure.Trains;

namespace SZDC.JsonParser.Parser {

    public class TrainParser {

        public static readonly string TrainsKey = "trains";
        public static readonly string ScheduleKey = "schedule";
        public static readonly string TrainTypeKey = "trainType";
        public static readonly string DepartureTimeKey = "departureTime";
        public static readonly string ArrivalTimeKey = "arrivalTime";
        public static readonly string StationNameKey = "stationName";
        public static readonly string IsAfter30SecondsKey = "isAfter30Seconds";
        public static readonly string HoursKey = "hours";
        public static readonly string MinutesKey = "minutes";

        private readonly IParsedTrainsReceiver _parsedTrainsReceiver;

        public TrainParser(IParsedTrainsReceiver parsedTrainsReceiver) {
            _parsedTrainsReceiver = parsedTrainsReceiver;
        }

        public void ParseJsonRoot(JObject jsonRoot) {

            foreach (var jsonToken in jsonRoot[TrainsKey]) {

                if (!(jsonToken is JProperty trainJsonProperty) ||
                    !Int32.TryParse(trainJsonProperty.Name, out var trainNumber)) {
                    throw new ImportTypeException(jsonToken, "property", "train number value");
                }

                var trainTypeProperty = trainJsonProperty.Children()[TrainTypeKey].First();

                if (!(trainTypeProperty is JValue trainTypeValue) || !(trainTypeValue.Value is string stringValue)) {
                    throw new ImportValueException(jsonToken, nameof(String));
                }

                if (!Enum.TryParse<TrainType>(stringValue, ignoreCase: false, result: out var trainType)) {
                    throw new ImportValueException(jsonToken, nameof(TrainType));
                }

                var scheduleArray = trainJsonProperty.Children()[ScheduleKey].First();
                var schedule = ParseTrainSchedule(scheduleArray);

                var staticTrain = new ParsedStaticTrain {
                    Schedule = schedule,
                    TrainNumber = trainNumber,
                    TrainType = trainType,
                };

                _parsedTrainsReceiver.AddTrain(staticTrain);
            }
        }

        private List<ParsedStationStop> ParseTrainSchedule(JToken scheduleRoot) {

            if (!(scheduleRoot is JArray scheduleArray)) {
                throw new ImportTypeException(scheduleRoot, "array", "station stops");
            }

            return scheduleArray.Select(ParseStationStop).ToList();
        }

        private ParsedStationStop ParseStationStop(JToken jsonToken) {

            if (!(jsonToken is JObject scheduleJsonObject)) {
                throw new ImportTypeException(jsonToken, "object", "station stop");
            }

            var departureTimeJson = scheduleJsonObject[DepartureTimeKey];
            var departureEvent = ParseStaticTimeEvent(departureTimeJson);

            var arrivalTimeJson = scheduleJsonObject[ArrivalTimeKey];
            var arrivalEvent = ParseStaticTimeEvent(arrivalTimeJson);

            var stationNameJson = scheduleJsonObject[StationNameKey];
            if (!(stationNameJson is JValue stationNameJsonObject) ||
                !(stationNameJsonObject.Value is string stationName)) {
                throw new ImportValueException(stationNameJson, "string");
            }

            if (!char.IsLetter(stationName[0])) {
                stationName = stationName.Substring(1);
            }

            var index = stationName.IndexOf(':');
            if (index != -1) {
                stationName = stationName.Substring(0, index);
            }

            var c = stationName.Contains("TR");
            if (c) {
                stationName = stationName.Substring(0, index);
            }

            if (stationName[stationName.Length - 1] == '.') {
                stationName = stationName.Substring(0, stationName.Length - 1);
            }

            return new ParsedStationStop {
                DepartureTimeEvent = departureEvent,
                ArrivalTimeEvent = arrivalEvent,
                StationName = stationName
            };
        }

        private static ParsedTrainEvent ParseStaticTimeEvent(JToken jsonToken) {

            if (jsonToken.Type == JTokenType.Null) {
                return null;
            }

            if (!(jsonToken is JObject timeEventJsonObject)) {
                throw new ImportTypeException(jsonToken, "object", "time event");
            }

            var hoursJsonField = timeEventJsonObject[HoursKey];
            if (hoursJsonField.Type != JTokenType.Integer) {
                throw new ImportValueException(hoursJsonField, "int");
            }

            var hoursValue = ((JValue)hoursJsonField).ToObject<int>();

            var minutesJsonField = timeEventJsonObject[MinutesKey];
            if (minutesJsonField.Type != JTokenType.Integer) {
                throw new ImportValueException(minutesJsonField, "int");
            }

            var minutesValue = ((JValue)minutesJsonField).ToObject<int>();

            var isAfter30SecondsJsonValue =
                timeEventJsonObject[IsAfter30SecondsKey] ?? timeEventJsonObject["isAfter30seconds"];
            if (isAfter30SecondsJsonValue.Type != JTokenType.Boolean) {
                throw new ImportValueException(isAfter30SecondsJsonValue, "bool");
            }

            var isAfter30Seconds = ((JValue)isAfter30SecondsJsonValue).ToObject<bool>();

            return new ParsedTrainEvent {
                HasMoreThan30Seconds = isAfter30Seconds,
                Hours = hoursValue,
                Minutes = minutesValue
            };
        }
    }
}
