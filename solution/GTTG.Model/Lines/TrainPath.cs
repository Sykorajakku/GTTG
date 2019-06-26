// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SkiaSharp;

using GTTG.Core.Component;
using GTTG.Core.Drawing.Canvases;
using GTTG.Core.Extensions;
using GTTG.Core.Strategies.Implementations;
using GTTG.Core.Strategies.Interfaces;
using GTTG.Model.Model.Events;
using GTTG.Model.Strategies.Types;

namespace GTTG.Model.Lines {

    /// <summary>
    /// Creates train path from <see cref="TrainEvent"/> of updated schedule.
    /// </summary>
    public class TrainPath : ITrainPath {

        /// <inheritdoc />
        public IReadOnlyDictionary<TrainEvent, (int Index, SKPoint PathPoint)> PointsByTrainPathEvents => _PointsByTrainPathEvents;

        /// <inheritdoc />
        public IReadOnlyList<TrainEvent> TrainPathEvents => _trainPathEvents;

        /// <summary>
        /// View provider to converts <see cref="System.DateTime"/> values to horizontal values.
        /// </summary>
        protected readonly IViewProvider ViewProvider;

        /// <summary>
        /// <see cref="LinePaint"/> to draw <see cref="SkTrainPath"/>.
        /// </summary>
        protected readonly LinePaint TrainLinePaint;

        /// <summary>
        /// <see cref="SKPath"/> of points that forms path of train in <see cref="ContentDrawingCanvas"/>.
        /// </summary>
        protected readonly SKPath SkTrainPath;

        /// <summary>
        /// Provides segments of position of horizontal lines where points <see cref="SkTrainPath"/> are placed.
        /// </summary>
        protected readonly ISegmentRegistry<LineType, MeasureableSegment> SegmentRegistry;

        /// <summary>
        /// Maps <see cref="TrainEvent"/> to index in <see cref="SkTrainPath"/> and it's point.
        /// </summary>
        protected readonly Dictionary<TrainEvent, (int Index, SKPoint PathPoint)> _PointsByTrainPathEvents;

        /// <summary>
        /// <see cref="TrainEvent"/> as schedule to be arranged to form <see cref="SkTrainPath"/>.
        /// </summary>
        protected readonly List<TrainEvent> _trainPathEvents;

        /// <summary>
        /// Keeps track of segments where line was registered for measure.
        /// </summary>
        protected readonly List<MeasureableSegment> SegmentRegistrations;

        /// <summary>
        /// Creates empty train path.
        /// </summary>
        /// <param name="viewProvider">Converter of date time event values to horizontal positions.</param>
        /// <param name="segmentRegistry">Registry of lines providing it's vertical position.</param>
        /// <param name="linePaint">Line to create path from.</param>
        public TrainPath(IViewProvider viewProvider, ISegmentRegistry<LineType, MeasureableSegment> segmentRegistry, LinePaint linePaint) {

            ViewProvider = viewProvider;
            SegmentRegistry = segmentRegistry;
            TrainLinePaint = linePaint;

            SkTrainPath = new SKPath();
            SegmentRegistrations = new List<MeasureableSegment>();
            _PointsByTrainPathEvents = new Dictionary<TrainEvent, (int index, SKPoint pathPoint)>();
            _trainPathEvents = new List<TrainEvent>();
        }

        /// <inheritdoc />
        public int PointCount => SkTrainPath.PointCount;

        /// <inheritdoc />
        public LinePaint LinePaint => TrainLinePaint;

        /// <inheritdoc />
        public SKPoint this[int index] => SkTrainPath[index];

        /// <inheritdoc />
        public void Clear() {

            SkTrainPath.Reset();
            _PointsByTrainPathEvents.Clear();
            _trainPathEvents.Clear();

            foreach (var segment in SegmentRegistrations) {
                segment.HeightMeasureHelpers -= MeasurePathStrokeWidth;
            }

            SegmentRegistrations.Clear();
        }

