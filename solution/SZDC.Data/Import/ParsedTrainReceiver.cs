using System.Collections.Generic;

using SZDC.JsonParser.Parser;
using SZDC.JsonParser.Parser.Model;
using SZDC.Model.Infrastructure.Trains;

namespace SZDC.Data.Import {

    public class ParsedTrainReceiver : IParsedTrainsReceiver {

        public Dictionary<int, List<List<ParsedStationStop>>> TrainsWithSchedule { get; set; }
        public Dictionary<int, List<TrainType>> TrainTypes { get; set; }

        public ParsedTrainReceiver() {

            TrainsWithSchedule = new Dictionary<int, List<List<ParsedStationStop>>>();
            TrainTypes = new Dictionary<int, List<TrainType>>();
        }

        public void AddTrain(ParsedStaticTrain parsedStaticTrain) {

            var trainNumber = parsedStaticTrain.TrainNumber;

            if (TrainsWithSchedule.TryGetValue(trainNumber, out var schedule)) {
                schedule.Add(parsedStaticTrain.Schedule);
            }
            else {
                var list = new List<List<ParsedStationStop>> {parsedStaticTrain.Schedule};
                TrainsWithSchedule.Add(trainNumber, list);
            }

            if (TrainTypes.TryGetValue(trainNumber, out var trainType)) {

                if (!trainType.Contains(parsedStaticTrain.TrainType)) {
                    trainType.Add(parsedStaticTrain.TrainType);
                }
            }
            else {
                TrainTypes.Add(trainNumber, new List<TrainType> { parsedStaticTrain.TrainType });
            }
        }
    }
}
