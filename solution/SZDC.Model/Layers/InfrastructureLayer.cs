using System.Collections.Generic;

using GTTG.Core.Base;
using GTTG.Core.Drawing.Canvases;
using GTTG.Core.Drawing.Layers;
using SZDC.Model.Views;

namespace SZDC.Model.Layers {
    
    public class InfrastructureLayer : ContentDrawingLayer {

        private readonly IViewModel _viewModel;

        public InfrastructureLayer(IViewModel viewModel) {
            _viewModel = viewModel;
        }

        protected override void OnDraw(DrawingCanvas drawingCanvas) {
            drawingCanvas.Draw(_viewModel.RailwayView);
        }

        public override IEnumerable<IVisual> ProvideVisuals() {
            yield return _viewModel.RailwayView;
        }
    }
}
