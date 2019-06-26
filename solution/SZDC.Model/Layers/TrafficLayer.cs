using System.Collections.Generic;

using GTTG.Core.Base;
using GTTG.Core.Drawing.Canvases;
using GTTG.Core.Drawing.Layers;
using SZDC.Model.Views;

namespace SZDC.Model.Layers {

    public class TrafficLayer : ContentDrawingLayer {

        private readonly IViewModel _viewModel;

        public TrafficLayer(IViewModel viewModel) {
            _viewModel = viewModel;
        }

        protected override void OnDraw(DrawingCanvas drawingCanvas) {
            drawingCanvas.Draw(_viewModel.TrafficView);
        }

        public override IEnumerable<IVisual> ProvideVisuals() {
            yield return _viewModel.TrafficView;
        }
    }
}
 