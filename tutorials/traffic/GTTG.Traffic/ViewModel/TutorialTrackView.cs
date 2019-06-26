using GTTG.Core.Strategies.Implementations;
using GTTG.Model.Lines;
using GTTG.Model.Model.Infrastructure;
using GTTG.Model.ViewModel.Infrastructure.Tracks;

namespace GTTG.Traffic.ViewModel {

    public class TutorialTrackView : TrackView {

        public TutorialTrackView(Track track, LinePaint trackLine, MeasureableSegment trackLineSegment)
            : base(track, trackLine, trackLineSegment) {
        }
    }
}
