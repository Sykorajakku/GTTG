// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Strategies.Implementations;
using GTTG.Core.Strategies.Interfaces;
using GTTG.Core.Utils;
using GTTG.Model.Lines;
using GTTG.Model.Model.Infrastructure;
using GTTG.Model.Strategies.Converters;
using GTTG.Model.Strategies.Types;

namespace GTTG.Model.Strategies.Dockers  {

    /// <summary>
    /// Docks elements into segments of <see cref="SegmentType{Track}"/> type.
    /// Element is placed on horizontal line of segment depending on it's type nearby train path.
    /// </summary>
    public class TracksStrategyDocker<TElement> : IStrategyDocker
        where TElement : ViewElement {

        private readonly TrainEventPlacementConverter _trainEventPlacementConverter;
        private readonly ITrainPath _trainPath;
        private readonly StrategyManager<TrainEventPlacement, TElement, SegmentType<Track>, MeasureableSegment> _strategyManager;

        /// <summary>Creates docker for particular train path.</summary>
        /// <param name="trainPath">Train path nearby which elements are positioned.</param>
        /// <param name="trainEventPlacementConverter">Converter from which segments are received.</param>
        /// <param name="strategyManager">Manager with elements to position.</param>
        public TracksStrategyDocker(ITrainPath trainPath,
                                    TrainEventPlacementConverter trainEventPlacementConverter,
                                    StrategyManager<TrainEventPlacement, TElement, SegmentType<Track>, MeasureableSegment> strategyManager)  {

            _trainEventPlacementConverter = trainEventPlacementConverter;
            _trainPath = trainPath;
            _strategyManager = strategyManager;
        }

        /// <inheritdoc/>
        public void Dock() {

            foreach (var containerRegistration in _strategyManager.ManagedSegments) {

                var container = _strategyManager[containerRegistration.Key];
                var segment = containerRegistration.Value;
                var anglePlacement = containerRegistration.Key.AnglePlacement;
                var segmentPlacement = _strategyManager.ManagedSegmentTypes[containerRegistration.Key].SegmentPlacement;

                var (segmentBase, vectorFromBase) = _trainEventPlacementConverter.ComputeVectorFromEvent(containerRegistration.Key.TrainEvent);

                var upperIntersection =
                    PlacementUtils.ComputeHorizontalLineIntersection(vectorFromBase, segmentBase, segment.ContentUpperBoundPosition);
                var lowerIntersection =
                    PlacementUtils.ComputeHorizontalLineIntersection(vectorFromBase, segmentBase, segment.ContentLowerBoundPosition);

                ResizeContainerToFitSegment(container, segment);

                if (anglePlacement == AnglePlacement.Acute && segmentPlacement == SegmentPlacement.Lower) {
                    DockUpperAcute(container, upperIntersection, lowerIntersection - upperIntersection);
                }

                if (anglePlacement == AnglePlacement.Acute && segmentPlacement == SegmentPlacement.Upper) {
                    DockLowerAcute(container, lowerIntersection, upperIntersection - lowerIntersection);
                }

                if (anglePlacement == AnglePlacement.Obtuse && segmentPlacement == SegmentPlacement.Lower) {
                    DockUpperObtuse(container, upperIntersection, lowerIntersection - upperIntersection);
                }

                if (anglePlacement == AnglePlacement.Obtuse && segmentPlacement == SegmentPlacement.Upper) {
                    DockLowerObtuse(container, lowerIntersection, upperIntersection - lowerIntersection);
                }
            }
        }

        /// <summary>
        /// Docks element in lower segment to acute angle.
        /// </summary>
        /// <param name="element">Element to dock.</param>
        /// <param name="upperBoundOrigin">Intersection of upper horizontal line of segment and train path.</param>
        /// <param name="vectorToLowerBound">Train path segment from <paramref name="upperBoundOrigin"/> to lower horizontal line of segment.</param>
        protected virtual void DockUpperAcute(TElement element, SKPoint upperBoundOrigin, SKPoint vectorToLowerBound) {

            var trainPathThickness = _trainPath.MeasurePathStrokeWidth() / 2;

            var pathLineOffset = PlacementUtils.ComputeHypotenuseLength(vectorToLowerBound, LayoutConstants.HorizontalLineVector, trainPathThickness);
            var acuteOffset = PlacementUtils.ComputeLegLength(vectorToLowerBound, LayoutConstants.HorizontalLineVector, element.ContentHeight);

            var transitions = new SKPoint {
                X = vectorToLowerBound.X < 0 ? -(pathLineOffset + acuteOffset + element.ContentWidth) : pathLineOffset + acuteOffset
            };

            element.Reposition(upperBoundOrigin + transitions);
        }

        /// <summary>
        /// Docks element in upper segment to acute angle.
        /// </summary>
        /// <param name="element">Element to dock.</param>
        /// <param name="lowerBoundOrigin">Point on lower horizontal line of segment.</param>
        /// <param name="vectorToUpperBound">Train path segment from <paramref name="lowerBoundOrigin"/> to upper horizontal line of segment.</param>
        protected virtual void DockLowerAcute(TElement element, SKPoint lowerBoundOrigin, SKPoint vectorToUpperBound) {

            var trainPathThickness = _trainPath.MeasurePathStrokeWidth() / 2;

            var pathLineOffset = PlacementUtils.ComputeHypotenuseLength(vectorToUpperBound, LayoutConstants.HorizontalLineVector, trainPathThickness);
            var acuteOffset = PlacementUtils.ComputeLegLength(vectorToUpperBound, LayoutConstants.HorizontalLineVector, element.ContentHeight);

            var transitions = new SKPoint {
                X = vectorToUpperBound.X < 0 ? -(pathLineOffset + acuteOffset + element.ContentWidth) : pathLineOffset + acuteOffset,
                Y = -element.ContentHeight
            };

            element.Reposition(lowerBoundOrigin + transitions);
        }

        /// <summary>
        /// Docks element in lower segment to acute angle.
        /// </summary>
        /// <param name="element">Element to dock.</param>
        /// <param name="upperBoundOrigin">Intersection of upper horizontal line of segment and train path.</param>
        /// <param name="vectorToLowerBound">Train path segment from <paramref name="upperBoundOrigin"/> to lower horizontal line of segment.</param>
        protected virtual void DockUpperObtuse(TElement element, SKPoint upperBoundOrigin, SKPoint vectorToLowerBound) {

            var trainPathThickness = _trainPath.MeasurePathStrokeWidth() / 2;
            var pathLineOffset = PlacementUtils.ComputeHypotenuseLength(upperBoundOrigin, LayoutConstants.HorizontalLineVector, trainPathThickness);

            var transitions = new SKPoint {
                X = vectorToLowerBound.X < 0 ? pathLineOffset : -(pathLineOffset + element.ContentWidth)
            };

            element.Reposition(upperBoundOrigin + transitions);
        }

        /// <summary>
        /// Docks element in upper segment to acute angle.
        /// </summary>
        /// <param name="element">Element to dock.</param>
        /// <param name="lowerBoundOrigin">Point on lower horizontal line of segment.</param>
        /// <param name="vectorToUpperBound">Train path segment from <paramref name="lowerBoundOrigin"/> to upper horizontal line of segment.</param>
        protected virtual void DockLowerObtuse(TElement element, SKPoint lowerBoundOrigin, SKPoint vectorToUpperBound) {

            var trainPathThickness = _trainPath.MeasurePathStrokeWidth() / 2;
            var pathLineOffset = PlacementUtils.ComputeHypotenuseLength(vectorToUpperBound, LayoutConstants.HorizontalLineVector, trainPathThickness);

            var transitions = new SKPoint {
                X = vectorToUpperBound.X < 0 ? pathLineOffset : -(pathLineOffset + element.ContentWidth)
            };

            element.Reposition(lowerBoundOrigin + transitions);
        }


        /// <summary>
        /// Arranges element with it's desired size and scales if does not match segment height.
        /// </summary>
        /// <param name="element">Element to measure and scale.</param>
        /// <param name="segment">Segment where element is placed.</param>
        protected virtual void ResizeContainerToFitSegment(TElement element, ISegment segment) {
            element.Measure(LayoutConstants.InfiniteSize);
            element.Arrange(SKPoint.Empty, element.DesiredSize);

            if (element.ContentHeight > segment.SegmentContentHeight) {
                element.Scale(segment.SegmentContentHeight / element.ContentHeight);
            }
        }

        /// <summary>Measures height of element after being positioned by strategy. Returns height of element from arrange.</summary>
        public float MeasureHeight(TrainEventPlacement placementPlacement, TElement element, SegmentType<Track> segmentType, ISegment segment) {
            element.Measure(LayoutConstants.InfiniteSize);
            return element.DesiredSize.Height;
        }
    }
}
