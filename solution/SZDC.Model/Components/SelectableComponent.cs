using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Drawing.Canvases;
using SZDC.Model.Views.Traffic;

namespace SZDC.Model.Components {

    /// <summary>
    /// If derived from, background is drawn under deriving component if it's train is currently selected
    /// </summary>
    public abstract class SelectableComponent : ViewElement {

        private readonly SzdcTrainView _trainView;

        protected SelectableComponent(SzdcTrainView trainView) {
            _trainView = trainView;
        }

        protected sealed override void OnDraw(DrawingCanvas drawingCanvas) {

            if (_trainView.IsSelected) {

                var sp = drawingCanvas.Canvas.Save();
                drawingCanvas.Canvas.ClipRect(Clip);

                // draw background to hide other elements beneath
                drawingCanvas.Canvas.DrawColor(SKColors.WhiteSmoke);
                DrawComponent(drawingCanvas);

                // remove clip
                drawingCanvas.Canvas.RestoreToCount(sp);
            }

            else {
                DrawComponent(drawingCanvas);
            }
        }

        /// <summary>
        /// Draws content of derived component in it's desired size
        /// </summary>
        protected abstract void DrawComponent(DrawingCanvas drawingCanvas);
    }
}
