using System.Linq;
using SkiaSharp;

using GTTG.Core.Drawing.Canvases;
using GTTG.Core.Strategies.Implementations;
using GTTG.Core.Strategies.Interfaces;
using GTTG.Model.Model.Infrastructure;
using GTTG.Model.Strategies.Types;
using GTTG.Model.ViewModel.Infrastructure.Stations;
using GTTG.Model.ViewModel.Infrastructure.Tracks;

namespace GTTG.Traffic.ViewModel {

    public class TutorialStationView : StrategyStationView<TutorialTrackView> {

        public TutorialStationView(Station station,
            ITrackViewFactory<TutorialTrackView> trackViewFactory,
            ISegmentRegistry<SegmentType<Track>, MeasureableSegment> segmentRegistry)
            : base(station, segmentRegistry, trackViewFactory) {

            HasClipEnabled = true; // apply clip, because DrawColor would otherwise cover whole canvas

            foreach (var track in TrackViews.Select(t => t.Track)) {

                TrackSegments.Resolve(new SegmentType<Track>(track, SegmentPlacement.Lower)).HeightMeasureHelpers += MeasureSegmentHeight;
                TrackSegments.Resolve(new SegmentType<Track>(track, SegmentPlacement.Upper)).HeightMeasureHelpers += MeasureSegmentHeight;
            }
        }
         
        protected override void OnDraw(DrawingCanvas drawingCanvas) {

            drawingCanvas.Canvas.DrawColor(SKColors.WhiteSmoke);
            foreach (var trackView in TrackViews) {
                    drawingCanvas.Draw(trackView);
                }
        }

        private static float MeasureSegmentHeight() => 15;
    }
}
