using System;
using System.Collections.Generic;
using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Component;
using GTTG.Core.Drawing.Canvases;
using GTTG.Core.Drawing.Layers;

namespace GTTG.Traffic.Layers {

    public class TimeAxisLayer : ContentDrawingLayer {

        private readonly IViewProvider _viewProvider;

        public float Height { get; }

        private static readonly SKPaint TimePaint = new SKPaint {
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
            StrokeWidth = 3,
            Typeface = SKTypeface.FromFamilyName(
                "Arial",
                SKFontStyleWeight.Normal,
                SKFontStyleWidth.Normal,
                SKFontStyleSlant.Upright),
            TextSize = 14
        };

        private static readonly SKPaint RectPaint = new SKPaint {
            Style = SKPaintStyle.Fill,
            Color = SKColors.White
        };

        protected float Padding { get; }

        public TimeAxisLayer(IViewProvider viewProvider) {

            _viewProvider = viewProvider;
            var measuredHeight = Math.Abs(TimePaint.FontMetrics.CapHeight);
            Padding = measuredHeight / 10;
            measuredHeight += 2 * Padding;
            Height = measuredHeight;
        }

        protected override void OnDraw(DrawingCanvas drawingCanvas) {

            drawingCanvas.Canvas.DrawRect(SKRect.Create(SKPoint.Empty, new SKSize(drawingCanvas.Width, Height)), RectPaint);
            
            var interval = _viewProvider.DateTimeContext.ContentDateTimeInterval;
            foreach (var dateTime in interval.GetDateTimesByPeriod(interval.Start,TimeSpan.FromMinutes(10))) {

                var x = _viewProvider.GetContentHorizontalPosition(dateTime);
                drawingCanvas.Canvas.DrawText(dateTime.ToString("HH:mm"), new SKPoint(x, Height - Padding), TimePaint);
            }
        }

        public override IEnumerable<IVisual> ProvideVisuals() {
            yield break;
        }
    }
}
