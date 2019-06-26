using System.Collections.Generic;
using GTTG.Core.Base;
using GTTG.Core.Drawing.Canvases;
using GTTG.Core.Drawing.Layers;
using GTTG.Model.Model.Traffic;
using GTTG.Model.ViewModel.Traffic;
using GTTG.Traffic.ViewModel.Traffic;

namespace GTTG.Traffic.Layers {

    public class TrafficLayer : ContentDrawingLayer {

        private readonly TrafficView<TutorialTrainView, Train> _trafficView;

        public TrafficLayer(TrafficView<TutorialTrainView, Train> trafficView) {
            _trafficView = trafficView;
        }

        protected override void OnDraw(DrawingCanvas drawingCanvas) {
            drawingCanvas.Draw(_trafficView);
        }

        public override IEnumerable<IVisual> ProvideVisuals() {
            yield return _trafficView;
        }
    }
}
