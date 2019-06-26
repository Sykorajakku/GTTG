using System;
using System.Collections.Generic;
using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Component;
using GTTG.Core.Drawing.Canvases;
using GTTG.Core.Drawing.Layers;

namespace GTTG.Infrastructure.Layers {

    public class TimeLinesLayer : ContentDrawingLayer {

        private readonly IViewProvider _viewProvider;

        private readonly SKPaint _hourLinePaint = new SKPaint {
            Style = SKPaintStyle.Stroke,
            Color = new SKColor(228, 154, 1),
            IsAntialias = true
        };

        private readonly SKPaint _minuteLinePaint = new SKPaint {
            Style = SKPaintStyle.Stroke,
            Color = new SKColor(228, 154, 1),
            IsAntialias = true
        };

        private readonly SKPaint _halfHourLinePaint = new SKPaint {

            Style = SKPaintStyle.Stroke,
            Color = new SKColor(228, 154, 1),
            IsAntialias = true,
        };

        private readonly SKPath _verticalHourLine = new SKPath();

        public TimeLinesLayer(IViewProvider viewProvider) {
            _viewProvider = viewProvider;
        }

        protected override void OnDraw(DrawingCanvas drawingCanvas) {

            var timeContext = _viewProvider.DateTimeContext.ContentDateTimeInterval;

            foreach (var hour in timeContext.GetDateTimesByPeriod(timeContext.Start, TimeSpan.FromHours(1))) {

                _verticalHourLine.Reset();
                var canvasX = _viewProvider.GetContentHorizontalPosition(hour);

                _verticalHourLine.MoveTo(new SKPoint(canvasX, 0));
                _verticalHourLine.LineTo(new SKPoint(canvasX, drawingCanvas.Size.Height));

                _hourLinePaint.StrokeWidth = 5 / _viewProvider.Scale;
                drawingCanvas.Canvas.DrawPath(_verticalHourLine, _hourLinePaint);
            }

            foreach (var minute in timeContext.GetDateTimesByPeriod(timeContext.Start, TimeSpan.FromMinutes(10))) {

                if (minute.Minute == 00 || minute.Minute == 30) {
                    continue;
                }

                _verticalHourLine.Reset();
                var canvasX = _viewProvider.GetContentHorizontalPosition(minute);

                _verticalHourLine.MoveTo(new SKPoint(canvasX, 0));
                _verticalHourLine.LineTo(new SKPoint(canvasX, drawingCanvas.Size.Height));

                _minuteLinePaint.StrokeWidth = 2 / _viewProvider.Scale;
                drawingCanvas.Canvas.DrawPath(_verticalHourLine, _minuteLinePaint);
            }

            _halfHourLinePaint.StrokeWidth = 1 * _viewProvider.DpiScale / _viewProvider.Scale;
            _halfHourLinePaint.PathEffect = SKPathEffect.CreateDash(
                new[] { 10 / _viewProvider.Scale, 20 / _viewProvider.Scale },
                10 / _viewProvider.Scale);

            foreach (var halfHour in timeContext.GetDateTimesByPeriod(timeContext.Start, TimeSpan.FromMinutes(30))) {

                if (halfHour.Minute != 30) {
                    continue;
                }

                _verticalHourLine.Reset();
                var canvasX = _viewProvider.GetContentHorizontalPosition(halfHour);
                _verticalHourLine.MoveTo(new SKPoint(canvasX, 0));
                _verticalHourLine.LineTo(new SKPoint(canvasX, drawingCanvas.Size.Height));
                drawingCanvas.Canvas.DrawPath(_verticalHourLine, _halfHourLinePaint);
            }
        }

        public override IEnumerable<IVisual> ProvideVisuals() {
            yield break;
        }
    }
}
