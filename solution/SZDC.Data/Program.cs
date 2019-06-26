using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using SZDC.Data.Model;
using SZDC.JsonParser.Parser;
using SZDC.JsonParser.Parser.Exceptions;
using SZDC.JsonParser.Parser.Model;

namespace SZDC.Data {
  
    public class Program {

        public static void Main(string[] args) {

            if (args.Length != 1) {
                Console.WriteLine("Expecting only one argument as directory, exiting. Use --help if needed.");
                return;
            }

            if (args[0] == "--help") {
                Console.WriteLine("Provide directory where JSON files resides, import tool tries to parse every JSON file.");
                return;
            }

            var files = Directory.GetFiles(args[0], "*.json");

            var trackGenerator = new TrackGenerator();
            var trainReceiver = new ParsedTrainReceiver();
            var trainParser = new TrainParser(trainReceiver);
            var railwayReceiver = new ParsedRailwayReceiver();
            var railwayParser = new RailwayParser(railwayReceiver);

            foreach (var file in files) {

                try {
                    using (var reader = File.OpenText(file)) {
                        using (var jsonReader = new JsonTextReader(reader)) {

                            var jsonObject = (JObject) JToken.ReadFrom(jsonReader);

                            if (jsonObject.ContainsKey("trains")) {
                                trainParser.ParseJsonRoot(jsonObject);
                            }

                            if (jsonObject.ContainsKey("railways")) {
                                railwayParser.ParseJsonRoot(jsonObject);
                            }
                        }
                    }
                }
                catch (ImportException ex) {
                    Console.WriteLine($"Unable to parse file {file}: ", ex.Message);
                }
            }

            using (var dbContext = new SzdcContext()) {

                foreach (var railway in railwayReceiver.Railways) {

                    var foundRailway = dbContext.Railways
                        .Where(r => r.RailwayNumber == railway.Key)
                        .Include(r => r.RailwaySections)
                        .FirstOrDefault();

                    if (foundRailway == null) {
                        foundRailway = new Railway { RailwayNumber = railway.Key, RailwaySections = new List<RailwaySection>() };
                        dbContext.Add(foundRailway);
                    }

                    var railwaySections = new List<RailwaySection>();
                    foreach (var segment in railway.Value.RailwaySegments) {

                        var order = 1;
                        var stations = new List<RailwaySectionStation>();
                        foreach (var station in segment.ParsedStations) {

                            var entity = dbContext.Find<Station>(station.Name) ?? new Station {
                                Name = station.Name,
                                Tracks = trackGenerator.GenerateTracks(station.Name)
                            };

                            stations.Add(new RailwaySectionStation {
                                KilometersInSegment = station.KilometersValue,
                                Station = entity,
                                StationOrder = order++,
                                KilometersString = station.KilometersString
                            });
                        }

                        railwaySections.Add(new RailwaySection { Stations = stations });
                    }

                    foundRailway.RailwaySections.AddRange(railwaySections);
                    dbContext.SaveChanges();
                }
            }

            
                    

            var finalSchedules = new Dictionary<int, List<ParsedStationStop>>();

            foreach (var trainSchedules in trainReceiver.TrainsWithSchedule) {
                
                try {
                    var schedule = MergeSchedules(trainSchedules.Value);
                    ValidateSchedule(schedule);
                    finalSchedules.Add(trainSchedules.Key, schedule);
                }
                catch (ImportException ex) {
                    Console.WriteLine($"Unable to process train {trainSchedules.Key}: {ex.Message}");
                }
            }

            using (var dbContext = new SzdcContext()) {

                var index = 0;
                foreach (var finalSchedule in finalSchedules)
                {

                    var trainNumber = finalSchedule.Key;
                    var trainType = trainReceiver.TrainTypes[trainNumber];
                    var schedule = finalSchedule.Value.Select(t => ToStationStopOrder(t, index++, dbContext, trackGenerator)).ToList();

                    index = 0;
                    var train = new Train
                    {
                        TrainNumber = trainNumber,
                        TrainType = trainType[0],
                        Schedule = schedule
                    };

                    dbContext.Add(train);
                }

                dbContext.SaveChanges();
            }
        }

        private static StationStopOrder ToStationStopOrder(ParsedStationStop stationStop, int order, DbContext dbContext, TrackGenerator trackGenerator) {

            Station station = null;
            station = (Station) dbContext.Find(typeof(Station), stationStop.StationName);

            station = station ?? new Station {
                Name = stationStop.StationName,
                Tracks = trackGenerator.GenerateTracks(stationStop.StationName)
            };

            var s = new StationStop {
                Station = station,
                Arrival = ConvertToTrainEvent(stationStop.ArrivalTimeEvent),
                Departure = ConvertToTrainEvent(stationStop.DepartureTimeEvent)
            };

            return new StationStopOrder { StationStop = s, Order = order };
        }

        private static void ValidateSchedule(IList<ParsedStationStop> schedule) {

            if (schedule.Count == 0) throw new ImportException("Schedule is empty");

            for (var i = 0; i < schedule.Count; ++i) {

                var current = schedule[i];
                if (current.ArrivalTimeEvent == null && current.DepartureTimeEvent == null) {
                    throw new ImportException($"Both arrival and departure events in event of {current} are non-present.");
                }

                if (i == 0) continue;

                var previous = schedule[i - 1];
                

                if (!previous.IsBefore(current) && !previous.IsOverMidnight(current)) {
                    throw new ImportException($"Schedule is not valid: {previous} is after {current}");
                }
            }
        }

