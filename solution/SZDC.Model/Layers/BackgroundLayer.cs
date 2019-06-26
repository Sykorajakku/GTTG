using System;
using System.Collections.Generic;
using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Component;
using GTTG.Core.Drawing.Canvases;
using GTTG.Core.Drawing.Layers;

namespace SZDC.Model.Layers {

    public class BackgroundLayer : ContentDrawingLayer {

        private readonly IViewProvider _viewProvider;
        private readonly SKPath _verticalLine = new SKPath();

        private readonly SKPaint _hourLinePaint = new SKPaint {
            Style = SKPaintStyle.Stroke,
            Color = ViewConstants.TimeLinesColor,
            IsAntialias = true
        };

        private readonly SKPaint _minuteLinePaint = new SKPaint {
            Style = SKPaintStyle.Stroke,
            Color = ViewConstants.TimeLinesColor,
            IsAntialias = true
        };

        public BackgroundLayer(IViewProvider viewProvider) {
            _viewProvider = viewProvider;
        }

        protected override void OnDraw(DrawingCanvas drawingCanvas) {

            var timeContext = _viewProvider.DateTimeContext.ContentDateTimeInterval;

            foreach (var hour in timeContext.GetDateTimesByPeriod(timeContext.Start, TimeSpan.FromHours(1))) {
                
                _verticalLine.Reset();
                var canvasX = _viewProvider.GetContentHorizontalPosition(hour);

                _verticalLine.MoveTo(new SKPoint(canvasX, 0));
                _verticalLine.LineTo(new SKPoint(canvasX, drawingCanvas.Size.Height));

                _hourLinePaint.StrokeWidth = 5 * _viewProvider.DpiScale / _viewProvider.Scale;
                drawingCanvas.Canvas.DrawPath(_verticalLine, _hourLinePaint);
            }

            foreach (var minute in timeContext.GetDateTimesByPeriod(timeContext.Start, TimeSpan.FromMinutes(10))) {

                if (minute.Minute == 00 || minute.Minute == 30) {
                    continue;
                }

                _verticalLine.Reset();
                var canvasX = _viewProvider.GetContentHorizontalPosition(minute);

                _verticalLine.MoveTo(new SKPoint(canvasX, 0));
                _verticalLine.LineTo(new SKPoint(canvasX, drawingCanvas.Size.Height));

                _minuteLinePaint.StrokeWidth = 2 * _viewProvider.DpiScale / _viewProvider.Scale;
                drawingCanvas.Canvas.DrawPath(_verticalLine, _minuteLinePaint);
            }

            var halfHourLinePaint = new SKPaint {

                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1 * _viewProvider.DpiScale / _viewProvider.Scale,
                Color = ViewConstants.TrackLinesColor,
                IsAntialias = true,
                PathEffect = SKPathEffect.CreateDash(
                    new [] {
                        10 / _viewProvider.Scale,
                        20 / _viewProvider.Scale},
                        10 / _viewProvider.Scale)
            };


            foreach (var halfHour in timeContext.GetDateTimesByPeriod(timeContext.Start, TimeSpan.FromMinutes(30))) {

                if (halfHour.Minute != 30) {
                    continue;
                }

                _verticalLine.Reset();
                var canvasX =_viewProvider.GetContentHorizontalPosition(halfHour);

                _verticalLine.MoveTo(new SKPoint(canvasX, 0));
                _verticalLine.LineTo(new SKPoint(canvasX, drawingCanvas.Size.Height));

                drawingCanvas.Canvas.DrawPath(_verticalLine, halfHourLinePaint);
            }
        }

        public override IEnumerable<IVisual> ProvideVisuals() {
            yield break;
        }
    }
}
