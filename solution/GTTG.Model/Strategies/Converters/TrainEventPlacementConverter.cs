// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using SkiaSharp;

using GTTG.Core.Strategies.Interfaces;
using GTTG.Model.Lines;
using GTTG.Model.Model.Events;
using GTTG.Model.Model.Infrastructure;
using GTTG.Model.Strategies.Types;

namespace GTTG.Model.Strategies.Converters {
    
    /// <summary>
    /// Converts <see cref="TrainEventPlacement"/> to <see cref="SegmentType{T}"/>
    /// and provides vector from <see cref="TrainEvent"/> point to neighbor <see cref="TrainEvent"/> in provided train path.
    /// </summary>
    public class TrainEventPlacementConverter : ITypeConverter<TrainEventPlacement, SegmentType<Track>>,
                                                ITypeConverter<TrainEventPlacement, SegmentType<Station>> {

        private readonly ITrainPath _trainPath;

        /// <summary>Creates convertor for provided train path.</summary>
        public TrainEventPlacementConverter(ITrainPath trainPath) {
            _trainPath = trainPath;
        }
        
        private SKPoint GetVerticallyDifferentTrainPathPoint(bool isAscending, int pathIndex, SKPoint p) {

            var events = _trainPath.TrainPathEvents;
            var points = _trainPath.PointsByTrainPathEvents;
            var index = pathIndex;

            if (isAscending) {

                index++;
                while (index < events.Count && Math.Abs(points[events[index]].PathPoint.Y - p.Y) < 0.001) {
                    index++;
                }
            }
            else {

                --index;
                while (index >= 0 && Math.Abs(points[events[index]].PathPoint.Y - p.Y) < 0.001) {
                    --index;
                }
            }

            if (index == -1 || index == events.Count) {
                return p;
            }

            return points[events[index]].PathPoint;
        }

        private SegmentPlacement DetermineSegmentPlacement(TrainEvent trainMovementEvent) {

            var originEvent = _trainPath.PointsByTrainPathEvents[trainMovementEvent];
            var vector = DetermineEventVector(trainMovementEvent);

            if (Math.Abs(vector.Y - originEvent.PathPoint.Y) < 0.001) {
                throw new ArgumentException($"Unable to determine segment placement for {trainMovementEvent}.");
            }

            return vector.Y >= 0 ? SegmentPlacement.Lower : SegmentPlacement.Upper;
        }

        private SKPoint DetermineEventVector(TrainEvent trainMovementEvent) {

            var trainPathEventsCount = _trainPath.TrainPathEvents.Count;
            AssertValidContainerType(trainMovementEvent);

            var (eventPathIndex, eventPathPoint) = _trainPath.PointsByTrainPathEvents[trainMovementEvent];

            var adjacentTrainMovementEventIndex = trainMovementEvent.TrainEventType == TrainEventType.Arrival ?
                eventPathIndex - 1 :
                eventPathIndex + 1;

            SKPoint pathVector;
            SKPoint adjacentPoint;

            if (adjacentTrainMovementEventIndex == -1) {
                adjacentPoint = GetVerticallyDifferentTrainPathPoint(true, 0, eventPathPoint);
                pathVector = eventPathPoint - adjacentPoint;
            } else if (adjacentTrainMovementEventIndex == trainPathEventsCount) {
                adjacentPoint = GetVerticallyDifferentTrainPathPoint(false, trainPathEventsCount, eventPathPoint);
                pathVector = eventPathPoint - adjacentPoint;
            } else {

                var isAscending = adjacentTrainMovementEventIndex > eventPathIndex;
                adjacentPoint = GetVerticallyDifferentTrainPathPoint(isAscending, eventPathIndex, eventPathPoint);
                pathVector = adjacentPoint - eventPathPoint;
            }

            return pathVector;
        }

        /// <summary>
        /// Returns vector from point representing event to neighbor event in path direction depending on event type.
        /// If first or last event in schedule provided, returns vector in opposite direction multiplied by (-1,-1).
        /// </summary>
        /// <returns>
        /// SegmentBase -- point in path <paramref name="trainEvent"/> that represents <paramref name="trainEvent"/>.
        /// VectorFromBase -- vector in direction to other point depending on <see cref="TrainEvent.TrainEventType"/>.
        /// For departure and passage picks points of next events in schedule.
        /// For arrival picks points of previous events to schedule.
        /// </returns>
        /// <exception cref="ArgumentException">If <paramref name="trainEvent"/> conversion can't be determined.</exception>
        public (SKPoint SegmentBase, SKPoint VectorFromBase) ComputeVectorFromEvent(TrainEvent trainEvent) {

            var vector = DetermineEventVector(trainEvent);
            var eventPoint = _trainPath.PointsByTrainPathEvents[trainEvent].PathPoint;

            return (eventPoint, vector);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException">If <paramref name="eventPlacement"/> conversion can't be determined.</exception>
        SegmentType<Track> ITypeConverter<TrainEventPlacement, SegmentType<Track>>.Convert(TrainEventPlacement eventPlacement) {
            var placement = DetermineSegmentPlacement(eventPlacement.TrainEvent);
            return new SegmentType<Track>(eventPlacement.TrainEvent.Track, placement);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException">If <paramref name="eventPlacement"/> conversion can't be determined.</exception>
        SegmentType<Station> ITypeConverter<TrainEventPlacement, SegmentType<Station>>.Convert(TrainEventPlacement eventPlacement) {
            var placement = DetermineSegmentPlacement(eventPlacement.TrainEvent);
            return new SegmentType<Station>(eventPlacement.TrainEvent.Station, placement);
        }

        private void AssertValidContainerType(TrainEvent trainMovementEvent) {

            if (!_trainPath.PointsByTrainPathEvents.ContainsKey(trainMovementEvent)) {
                throw new ArgumentException($"Event {trainMovementEvent} can't be converted to segment type, event is not found in train path.");
            }
        }
    }
}