        private static StaticTrainEvent ConvertToTrainEvent(ParsedTrainEvent trainEvent) {

            if (trainEvent == null) return null;

            return new StaticTrainEvent {
                HasMoreThan30Seconds = trainEvent.HasMoreThan30Seconds,
                Hours = trainEvent.Hours,
                Minutes = trainEvent.Minutes
            };
        }

        private static List<ParsedStationStop> MergeSchedules(List<List<ParsedStationStop>> trainSchedules) {

            while (trainSchedules.Count != 1) {

                var intersectionFound = false;

                for (var i = 0; i < trainSchedules.Count; ++i) {
                    // try for each schedule if exists intersection with other

                    if (intersectionFound) break; // to break from inner cycle if found
                    for (var j = 0; j < trainSchedules.Count; ++j) {

                        if (i == j) continue; // skip the same
                        if (trainSchedules[i].HasSameStations(trainSchedules[j])) {

                            // remove schedule [j] and join to schedule [i]
                            var removedSchedule = trainSchedules[j];
                            trainSchedules.RemoveAt(j);
                            trainSchedules[i] = trainSchedules[i].Merge(removedSchedule);

                            // break and restart search for lower number of schedules
                            intersectionFound = true;
                            break;
                        }
                    }
                }

                // all combinations tested and no intersection found
                if (!intersectionFound) {
                    throw new ImportException("Unable to merge schedules to one.");
                }
            }
            return trainSchedules[0];
        }
    }

    public static class ListHelper {
        
        public static bool HasSameStations(this List<ParsedStationStop> schedule, List<ParsedStationStop> otherSchedule) {
            return schedule
                    .Select(s => s.StationName)
                    .Any(stationName => otherSchedule.Exists(t => t.StationName == stationName));
        }

        public static List<ParsedStationStop> Merge(this List<ParsedStationStop> schedule, List<ParsedStationStop> otherSchedule) {

            var result = new List<ParsedStationStop>();

            for (var i = 0; i < schedule.Count; ++i) {

                var stationName = schedule[i].StationName;

                for (var j = 0; j < otherSchedule.Count; ++j) {

                    if (otherSchedule[j].StationName == stationName) {

                        // We have station with same name in both schedules
                        // Now merge both schedules

                        if (i == 0) { // schedule first index --> insert whats before index j in other schedule
                            for (var k = 0; k < j; k++) {
                                result.Add(otherSchedule[k]);
                            }
                            var mergedEvent = MergeEvents(otherSchedule[j], schedule[i]); // merge same indices and check for same values
                            result.Add(mergedEvent);
                            result.AddRange(MergeFromSameBase(otherSchedule.Skip(j + 1), schedule.Skip(1))); // continue with following
                        }
                        else if (j == 0) {
                            for (var k = 0; k < i; k++) {
                                result.Add(schedule[k]);
                            }
                            var mergedEvent = MergeEvents(otherSchedule[0], schedule[i]);
                            result.Add(mergedEvent);
                            result.AddRange(MergeFromSameBase(schedule.Skip(i + 1), otherSchedule.Skip(1)));
                        }
                        return result;
                    }
                }
            }
            throw new ImportException("Same stations of schedules not found.");
        }

        private static IEnumerable<ParsedStationStop> MergeFromSameBase(IEnumerable<ParsedStationStop> skip, IEnumerable<ParsedStationStop> parsedStationStops) {

            var first = skip.GetEnumerator();
            var last = parsedStationStops.GetEnumerator();

            var firstNext = first.MoveNext();
            var lastNext = last.MoveNext();

            while (firstNext && lastNext) {

                yield return first.Current;

                firstNext = first.MoveNext();
                lastNext = last.MoveNext();
            }

            if (!firstNext && !lastNext) {
                yield break;
            }

            var toEnumerate = firstNext ? first : last;
            while (toEnumerate.MoveNext()) {
                yield return toEnumerate.Current;
            }
        }

        private static ParsedStationStop MergeEvents(ParsedStationStop parsedStationStop, ParsedStationStop parsedStationStop1) {

            ParsedTrainEvent arrival;
            ParsedTrainEvent departure;

            if (parsedStationStop.ArrivalTimeEvent != null && parsedStationStop1.ArrivalTimeEvent != null) {

                if (!parsedStationStop.ArrivalTimeEvent.Equals(parsedStationStop1.ArrivalTimeEvent)) {
                    throw new ImportException("Arrival events on station are not equal.");
                }
                arrival = parsedStationStop.ArrivalTimeEvent;
            }
            else {
                arrival = parsedStationStop.ArrivalTimeEvent ?? parsedStationStop1.ArrivalTimeEvent;
            }

            if (parsedStationStop.DepartureTimeEvent != null && parsedStationStop1.DepartureTimeEvent != null) {

                if (!parsedStationStop.DepartureTimeEvent.Equals(parsedStationStop1.DepartureTimeEvent)) {
                    throw new ImportException("Departure events on station are not equal.");
                }
                departure = parsedStationStop.DepartureTimeEvent;
            }
            else {
                departure = parsedStationStop.DepartureTimeEvent ?? parsedStationStop1.DepartureTimeEvent;
            }

            return new ParsedStationStop {
                ArrivalTimeEvent = arrival,
                DepartureTimeEvent = departure,
                StationName = parsedStationStop.StationName
            };
        }
    }
}
