using SkiaSharp;

using GTTG.Core.Drawing.Canvases;
using GTTG.Core.Utils;
using GTTG.Model.Model.Events;
using SZDC.Model.Views.Traffic;

namespace SZDC.Model.Components {

    /// <summary>
    /// Draws circle around time component, useful for future implementation of not yet supported rule (rule: koty k rusicim vlaku)
    /// </summary>
    public class CircumscribedTimeComponent : TimeComponent {
        
        private SKSize _timeRectangleSize;
        private float _diameter;
        private static readonly SKPaint CirclePaint =
            new SKPaint { StrokeWidth = 2, Style = SKPaintStyle.Stroke, IsAntialias = true, Color = SKColors.Red };

        public CircumscribedTimeComponent(SzdcTrainView trainView, TrainEvent trainEvent)
            : base(trainView, trainEvent) {
        }

        protected override void DrawComponent(DrawingCanvas drawingCanvas) {

            TimePaint.Color = TrainView.PathColor;
            drawingCanvas.Canvas.DrawPath(TextPath, TimePaint);

            UnderscorePaint.Color = TimePaint.Color;

            if (TrainEvent.DateTime.Second > 30) {

                var x = (_diameter - _timeRectangleSize.Width) / 2 + CirclePaint.StrokeWidth;
                var y = (_diameter - _timeRectangleSize.Height) / 2 + _timeRectangleSize.Height - UnderscoreSize / 2f + CirclePaint.StrokeWidth / 2;
                drawingCanvas.Canvas.DrawLine(new SKPoint(x, y), new SKPoint(x + _timeRectangleSize.Width, y), UnderscorePaint);
            }

            var circleMid = _diameter / 2 + CirclePaint.StrokeWidth;
            drawingCanvas.Canvas.DrawCircle(circleMid, circleMid, _diameter / 2, CirclePaint);
        }

        protected override SKSize MeasureOverride(SKSize availableSize) {

            _timeRectangleSize = base.MeasureOverride(availableSize);
            _diameter = PlacementUtils.ComputesVectorLength(new SKPoint(_timeRectangleSize.Width, _timeRectangleSize.Height));

            var x = (_diameter - _timeRectangleSize.Width) / 2 + CirclePaint.StrokeWidth;
            var y = TextPathY + (_diameter - _timeRectangleSize.Height) / 2 + CirclePaint.StrokeWidth;
            TextPath = TimePaint.GetTextPath(TextString, x, y);

            var edgeLength = _diameter + 2 * CirclePaint.StrokeWidth;
            return new SKSize(edgeLength, edgeLength);
        }
    }
}
