using System;
using System.Collections.Generic;
using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Drawing.Canvases;
using GTTG.Model.Model.Events;
using SZDC.Model.Views.Traffic;

namespace SZDC.Model.Components {
    
    public class TimeComponent : SelectableComponent {

        protected const int DefaultTextSize = 20;
        protected const int UnderscoreSize = DefaultTextSize / 10;

        protected static readonly SKPaint TimePaint = new SKPaint { Style = SKPaintStyle.Fill, IsAntialias = true, StrokeWidth = 3, Typeface = SKTypeface.FromFamilyName(
            "Arial",
            SKFontStyleWeight.Normal,
            SKFontStyleWidth.Normal,
            SKFontStyleSlant.Upright),
            TextSize = DefaultTextSize
        };

        protected static readonly SKPaint UnderscorePaint = new SKPaint { Style = SKPaintStyle.Stroke, IsAntialias = true, StrokeWidth = 1 };

        public float Padding { get; private set; }
        public TrainEvent TrainEvent { get; }
        public SzdcTrainView TrainView { get; }

        protected SKPath TextPath;
        protected string TextString;
        protected float TextPathY;

        public TimeComponent(SzdcTrainView trainView, TrainEvent trainEvent)
            : base(trainView) {

            TrainEvent = trainEvent;
            TrainView = trainView;
            TimePaint.TextSize = DefaultTextSize;

            // margin so component is not drawn directly on line but has space between instead
            TopMargin = 2;
            BottomMargin = 2;
        }

        protected override void DrawComponent(DrawingCanvas drawingCanvas) {

            TimePaint.Color = TrainView.PathColor;
            drawingCanvas.Canvas.DrawPath(TextPath, TimePaint);

            UnderscorePaint.Color = TimePaint.Color;

            // apply rule: draw underline if > 30 sec
            if (TrainEvent.DateTime.Second >= 30) {

                var underscoreHeight = drawingCanvas.Height - UnderscoreSize / 2f;
                drawingCanvas.Canvas.DrawLine(new SKPoint(0, underscoreHeight), new SKPoint(drawingCanvas.Width, underscoreHeight), UnderscorePaint);
            }
        }

        protected override SKSize MeasureOverride(SKSize availableSize) {

            float measuredHeight = Math.Abs(TimePaint.FontMetrics.CapHeight);
            Padding = measuredHeight / 10;

            measuredHeight += 2 * Padding;
            measuredHeight += UnderscoreSize;

            TextString = (TrainEvent.DateTime.Minute % 10).ToString();
            var measuredTextWidth = TimePaint.MeasureText(TextString);
            TextPathY = measuredHeight - Padding - UnderscoreSize;
            TextPath = TimePaint.GetTextPath(TextString, 0, TextPathY);
            return new SKSize(measuredTextWidth, measuredHeight);
        }

        public override IEnumerable<IVisual> ProvideVisuals() {
            yield break;
        }
    }
}
