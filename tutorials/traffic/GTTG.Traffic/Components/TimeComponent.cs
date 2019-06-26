using System;
using System.Collections.Generic;
using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Drawing.Canvases;
using GTTG.Model.Model.Events;

namespace GTTG.Traffic.Components {

    public class TimeComponent : ViewElement {

        protected const int DefaultTextSize = 20;

        public float Padding { get; private set; }
        protected static readonly SKPaint TimePaint = new SKPaint {
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
            StrokeWidth = 3,
            Typeface = SKTypeface.FromFamilyName(
            "Arial",
            SKFontStyleWeight.Normal,
            SKFontStyleWidth.Normal,
            SKFontStyleSlant.Upright),
            TextSize = DefaultTextSize
        };

        protected readonly TrainEvent TrainEvent;
        protected SKPath TextPath;

        protected string TextString;
        protected float TextPathY;

        public TimeComponent(TrainEvent trainEvent) {

            TrainEvent = trainEvent;
            TimePaint.TextSize = DefaultTextSize;

            TopMargin = 2;
            BottomMargin = 2;
        }

        protected override void OnDraw(DrawingCanvas drawingCanvas) {
            drawingCanvas.Canvas.DrawPath(TextPath, TimePaint);
        }

        public override IEnumerable<IVisual> ProvideVisuals() {
            yield break;
        }

        protected override SKSize MeasureOverride(SKSize availableSize) {

            float measuredHeight = Math.Abs(TimePaint.FontMetrics.CapHeight);
            Padding = measuredHeight / 10;
            measuredHeight += 2 * Padding;

            TextString = (TrainEvent.DateTime.Minute % 10).ToString();
            var measuredTextWidth = TimePaint.MeasureText(TextString);
            TextPathY = measuredHeight - Padding;
            TextPath = TimePaint.GetTextPath(text: TextString, x:0, y:TextPathY);
            return new SKSize(measuredTextWidth, measuredHeight);
        }
    }
}
