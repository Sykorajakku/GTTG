using GTTG.Model.Model.Infrastructure;

namespace SZDC.Model.Infrastructure {

    /// <inheritdoc />
    public class SzdcTrack : Track {

        public TrackType TrackType { get; }

        public string TrackName { get; }

        public string StationName { get; }

        public SzdcTrack(TrackType trackType, string trackName, string stationName) {
            TrackType = trackType;
            TrackName = trackName;
            StationName = stationName;
        }

        public override bool Equals(object obj) {

            if (obj is SzdcTrack other) {
                return TrackType == other.TrackType &&
                       TrackName == other.TrackName &&
                       StationName == other.StationName;
            }
            return false;
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = (int) TrackType;
                hashCode = (hashCode * 397) ^ (TrackName != null ? TrackName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (StationName != null ? StationName.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
