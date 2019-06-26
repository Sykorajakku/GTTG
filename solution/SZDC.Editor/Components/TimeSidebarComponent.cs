using System;
using SkiaSharp;

using GTTG.Core.Component;
using GTTG.Core.Drawing.Canvases;
using GTTG.Core.Drawing.Layers;
using SZDC.Editor.Interfaces;

namespace SZDC.Editor.Components {

    /// <summary>
    /// Horizontal line with time values above train timetable diagram.
    /// </summary>
    public class TimeSidebarComponent {

        /// <summary>
        /// Requests <see cref="Draw"/> call.
        /// </summary>
        public event ViewModifiedHandler ViewModified;

        private float _height;
        private readonly IViewProvider _viewProvider;
        private readonly SKPaint _textPaint = new SKPaint { TextSize = 20, Style = SKPaintStyle.Fill, Color = SKColors.Black, IsAntialias = true };
        private readonly Func<DateTime, string> _shortIntervalFormat = d => d.ToString("H:mm"); // draws minutes if zoomed
        private readonly Func<DateTime, string> _longIntervalFormat = d => $"{d.Hour}"; // draws only hour values

        public TimeSidebarComponent(IViewProvider viewProvider, IViewModifiedNotifier viewModifiedNotifier) {

            _viewProvider = viewProvider;
            viewModifiedNotifier.ViewModified += () => ViewModified?.Invoke();
        }

        /// <summary>
        /// Sets height of the line of time values.
        /// </summary>
        public void SetHeight(float height) {
            _height = height;
        }

        public void Draw(SKSurface surface) {

            var viewMatrix = _viewProvider.ContentMatrix;
            var timeSidebarMatrix = SKMatrix.MakeIdentity();
            timeSidebarMatrix.TransX = viewMatrix.TransX;

            surface.Canvas.Clear();
            surface.Canvas.SetMatrix(timeSidebarMatrix);

            var drawingCanvas = new DrawingCanvas(DefaultDrawingLayer.Get, surface.Canvas, new SKSize(_viewProvider.ContentWidth, _viewProvider.ContentHeight), _viewProvider.GetViewRect());
            var contentDateTimeInterval = _viewProvider.DateTimeContext.ContentDateTimeInterval;
            var canvasViewTimeInterval = _viewProvider.DateTimeContext.ViewDateTimeInterval;

            // select displayed time values and their format
            var isShortTimespan = canvasViewTimeInterval.TimeSpan <= new TimeSpan(hours: 2, minutes: 15, seconds: 0);
            var timePeriod = isShortTimespan ? TimeSpan.FromMinutes(20) : TimeSpan.FromHours(1);
            var formatTime = isShortTimespan ? _shortIntervalFormat : _longIntervalFormat;

            foreach (var time in contentDateTimeInterval.GetDateTimesByPeriod(contentDateTimeInterval.Start, timePeriod)) {

                if (time < canvasViewTimeInterval.Start || time > canvasViewTimeInterval.End) {
                    continue;
                }

                var text = formatTime(time);
                var textTranslation = _textPaint.MeasureText(text) / (2 * _viewProvider.Scale);
                var horizontalPosition = (_viewProvider.GetContentHorizontalPosition(time) - textTranslation) * _viewProvider.Scale;
                var textPoint = new SKPoint(horizontalPosition, _height * 0.9f);
                drawingCanvas.Canvas.DrawText(formatTime(time), textPoint, _textPaint);
            }
        }
    }
}
