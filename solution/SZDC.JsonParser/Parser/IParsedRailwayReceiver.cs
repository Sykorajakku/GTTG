using SZDC.JsonParser.Parser.Model;

namespace SZDC.JsonParser.Parser {

    public interface IParsedRailwayReceiver {
        void AddRailway(ParsedRailway parsedRailway);
    }
}
