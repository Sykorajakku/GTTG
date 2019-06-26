using System;
using System.Collections.Generic;
using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Component;
using GTTG.Core.Drawing.Canvases;
using GTTG.Core.Drawing.Layers;

namespace SZDC.Editor.Layers {

    /// <summary>
    /// Used by dynamic train timetable as green line on content representing current time.
    /// </summary>
    public class CurrentTimeLayer : ContentDrawingLayer {

        private const float CurrentTimeLineThickness = 4;
        private static readonly SKColor CurrentTimeLineColor = SKColors.Green;
    
        private readonly IViewProvider _timeConverter;
        private readonly IViewProvider _scaleProvider;
        private readonly SKPath _currentTimeVerticalLine;
        private readonly SKPaint _currentTimeVerticalLinePaint;

        public CurrentTimeLayer(IViewProvider timeConverter,
                                IViewProvider scaleProvider) {

            _timeConverter = timeConverter;
            _scaleProvider = scaleProvider;

            _currentTimeVerticalLine = new SKPath();
            _currentTimeVerticalLinePaint = new SKPaint {
                Color = CurrentTimeLineColor, IsAntialias = true, Style = SKPaintStyle.Stroke
            };
        }

        protected override void OnDraw(DrawingCanvas drawingCanvas) {

            var currentDateTime = DateTime.Now;
            var lineHorizontalPosition = _timeConverter.GetContentHorizontalPosition(currentDateTime);

            _currentTimeVerticalLine.Reset();
            _currentTimeVerticalLine.MoveTo(lineHorizontalPosition, 0);
            _currentTimeVerticalLine.LineTo(lineHorizontalPosition, drawingCanvas.Height);

            _currentTimeVerticalLinePaint.StrokeWidth = CurrentTimeLineThickness * _scaleProvider.DpiScale / _scaleProvider.Scale;
            drawingCanvas.Canvas.DrawPath(_currentTimeVerticalLine, _currentTimeVerticalLinePaint);
        }

        public override IEnumerable<IVisual> ProvideVisuals() {
            yield break;
        }
    }
}
