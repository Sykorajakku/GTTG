using SZDC.JsonParser.Parser.Model;

namespace SZDC.JsonParser.Parser {

    public interface IParsedTrainsReceiver {
        void AddTrain(ParsedStaticTrain parsedStaticTrain);
    }
}
