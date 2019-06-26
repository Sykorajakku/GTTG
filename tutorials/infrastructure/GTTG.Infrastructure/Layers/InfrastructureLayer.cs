using System.Collections.Generic;
using GTTG.Core.Base;
using GTTG.Core.Drawing.Canvases;
using GTTG.Core.Drawing.Layers;
using GTTG.Infrastructure.ViewModel;
using GTTG.Model.ViewModel.Infrastructure.Railways;

namespace GTTG.Infrastructure.Layers {

    public class InfrastructureLayer : ContentDrawingLayer {

        private readonly RailwayView<TutorialStationView,TutorialTrackView> _railwayView;

        public InfrastructureLayer(RailwayView<TutorialStationView, TutorialTrackView> railwayView) {
            _railwayView = railwayView;
        }

        protected override void OnDraw(DrawingCanvas drawingCanvas) {
            drawingCanvas.Draw(_railwayView);
        }

        public override IEnumerable<IVisual> ProvideVisuals() {
            yield return _railwayView;
        }
    }
}