        /// <inheritdoc />
        public void Arrange() {

            if (_trainPathEvents.Count == 0) return;
            SkTrainPath.Reset();

            var trainPathEvent = _trainPathEvents[0];
            var trainPathEventPoint = ComputePathPoint(trainPathEvent);
            _PointsByTrainPathEvents[trainPathEvent] = (0, trainPathEventPoint);
            SkTrainPath.MoveTo(trainPathEventPoint);

            for (var i = 1; i < _trainPathEvents.Count; ++i) {

                trainPathEvent = _trainPathEvents[i];
                trainPathEventPoint = ComputePathPoint(trainPathEvent);
                _PointsByTrainPathEvents[trainPathEvent] = (i, trainPathEventPoint);
                SkTrainPath.LineTo(trainPathEventPoint);
            }

            TrainLinePaint.Arrange(TrainLinePaint.DesiredStrokeWidth);
            foreach (var segment in SegmentRegistrations) {

                if (segment.SegmentContentHeight < TrainLinePaint.ArrangedStrokeWidth) {
                    TrainLinePaint.Arrange(segment.SegmentContentHeight);
                }
            }
        }

        /// <inheritdoc />
        public void Update(ImmutableArray<TrainEvent> schedule) {

            Clear();

            var scheduleIndex = -1;
            var trainPathEventIndex = 0;
            var scheduleEventsCount = schedule.Length;

            scheduleIndex = IterateToNextMovementEvent(scheduleIndex, schedule);
            if (scheduleIndex == scheduleEventsCount) return;

            var firstTrainPathEvent = schedule[scheduleIndex];
            var firstEventPathPoint = ComputePathPoint(firstTrainPathEvent);

            _trainPathEvents.Add(firstTrainPathEvent);
            _PointsByTrainPathEvents.Add(firstTrainPathEvent, (trainPathEventIndex, firstEventPathPoint));
            SkTrainPath.MoveTo(firstEventPathPoint);
            trainPathEventIndex++;

            scheduleIndex = IterateToNextMovementEvent(scheduleIndex, schedule);
            while (scheduleIndex != scheduleEventsCount) {

                var trainPathEvent = schedule[scheduleIndex];
                var trainPathEventPoint = ComputePathPoint(trainPathEvent);

                _trainPathEvents.Add(trainPathEvent);
                _PointsByTrainPathEvents.Add(trainPathEvent, (trainPathEventIndex, trainPathEventPoint));
                SkTrainPath.LineTo(trainPathEventPoint);
                trainPathEventIndex++;

                scheduleIndex = IterateToNextMovementEvent(scheduleIndex, schedule);
            }

            foreach (var track in schedule.Select(t => t.Track).Distinct()) {

                var segment = SegmentRegistry.Resolve(LineType.Of(track));
                segment.HeightMeasureHelpers += MeasurePathStrokeWidth;
                SegmentRegistrations.Add(segment);
            }
        }

        private int IterateToNextMovementEvent(int startIndex, ImmutableArray<TrainEvent> schedule) {

            var scheduleEventsCount = schedule.Length;
            var scheduleIndex = startIndex;
            scheduleIndex++;

            while (scheduleEventsCount < scheduleIndex) {
                ++scheduleIndex;
            }

            return scheduleIndex;
        }

        private SKPoint ComputePathPoint(TrainEvent trainEvent) {

            var x = ViewProvider.GetContentHorizontalPosition(trainEvent.DateTime);
            var y = SegmentRegistry.Resolve(LineType.Of(trainEvent.Track)).SegmentContentMiddle;
            return new SKPoint(x, y);
        }

        /// <inheritdoc />
        public virtual void Draw(DrawingCanvas drawingCanvas) {
            drawingCanvas.Canvas.DrawPath(SkTrainPath, TrainLinePaint.Paint);
        }

        /// <inheritdoc />
        public float MeasurePathStrokeWidth() {
            return TrainLinePaint.ArrangedStrokeWidth;
        }

        /// <inheritdoc />
        public float DistanceFromPoint(SKPoint point) {
            return SkTrainPath.CalculateDistanceFromPath(point);
        }

        /// <inheritdoc />
        public SKColor PathColor {
            get => TrainLinePaint.Paint.Color;
            set => TrainLinePaint.Paint.Color = value;
        }
    }
}
