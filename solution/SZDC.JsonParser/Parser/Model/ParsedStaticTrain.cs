using System.Collections.Generic;

using SZDC.Model.Infrastructure.Trains;

namespace SZDC.JsonParser.Parser.Model {

    public struct ParsedStaticTrain {

        public List<ParsedStationStop> Schedule { get; set; }
        public int TrainNumber { get; set; }
        public TrainType TrainType { get; set; }
    }
}
