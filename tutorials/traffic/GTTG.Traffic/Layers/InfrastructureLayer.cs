using System.Collections.Generic;

using GTTG.Core.Base;
using GTTG.Core.Drawing.Canvases;
using GTTG.Core.Drawing.Layers;
using GTTG.Model.ViewModel.Infrastructure.Railways;
using GTTG.Traffic.ViewModel;

namespace GTTG.Traffic.Layers {

    public class InfrastructureLayer : ContentDrawingLayer {

        private readonly StrategyRailwayView<TutorialStationView, TutorialTrackView> _railwayView;

        public InfrastructureLayer(StrategyRailwayView<TutorialStationView, TutorialTrackView> railwayView) {
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
