using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using SkiaSharp;

using GTTG.Core.Drawing.Canvases;
using GTTG.Model.Lines;
using SZDC.Model.Infrastructure.Trains;
using TrainEvent = GTTG.Model.Model.Events.TrainEvent;

namespace SZDC.Model.Views.TrainPaths {

    public class DottedEndPointsTrainPath : ITrainPath {

        private readonly SKPaint _pointPaint;
        private readonly ITrainPath _trainMovementPoints;
        private readonly float _pointRadius;
        private readonly bool _hasDotAtStart;
        private readonly bool _hasDostAtEnd;

        public DottedEndPointsTrainPath(ITrainPath trainMovementPoints, SzdcTrain train) {
            _trainMovementPoints = trainMovementPoints;
            _pointPaint = new SKPaint {Style = SKPaintStyle.Fill, IsAntialias = true };
            _pointRadius = 3;
            _hasDotAtStart = !train.IsArrivingFromOtherRailway;
            _hasDostAtEnd = !train.IsDepartingToOtherRailway;
        }

        public LinePaint LinePaint => _trainMovementPoints.LinePaint;
        public SKPoint this[int index] => _trainMovementPoints[index];

        public int PointCount => _trainMovementPoints.PointCount;

        public void Clear() {
            _trainMovementPoints.Clear();
        }

        public void Arrange() {
            _trainMovementPoints.Arrange();
        }

        public void Update(ImmutableArray<TrainEvent> schedule) {
            _trainMovementPoints.Update(schedule);
        }
        
        public void Draw(DrawingCanvas drawingCanvas) {

            _trainMovementPoints.Draw(drawingCanvas);

            var firstPoint = PointsByTrainPathEvents[TrainPathEvents[0]].PathPoint;
            var lastPoint = PointsByTrainPathEvents[TrainPathEvents[TrainPathEvents.Count - 1]].PathPoint;

            if (_hasDotAtStart) {
                DrawCargoEndpointDot(drawingCanvas, firstPoint);
            }

            if (_hasDostAtEnd) {
                DrawCargoEndpointDot(drawingCanvas, lastPoint);
            }
        }

        private void DrawCargoEndpointDot(DrawingCanvas drawingCanvas, SKPoint point) {

            _pointPaint.Color = PathColor;
            drawingCanvas.Canvas.DrawCircle(point, _pointRadius, _pointPaint);

            _pointPaint.Color = SKColors.Black;
            drawingCanvas.Canvas.DrawCircle(point, _pointRadius * 3f / 4, _pointPaint);
        }

        public float MeasurePathStrokeWidth() {
            return Math.Max(_pointRadius * 2, _trainMovementPoints.MeasurePathStrokeWidth());
        }

        public float DistanceFromPoint(SKPoint point) {
            return _trainMovementPoints.DistanceFromPoint(point);
        }

        public SKColor PathColor {
            get => _trainMovementPoints.PathColor;
            set => _trainMovementPoints.PathColor = value;
        }

        public IReadOnlyDictionary<TrainEvent, (int Index, SKPoint PathPoint)> PointsByTrainPathEvents =>
            _trainMovementPoints.PointsByTrainPathEvents;

        public IReadOnlyList<TrainEvent> TrainPathEvents =>
            _trainMovementPoints.TrainPathEvents;
    }
}
