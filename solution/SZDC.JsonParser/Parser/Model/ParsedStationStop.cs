using SZDC.JsonParser.Parser.Exceptions;

namespace SZDC.JsonParser.Parser.Model {

    public struct ParsedStationStop {

        public ParsedTrainEvent ArrivalTimeEvent { get; set; }
        public ParsedTrainEvent DepartureTimeEvent { get; set; }
        public string StationName { get; set; }

        public bool IsBefore(ParsedStationStop other) {

            var lowerBound = DepartureTimeEvent ?? ArrivalTimeEvent;
            var upperBound = other.ArrivalTimeEvent ?? other.DepartureTimeEvent;

            if (lowerBound == null) {
                throw new ImportException($"Event on {StationName} was null with both departure and arrival");
            }

            if (upperBound == null) {
                throw new ImportException($"Event on {other.StationName} was null with both departure and arrival");
            }

            if (lowerBound.Hours < upperBound.Hours) {
                return true;
            }
            else if (lowerBound.Hours > upperBound.Hours) {
                return false;
            }
            else {

                if (lowerBound.Minutes < upperBound.Minutes) {
                    return true;
                }
                else if (lowerBound.Minutes > upperBound.Minutes) {
                    return false;
                }
                else if (lowerBound.HasMoreThan30Seconds && !upperBound.HasMoreThan30Seconds) {
                    return false;
                }

                return true;
            }
        }

        public override bool Equals(object obj) {

            if (!(obj is ParsedStationStop other)) return false;
            return ((Equals(ArrivalTimeEvent, other.ArrivalTimeEvent)
                     || Equals(DepartureTimeEvent, other.DepartureTimeEvent)))
                   && StationName == other.StationName;
        }

        public bool Equals(ParsedStationStop other) {

            var t1 = Equals(ArrivalTimeEvent, other.ArrivalTimeEvent);
            var t2 = Equals(DepartureTimeEvent, other.DepartureTimeEvent);
            var t3 = StationName == other.StationName;

            return (t1 || t2) && t3;
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = (ArrivalTimeEvent != null ? ArrivalTimeEvent.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (DepartureTimeEvent != null ? DepartureTimeEvent.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (StationName != null ? StationName.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString(){
            return $@"Arrival {ArrivalTimeEvent} 
          Departure {DepartureTimeEvent}
          {StationName}";
        }

        public bool IsOverMidnight(ParsedStationStop current) {

            var previousEvent = DepartureTimeEvent ?? ArrivalTimeEvent;
            var nextEvent = ArrivalTimeEvent ?? DepartureTimeEvent;
            return nextEvent != null && previousEvent.Hours > 20 && nextEvent.Hours >= 0; // test 4 hour window, heuristic
        }
    }
}
