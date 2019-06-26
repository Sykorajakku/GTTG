using System;
using System.Collections.Generic;
using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Drawing.Canvases;
using SZDC.Model.Views.Traffic;

namespace SZDC.Model.Components {

    public class TrainNumberComponent : SelectableComponent {

        private const int DefaultTextSize = 30;

        public string Id { get; }
        public float Padding { get; private set; }
        public static SKPaint TextPaint = new SKPaint { Style = SKPaintStyle.Fill, IsAntialias = true };

        private readonly SzdcTrainView _trainView;
        private SKPath _textPath;

        public TrainNumberComponent(SzdcTrainView trainView) 
            : base(trainView) {

            Id = trainView.Train.TrainNumber.ToString();
            _trainView = trainView;

            // add margins to have space between train path and text
            TopMargin = 4;
            BottomMargin = 4;
        }
        
        protected override void DrawComponent(DrawingCanvas drawingCanvas) {
            TextPaint.Color = _trainView.PathColor;
            drawingCanvas.Canvas.DrawPath(_textPath, TextPaint);
        }

        protected override SKSize MeasureOverride(SKSize availableSize) {

            TextPaint.TextSize = DefaultTextSize;
            var measuredHeight = Math.Abs(TextPaint.FontMetrics.CapHeight);
            Padding = measuredHeight / 10;

            measuredHeight += 2 * Padding;
            var measuredWidth = TextPaint.MeasureText(Id);
            _textPath = TextPaint.GetTextPath(Id, 0, measuredHeight - Padding);

            return new SKSize(measuredWidth, measuredHeight);
        }

        public override IEnumerable<IVisual> ProvideVisuals() {
            yield break;
        }
    }
}
