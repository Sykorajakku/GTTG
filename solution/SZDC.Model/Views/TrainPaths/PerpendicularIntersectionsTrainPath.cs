using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using SkiaSharp;

using GTTG.Core.Drawing.Canvases;
using GTTG.Model.Lines;
using GTTG.Model.Model.Events;

namespace SZDC.Model.Views.TrainPaths {

    public class PerpendicularIntersectionsTrainPath : ITrainPath { 

        protected SKPath BackUpPath { get; }
        protected SKPath LineIntersection { get; }
        protected SKPaint LineIntersectionPaint { get; }

        private readonly ITrainPath _trainPath;
        private const float PerpendicularLineLength = 8;
        private const float PerpendicularLineThickness = 2;

        public PerpendicularIntersectionsTrainPath(ITrainPath trainPath) {

            _trainPath = trainPath;

            BackUpPath = new SKPath();
            LineIntersection = new SKPath();
            LineIntersection.AddRect(SKRect.Create(0, - PerpendicularLineLength / 2, PerpendicularLineThickness, PerpendicularLineLength));

            LineIntersectionPaint = new SKPaint {
                Color = PathColor,
                IsAntialias = true,
                PathEffect = SKPathEffect.Create1DPath(LineIntersection, 40, 10, SKPath1DPathEffectStyle.Rotate)
            };
        }

        public float MeasurePathStrokeWidth() {
            return Math.Max(PerpendicularLineLength, _trainPath.MeasurePathStrokeWidth());
        }

        public float DistanceFromPoint(SKPoint point) {
            return _trainPath.DistanceFromPoint(point);
        }

        public SKColor PathColor {
            get => _trainPath.PathColor;
            set => _trainPath.PathColor = value;
        }

        public LinePaint LinePaint => _trainPath.LinePaint;
        public SKPoint this[int index] => _trainPath[index];

        public int PointCount => _trainPath.PointCount;

        public void Clear() {
            _trainPath.Clear();
        }

        public void Arrange() {
            _trainPath.Arrange();
        }

        public void Update(ImmutableArray<TrainEvent> schedule) {
            _trainPath.Update(schedule);
        }
        
        public void Draw(DrawingCanvas drawingCanvas) {

            _trainPath.Draw(drawingCanvas);

            LineIntersectionPaint.Color = PathColor;

            BackUpPath.Reset();
            var isStart = true;

            foreach (var index in GetNonHorizontalSegments()) {

                if (index == -1) {

                    drawingCanvas.Canvas.DrawPath(BackUpPath, LineIntersectionPaint);
                    BackUpPath.Reset();
                    isStart = true;
                } else if (isStart) {
                    BackUpPath.MoveTo(_trainPath[index]);
                    isStart = false;
                } else {
                    BackUpPath.LineTo(_trainPath[index]);
                }
            }
        }

        public IEnumerable<int> GetNonHorizontalSegments() {

            var pathLength = _trainPath.PointCount;

            if (pathLength == 0) yield break;

            var pathIndex = 0;
            var pathPoint = _trainPath[pathIndex];

            yield return 0;
            ++pathIndex;

            while (pathIndex != pathLength) {

                var currentPoint = _trainPath[pathIndex];

                if (Math.Abs(currentPoint.Y - pathPoint.Y) < 0.001) {
                    yield return -1;
                }

                pathPoint = currentPoint;
                yield return pathIndex;

                ++pathIndex;
            }

            yield return -1;
        }

        public IReadOnlyDictionary<TrainEvent, (int Index, SKPoint PathPoint)> PointsByTrainPathEvents =>
            _trainPath.PointsByTrainPathEvents;

        public IReadOnlyList<TrainEvent> TrainPathEvents =>
            _trainPath.TrainPathEvents;
    }
}
