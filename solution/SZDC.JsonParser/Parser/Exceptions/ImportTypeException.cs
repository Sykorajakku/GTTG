using Newtonsoft.Json.Linq;

namespace SZDC.JsonParser.Parser.Exceptions {

    public class ImportTypeException : ImportException {

        public ImportTypeException(JToken invalidToken, string expectedType, string meaning)
            : base($"Json token {invalidToken} in {invalidToken.Path} is not expected {expectedType} of {meaning}.") {
        }
    }
}
