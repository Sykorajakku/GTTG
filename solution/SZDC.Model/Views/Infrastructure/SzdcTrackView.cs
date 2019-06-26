using GTTG.Core.Strategies.Implementations;
using GTTG.Model.Lines;
using GTTG.Model.Model.Infrastructure;
using GTTG.Model.ViewModel.Infrastructure.Tracks;

namespace SZDC.Model.Views.Infrastructure {

    public class SzdcTrackView : TrackView {

        public SzdcTrackView(Track track, LinePaint linePaint, MeasureableSegment trackLineSegment)
            : base(track, linePaint, trackLineSegment) {
        }
    }
}
