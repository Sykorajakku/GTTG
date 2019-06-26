// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Strategies.Implementations;
using GTTG.Core.Strategies.Interfaces;
using GTTG.Core.Utils;
using GTTG.Model.Lines;
using GTTG.Model.Model.Infrastructure;
using GTTG.Model.Strategies.Converters;
using GTTG.Model.Strategies.Types;

namespace GTTG.Model.Strategies.Dockers {

    /// <summary>
    /// Docks elements into segments of <see cref="SegmentType{Station}"/> type.
    /// Element is placed on middle of line segment of intersection of train path and particular segment.
    /// </summary>
    public class StationStrategyDocker<TElement> : IStrategyDocker
        where TElement : ViewElement {

        private readonly StrategyManager<TrainEventPlacement, TElement, SegmentType<Station>, MeasureableSegment> _strategyManager;
        private readonly TrainEventPlacementConverter _trainEventPlacementConverter;
        private readonly ITrainPath _trainPath;

        /// <summary>Creates docker for particular train path.</summary>
        /// <param name="trainPath">Train path on which elements are positioned.</param>
        /// <param name="trainEventPlacementConverter">Converter from which segments are received.</param>
        /// <param name="strategyManager">Manager with elements to position.</param>
        public StationStrategyDocker(ITrainPath trainPath, 
                                     TrainEventPlacementConverter trainEventPlacementConverter,
                                     StrategyManager<TrainEventPlacement, TElement, SegmentType<Station>, MeasureableSegment> strategyManager) {

            _trainEventPlacementConverter = trainEventPlacementConverter;
            _trainPath = trainPath;
            _strategyManager = strategyManager;
        }

        /// <inheritdoc/>
        public void Dock() {

            foreach (var containerRegistration in _strategyManager) {

                var container = _strategyManager[containerRegistration.Key];
                var segment = _strategyManager.ManagedSegments[containerRegistration.Key];
                var anglePlacement = containerRegistration.Key.AnglePlacement;
                var segmentPlacement = _strategyManager.ManagedSegmentTypes[containerRegistration.Key].SegmentPlacement;

                var (segmentBase, vectorFromBase) =
                    _trainEventPlacementConverter.ComputeVectorFromEvent(containerRegistration.Key.TrainEvent);

                var upperIntersection =
                    PlacementUtils.ComputeHorizontalLineIntersection(vectorFromBase, segmentBase, segment.ContentUpperBoundPosition);
                var lowerIntersection =
                    PlacementUtils.ComputeHorizontalLineIntersection(vectorFromBase, segmentBase, segment.ContentLowerBoundPosition);

                ScaleToFitSegment(container, segment, lowerIntersection - upperIntersection);

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
        /// Docks element in upper segment to acute angle.
        /// </summary>
        /// <param name="element">Element to dock.</param>
        /// <param name="lowerBoundOrigin">Point on lower horizontal line of segment.</param>
        /// <param name="vectorToUpperBound">Train path segment from <paramref name="lowerBoundOrigin"/> to upper horizontal line of segment.</param>
        protected void DockLowerAcute(TElement element, SKPoint lowerBoundOrigin, SKPoint vectorToUpperBound) {

            var trainPathThickness = _trainPath.MeasurePathStrokeWidth() / 2;

            var trainLineOffset = PlacementUtils.ComputeHypotenuseLength(vectorToUpperBound, LayoutConstants.HorizontalLineVector, trainPathThickness);
            var horizontalTranslation = vectorToUpperBound.X > 0 ? trainLineOffset : -trainLineOffset;

            var legLength = PlacementUtils.ComputeLegLength(vectorToUpperBound, LayoutConstants.HorizontalLineVector, element.ContentHeight);
            var segmentVectorTranslationLength = 2 * legLength + element.ContentWidth < vectorToUpperBound.Length ?
                (vectorToUpperBound.Length - element.ContentWidth) / 2 : legLength;
            if (vectorToUpperBound.X < 0) segmentVectorTranslationLength += element.ContentWidth;

            var segmentVectorTranslation = PlacementUtils.MoveInLine(vectorToUpperBound, lowerBoundOrigin, segmentVectorTranslationLength);

            var containerOrigin = new SKPoint {
                X = segmentVectorTranslation.X + horizontalTranslation,
                Y = segmentVectorTranslation.Y
            };

            var radAngle = PlacementUtils.ComputeAcuteRadAngle(vectorToUpperBound, LayoutConstants.HorizontalLineVector);
            var rotation = vectorToUpperBound.X > 0 ? 2 * Math.PI - radAngle : radAngle;

            element.Reposition(containerOrigin);
            element.Rotate((float)rotation);
        }


        /// <summary>
        /// Docks element in lower segment to acute angle.
        /// </summary>
        /// <param name="element">Element to dock.</param>
        /// <param name="upperBoundOrigin">Intersection of upper horizontal line of segment and train path.</param>
        /// <param name="vectorToLowerBound">Train path segment from <paramref name="upperBoundOrigin"/> to lower horizontal line of segment.</param>
        protected void DockUpperAcute(TElement element, SKPoint upperBoundOrigin, SKPoint vectorToLowerBound) {

            var trainPathThickness = _trainPath.MeasurePathStrokeWidth() / 2;

            var horizontalAcuteOffset = PlacementUtils.ComputeHypotenuseLength(vectorToLowerBound, LayoutConstants.HorizontalLineVector, element.ContentHeight);
            var trainLineOffset = PlacementUtils.ComputeHypotenuseLength(vectorToLowerBound, LayoutConstants.HorizontalLineVector, trainPathThickness);
            var horizontalTranslation = vectorToLowerBound.X < 0 ? -trainLineOffset : trainLineOffset;
            horizontalTranslation += vectorToLowerBound.X < 0 ? -horizontalAcuteOffset : horizontalAcuteOffset;

            var legLength = PlacementUtils.ComputeLegLength(vectorToLowerBound, LayoutConstants.HorizontalLineVector, element.ContentHeight);
            var segmentVectorTranslationLength = 2 * legLength + element.ContentWidth < vectorToLowerBound.Length
                ? (vectorToLowerBound.Length - element.ContentWidth) / 2 : 0;
            if (vectorToLowerBound.X < 0) segmentVectorTranslationLength += element.ContentWidth;

            var segmentVectorTranslation = PlacementUtils.MoveInLine(vectorToLowerBound, upperBoundOrigin, segmentVectorTranslationLength);

            var containerOrigin = new SKPoint {
                X = segmentVectorTranslation.X + horizontalTranslation,
                Y = segmentVectorTranslation.Y
            };

            var radAngle = PlacementUtils.ComputeAcuteRadAngle(vectorToLowerBound, LayoutConstants.HorizontalLineVector);
            var rotation = vectorToLowerBound.X < 0 ? 2 * Math.PI - radAngle : radAngle;

            element.Reposition(containerOrigin);
            element.Rotate((float)rotation);
        }


        /// <summary>
        /// Docks element in upper segment to acute angle.
        /// </summary>
        /// <param name="element">Element to dock.</param>
        /// <param name="upperBoundOrigin">Intersection of upper horizontal line of segment and train path.</param>
        /// <param name="vectorToLowerBound">Train path segment from <paramref name="upperBoundOrigin"/> to lower horizontal line of segment.</param>
        protected void DockLowerObtuse(TElement element, SKPoint upperBoundOrigin, SKPoint vectorToLowerBound) {

            var trainPathThickness = _trainPath.MeasurePathStrokeWidth() / 2;

            var horizontalAcuteOffset = PlacementUtils.ComputeHypotenuseLength(vectorToLowerBound, LayoutConstants.HorizontalLineVector, element.ContentHeight);
            var trainLineOffset = PlacementUtils.ComputeHypotenuseLength(vectorToLowerBound, LayoutConstants.HorizontalLineVector, trainPathThickness);
            var horizontalTranslation = vectorToLowerBound.X > 0 ? -trainLineOffset : +trainLineOffset;
            horizontalTranslation += vectorToLowerBound.X > 0 ? -horizontalAcuteOffset : horizontalAcuteOffset;

            var legLength = PlacementUtils.ComputeLegLength(vectorToLowerBound, LayoutConstants.HorizontalLineVector, element.ContentHeight);
            var segmentVectorTranslationLength = 2 * legLength + element.ContentWidth < vectorToLowerBound.Length ?
                (vectorToLowerBound.Length - element.ContentWidth) / 2 : legLength;
            if (vectorToLowerBound.X < 0) segmentVectorTranslationLength += element.ContentWidth;

            var segmentVectorTranslation = PlacementUtils.MoveInLine(vectorToLowerBound, upperBoundOrigin, segmentVectorTranslationLength);

            var containerOrigin = new SKPoint {
                X = segmentVectorTranslation.X + horizontalTranslation,
                Y = segmentVectorTranslation.Y
            };

            var radAngle = PlacementUtils.ComputeAcuteRadAngle(vectorToLowerBound, LayoutConstants.HorizontalLineVector);
            var rotation = vectorToLowerBound.X > 0 ? 2 * Math.PI - radAngle : radAngle;

            element.Reposition(containerOrigin);
            element.Rotate((float)rotation);
        }

        /// <summary>
        /// Docks element in lower segment to acute angle.
        /// </summary>
        /// <param name="element">Element to dock.</param>
        /// <param name="upperBoundOrigin">Intersection of upper horizontal line of segment and train path.</param>
        /// <param name="vectorToLowerBound">Train path segment from <paramref name="upperBoundOrigin"/> to lower horizontal line of segment.</param>
        protected void DockUpperObtuse(TElement element, SKPoint upperBoundOrigin, SKPoint vectorToLowerBound) {

            var trainPathThickness = _trainPath.MeasurePathStrokeWidth() / 2;
            var trainLineOffset = PlacementUtils.ComputeHypotenuseLength(vectorToLowerBound, LayoutConstants.HorizontalLineVector, trainPathThickness);
            var horizontalTranslation = vectorToLowerBound.X < 0 ? -trainLineOffset : trainLineOffset;

            var legLength = PlacementUtils.ComputeLegLength(vectorToLowerBound, LayoutConstants.HorizontalLineVector, element.ContentHeight);
            var segmentVectorTranslationLength = 2 * legLength + element.ContentWidth < vectorToLowerBound.Length
                ? (vectorToLowerBound.Length - element.ContentWidth) / 2 : 0;
            if (vectorToLowerBound.X < 0) segmentVectorTranslationLength += element.ContentWidth;

            var segmentVectorTranslation = PlacementUtils.MoveInLine(vectorToLowerBound, upperBoundOrigin, segmentVectorTranslationLength);

            var containerOrigin = new SKPoint {
                X = segmentVectorTranslation.X + horizontalTranslation,
                Y = segmentVectorTranslation.Y
            };

            var radAngle = PlacementUtils.ComputeAcuteRadAngle(vectorToLowerBound, LayoutConstants.HorizontalLineVector);
            var rotation = vectorToLowerBound.X < 0 ? 2 * Math.PI - radAngle : radAngle;

            element.Reposition(containerOrigin);
            element.Rotate((float)rotation);
        }

        /// <summary>
        /// Arranges element with it's desired size and scales if does not match segment height.
        /// </summary>
        /// <param name="element">Element to measure and scale.</param>
        /// <param name="segment">Segment where element is placed.</param>
        /// <param name="segmentVector">Line segment of train path intersection the segment.</param>
        protected virtual void ScaleToFitSegment(TElement element, ISegment segment, SKPoint segmentVector) {

            // compute length of transition of element in segment vector + width of vector 
            var currentContainerSegmentSize = element.ContentWidth + PlacementUtils.ComputeLegLength(segmentVector, LayoutConstants.HorizontalLineVector, element.ContentHeight);
            
            if (segmentVector.Length < currentContainerSegmentSize) {

                // acute angle of vector and horizontal line
                var angle = PlacementUtils.ComputeAcuteRadAngle(segmentVector, LayoutConstants.HorizontalLineVector);


                var scale = (segmentVector.Length / (1 / Math.Sin(angle) + element.ContentWidth / element.ContentHeight)) /
                            element.ContentHeight;

                element.Scale((float)scale);
            }
        }

        /// <summary>
        /// Measures height of element after being positioned by strategy as elements could be rotated.
        /// </summary>
        public float MeasureHeight(TrainEventPlacement placementPlacement, TElement element, SegmentType<Station> segmentType, ISegment segment) {
            element.Measure(LayoutConstants.InfiniteSize);
            return PlacementUtils.ComputeDiagonal(element.DesiredSize);
        }
    }
}
