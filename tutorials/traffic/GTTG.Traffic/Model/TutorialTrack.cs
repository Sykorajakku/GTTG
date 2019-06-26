using GTTG.Model.Model.Infrastructure;

namespace GTTG.Traffic.Model {

    public class TutorialTrack : Track {

        public TrackType TrackType { get; }

        public TutorialTrack(TrackType trackType) {
            TrackType = trackType;
        }
    }
}
