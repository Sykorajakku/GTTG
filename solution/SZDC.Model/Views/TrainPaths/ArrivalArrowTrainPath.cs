using SkiaSharp;
using System.Collections.Generic;
using System.Collections.Immutable;

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

    public class ArrivalArrowTrainPath : ITrainPath {

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
        private readonly SKPath _arrivalArrowPath = new SKPath();
        private readonly SKPaint _arrivalArrowPaint;

        public ArrivalArrowTrainPath(ITrainPath trainPath,
                                     ITypeConverter<TrainEventPlacement, SegmentType<Track>> typeConverter,
                                     ISegmentRegistry<SegmentType<Track>, MeasureableSegment> segmentRegistry,
                                     ISegmentRegistry<LineType, MeasureableSegment> linesRegistry,
                                     TrainEventPlacementConverter segmentTypeVectorProvider) { 

            TrainPath = trainPath;

            _typeConverter = typeConverter;
            _segmentRegistry = segmentRegistry;
            _linesRegistry = linesRegistry;
            _segmentTypeVectorProvider = segmentTypeVectorProvider;

            _arrivalArrowPaint = new SKPaint {
                StrokeCap = SKStrokeCap.Round,
                Color = PathColor,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 0.5f,
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
            _arrivalArrowPaint.Color = PathColor;

            var firstEvent = TrainPath.TrainPathEvents[0];
            if (firstEvent.TrainEventType != TrainEventType.Departure) {

                var containerMovementType = new TrainEventPlacement(firstEvent, AnglePlacement.Acute);
                var segmentPlacement = _typeConverter.Convert(containerMovementType);
                var segment = _segmentRegistry.Resolve(segmentPlacement);
                var (segmentBase, vector) = _segmentTypeVectorProvider.ComputeVectorFromEvent(containerMovementType.TrainEvent);

                if (containerMovementType.TrainEvent.TrainEventType == TrainEventType.Passage) {

                    segment = _segmentRegistry.Resolve(new SegmentType<Track>(firstEvent.Track,
                        segmentPlacement.SegmentPlacement == SegmentPlacement.Lower
                            ? SegmentPlacement.Upper
                            : SegmentPlacement.Lower));
                    vector = new SKPoint(- vector.X, - vector.Y);
                }

                var trackLine = _linesRegistry.Resolve(LineType.Of(firstEvent.Track));

                var upperPoint = PlacementUtils.ComputeHorizontalLineIntersection(vector, segmentBase, vector.Y > 0 ? trackLine.SegmentContentMiddle : segment.ContentUpperBoundPosition);
                var lowerPoint = PlacementUtils.ComputeHorizontalLineIntersection(vector, segmentBase, vector.Y < 0 ? trackLine.SegmentContentMiddle : segment.ContentLowerBoundPosition);

                _arrivalArrowPath.Reset();
                _arrivalArrowPath.MoveTo(upperPoint);
                _arrivalArrowPath.LineTo(lowerPoint);                
                drawingCanvas.Canvas.DrawPath(_arrivalArrowPath, _arrivalArrowPaint);

                if (vector.Y < 0) {

                    var vct = lowerPoint - upperPoint;
                    var rest = upperPoint + new SKPoint(vct.X / 3, vct.Y / 3);

                    _arrivalArrowPath.Reset();
                    _arrivalArrowPath.MoveTo(upperPoint.X - 4, upperPoint.Y);
                    _arrivalArrowPath.LineTo(rest);
                    _arrivalArrowPath.LineTo(upperPoint.X + 4, upperPoint.Y);
                }
                else {

                    var vct = upperPoint - lowerPoint;
                    var rest = lowerPoint + new SKPoint(vct.X / 3, vct.Y / 3);

                    _arrivalArrowPath.Reset();
                    _arrivalArrowPath.MoveTo(lowerPoint.X - 4, lowerPoint.Y);
                    _arrivalArrowPath.LineTo(rest);
                    _arrivalArrowPath.LineTo(lowerPoint.X + 4, lowerPoint.Y);
                }

                drawingCanvas.Canvas.DrawPath(_arrivalArrowPath, _arrivalArrowPaint);
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
