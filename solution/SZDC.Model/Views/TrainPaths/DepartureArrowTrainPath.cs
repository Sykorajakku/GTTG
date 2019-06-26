using System.Collections.Generic;
using System.Collections.Immutable;
using SkiaSharp;

using GTTG.Core.Drawing.Canvases;
using GTTG.Core.Strategies.Implementations;
using GTTG.Core.Strategies.Interfaces;
using GTTG.Core.Utils;
using GTTG.Model.Lines;
using GTTG.Model.Model.Events;
using GTTG.Model.Model.Infrastructure;
using GTTG.Model.Strategies.Converters;
using GTTG.Model.Strategies.Types;

namespace SZDC.Model.Views.TrainPaths {

    public class DepartureArrowTrainPath : ITrainPath {
        
        public LinePaint LinePaint => TrainPath.LinePaint;
        public SKPoint this[int index] => TrainPath[index];
        public int PointCount => TrainPath.PointCount;

        public SKColor PathColor {
            get => TrainPath.PathColor;
            set => TrainPath.PathColor = value;
        }

        public IReadOnlyDictionary<TrainEvent, (int Index, SKPoint PathPoint)> PointsByTrainPathEvents =>
            TrainPath.PointsByTrainPathEvents;

        public IReadOnlyList<TrainEvent> TrainPathEvents => TrainPath.TrainPathEvents;

        protected ITrainPath TrainPath { get; }

        private readonly ITypeConverter<TrainEventPlacement, SegmentType<Track>> _typeConverter;
        private readonly ISegmentRegistry<SegmentType<Track>, MeasureableSegment> _segmentRegistry;
        private readonly TrainEventPlacementConverter _segmentTypeVectorProvider;
        private readonly ISegmentRegistry<LineType, MeasureableSegment> _linesRegistry;
        private readonly SKPath _departureArrowPath = new SKPath();
        private readonly SKPaint _departureArrowPaint;

        public DepartureArrowTrainPath(ITrainPath trainPath,
                                       ITypeConverter<TrainEventPlacement, SegmentType<Track>> typeConverter,
                                       ISegmentRegistry<SegmentType<Track>, MeasureableSegment> segmentRegistry,
                                       ISegmentRegistry<LineType, MeasureableSegment> linesRegistry,
                                       TrainEventPlacementConverter segmentTypeVectorProvider) {

            TrainPath = trainPath;

            _typeConverter = typeConverter;
            _segmentRegistry = segmentRegistry;
            _linesRegistry = linesRegistry;
            _segmentTypeVectorProvider = segmentTypeVectorProvider;

            _departureArrowPaint = new SKPaint {
                Color = PathColor,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 0.5f,
                StrokeCap = SKStrokeCap.Round,
                IsAntialias = true
            };
        }
        
        public void Clear() {
            TrainPath.Clear();
        }

        public void Arrange() {
            TrainPath.Arrange();
        }

        public void Update(ImmutableArray<TrainEvent> schedule) {
            TrainPath.Update(schedule);
        }

        public void Draw(DrawingCanvas drawingCanvas) {

            TrainPath.Draw(drawingCanvas);
            _departureArrowPaint.Color = PathColor;

            var lastEvent = TrainPath.TrainPathEvents[TrainPath.PointCount - 1];
            if (lastEvent.TrainEventType != TrainEventType.Arrival) {

                var containerMovementType = new TrainEventPlacement(lastEvent, AnglePlacement.Acute);
                var segmentPlacement = _typeConverter.Convert(containerMovementType);
                var segment = _segmentRegistry.Resolve(segmentPlacement);
                var (segmentBase, vector) = _segmentTypeVectorProvider.ComputeVectorFromEvent(containerMovementType.TrainEvent);
                var trackLine = _linesRegistry.Resolve(LineType.Of(lastEvent.Track));

                var upperPoint = PlacementUtils.ComputeHorizontalLineIntersection(vector, segmentBase, vector.Y > 0 ? trackLine.SegmentContentMiddle : segment.ContentUpperBoundPosition);
                var lowerPoint = PlacementUtils.ComputeHorizontalLineIntersection(vector, segmentBase, vector.Y < 0 ? trackLine.SegmentContentMiddle : segment.ContentLowerBoundPosition);

                _departureArrowPath.Reset();
                _departureArrowPath.MoveTo(upperPoint);
                _departureArrowPath.LineTo(lowerPoint);
                drawingCanvas.Canvas.DrawPath(_departureArrowPath, _departureArrowPaint);

                if (vector.Y < 0) {
                    var vct = lowerPoint - upperPoint;
                    var rest = upperPoint + new SKPoint(vct.X / 3, vct.Y / 3);

                    _departureArrowPath.Reset();
                    _departureArrowPath.MoveTo(rest.X - 4, rest.Y);
                    _departureArrowPath.LineTo(upperPoint);
                    _departureArrowPath.LineTo(rest.X + 4, rest.Y);
                } else {

                    var vct = upperPoint - lowerPoint;
                    var rest = lowerPoint + new SKPoint(vct.X / 3, vct.Y / 3);

                    _departureArrowPath.Reset();
                    _departureArrowPath.MoveTo(rest.X - 4, rest.Y);
                    _departureArrowPath.LineTo(lowerPoint);
                    _departureArrowPath.LineTo(rest.X + 4, rest.Y);
                }
                drawingCanvas.Canvas.DrawPath(_departureArrowPath, _departureArrowPaint);
            }
        }

        public float MeasurePathStrokeWidth() {
            return TrainPath.MeasurePathStrokeWidth();
        }

        public float DistanceFromPoint(SKPoint point) {
            return TrainPath.DistanceFromPoint(point);
        }
    }
}
