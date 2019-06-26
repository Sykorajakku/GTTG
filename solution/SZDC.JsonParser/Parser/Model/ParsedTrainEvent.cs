namespace SZDC.JsonParser.Parser.Model {

    public class ParsedTrainEvent {

        public int Hours { get; set; }
        public int Minutes { get; set; }
        public bool HasMoreThan30Seconds { get; set; }

        public override bool Equals(object obj) {
            if (!(obj is ParsedTrainEvent parsedTrainEvent)) return false;
            return Hours == parsedTrainEvent.Hours &&
                   Minutes == parsedTrainEvent.Minutes &&
                   HasMoreThan30Seconds == parsedTrainEvent.HasMoreThan30Seconds;
        }
    }
}
