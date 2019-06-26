using System.Collections.Generic;

using GTTG.Core.Base;
using GTTG.Core.Drawing.Canvases;
using GTTG.Core.Drawing.Layers;
using SZDC.Editor.Tools;

namespace SZDC.Editor.Layers {

    public class SelectedTrainLayer : ContentDrawingLayer {

        private readonly TrainViewSelectionTool _trainViewSelectionTool;

        public SelectedTrainLayer(TrainViewSelectionTool trainViewSelectionTool) {
            _trainViewSelectionTool = trainViewSelectionTool;
        }
        
        protected override void OnDraw(DrawingCanvas drawingCanvas) {

            if (_trainViewSelectionTool.SelectedTrainView == null) return;
            drawingCanvas.Draw(_trainViewSelectionTool.SelectedTrainView);
        }

        public override IEnumerable<IVisual> ProvideVisuals() {
            if (_trainViewSelectionTool.SelectedTrainView != null) {
                yield return _trainViewSelectionTool.SelectedTrainView;
            }
        }
    }
}
