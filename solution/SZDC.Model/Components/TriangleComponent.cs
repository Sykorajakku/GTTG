using System;
using System.Collections.Generic;
using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Drawing.Canvases;
using SZDC.Model.Views.Traffic;

namespace SZDC.Model.Components {

    public class TriangleComponent : ViewElement {

        private const float ScaledEdgeWidth = 10;
        private readonly SKPath _triangle = new SKPath();
        private readonly SKPaint _trianglePaint = new SKPaint {
            Style = SKPaintStyle.Fill, Color = SKColors.Black, IsAntialias = true
        };

        private readonly SzdcTrainView _trainView;

        public TriangleComponent(SzdcTrainView trainView) {
            _trainView = trainView;
            TopMargin = LeftMargin = RightMargin = BottomMargin = 2;
        }

        protected override void OnDraw(DrawingCanvas drawingCanvas) {

            _trianglePaint.Color = _trainView.PathColor;

            _triangle.Reset();
            _triangle.MoveTo(0, drawingCanvas.Height);
            _triangle.LineTo(drawingCanvas.Width / 2, 0);
            _triangle.LineTo(drawingCanvas.Width, drawingCanvas.Height);
            _triangle.LineTo(0, drawingCanvas.Height);

            drawingCanvas.Canvas.DrawPath(_triangle, _trianglePaint);
        }

        protected override SKSize MeasureOverride(SKSize availableSize) {
            return new SKSize(ScaledEdgeWidth, (float)(ScaledEdgeWidth * Math.Sqrt(3)) / 2);
        }

        public override IEnumerable<IVisual> ProvideVisuals() {
            yield break;
        }
    }
}
