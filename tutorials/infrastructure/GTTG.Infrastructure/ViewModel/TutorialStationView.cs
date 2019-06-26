using SkiaSharp;

using GTTG.Core.Drawing.Canvases;
using GTTG.Model.Model.Infrastructure;
using GTTG.Model.ViewModel.Infrastructure.Stations;
using GTTG.Model.ViewModel.Infrastructure.Tracks;

namespace GTTG.Infrastructure.ViewModel {

    public class TutorialStationView : StationView<TutorialTrackView> {

        public TutorialStationView(Station station, ITrackViewFactory<TutorialTrackView> trackViewFactory)
            : base(station, trackViewFactory) {

            HasClipEnabled = true; // apply clip, because DrawColor would otherwise cover whole canvas
        }

        protected override void OnDraw(DrawingCanvas drawingCanvas) {


            drawingCanvas.Canvas.DrawColor(SKColors.WhiteSmoke);
            foreach (var trackView in TrackViews) {
                drawingCanvas.Draw(trackView);
            }
        }
    }
}
