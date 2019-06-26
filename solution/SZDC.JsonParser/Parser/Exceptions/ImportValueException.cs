using Newtonsoft.Json.Linq;

namespace SZDC.JsonParser.Parser.Exceptions {

    public class ImportValueException : ImportException {

        public ImportValueException(JToken invalidToken, string valueType)
            : base($"Json token {invalidToken} in {invalidToken.Path} is not {valueType} value.") {
        }
    }
}
