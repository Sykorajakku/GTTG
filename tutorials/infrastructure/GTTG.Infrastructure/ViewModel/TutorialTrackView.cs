using SkiaSharp;

using GTTG.Core.Strategies.Implementations;
using GTTG.Model.Lines;
using GTTG.Model.Model.Infrastructure;
using GTTG.Model.ViewModel.Infrastructure.Tracks;

namespace GTTG.Infrastructure.ViewModel {

    public class TutorialTrackView : TrackView {

        public readonly int Space = 4;

        public TutorialTrackView(Track track, LinePaint linePaint, MeasureableSegment trackLineSegment) 
            : base(track, linePaint, trackLineSegment) {
        }

        protected override SKSize MeasureOverride(SKSize availableSize) {

            var size = base.MeasureOverride(availableSize);
            return new SKSize(size.Width, size.Height + 2 * Space);
        }

        protected override SKSize ArrangeOverride(SKSize finalSize) {

            var scale = finalSize.Height / DesiredSize.Height;
            TrackLineSegment.SetBounds(this, Space * scale, finalSize.Height - Space * scale);
            if (LinePaint.DesiredStrokeWidth > finalSize.Height - 2 * Space) {
                LinePaint.Arrange(LinePaint.DesiredStrokeWidth * scale);
            }

            return new SKSize(float.MaxValue, finalSize.Height);
        }
    }
}
