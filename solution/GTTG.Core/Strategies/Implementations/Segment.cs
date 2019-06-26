// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Strategies.Interfaces;

namespace GTTG.Core.Strategies.Implementations {

    /// <summary>Represents segment which can be positioned in <see cref="Drawing.Canvases.ContentDrawingCanvas"/> or particular <see cref="ViewElement"/>.</summary>
    public class Segment : ISegment {
        
        /// <inheritdoc />
        public float ContentUpperBoundPosition { get; private set; }

        /// <inheritdoc />
        public float ContentLowerBoundPosition { get; private set; }

        /// <summary>If segment is placed from <see cref="SetBounds(ViewElement,float,float)"/>, this value is in coordinates of <see cref="ViewElement"/>. Otherwise equal to <see cref="ContentUpperBoundPosition"/>.</summary>
        public float LocalUpperBound { get; private set; }

        /// <summary>If segment is placed from <see cref="SetBounds(ViewElement,float,float)"/>, this value is in coordinates of <see cref="ViewElement"/>. Otherwise equal to <see cref="ContentLowerBoundPosition"/>.</summary>
        public float LocalLowerBound { get; private set; }

        /// <summary>If segment is placed from <see cref="SetBounds(ViewElement,float,float)"/>, value is in middle of <see cref="LocalUpperBound"/> and <see cref="LocalLowerBound"/>; otherwise same as <see cref="SegmentContentMiddle"/>.</summary>
        public float SegmentLocalMiddle => LocalUpperBound + SegmentLocalHeight / 2;

        /// <summary>Position of the middle of segment in <see cref="Drawing.Canvases.ContentDrawingCanvas"/>.</summary>
        public float SegmentContentMiddle => ContentUpperBoundPosition + SegmentContentHeight / 2;

        /// <summary>Distance between <see cref="LocalLowerBound"/> and <see cref="LocalUpperBound"/>.</summary>
        public float SegmentLocalHeight => LocalLowerBound - LocalUpperBound;
        
        /// <inheritdoc />
        public float SegmentContentHeight => ContentLowerBoundPosition - ContentUpperBoundPosition;
        
        /// <summary>Places segment in <see cref="Drawing.Canvases.ContentDrawingCanvas"/> from perspective of particular <see cref="ViewElement"/>.</summary>
        /// <param name="viewElement"><see cref="ViewElement"/> from which position of segment is determined.</param>
        /// <param name="localUpperBound">Position in <paramref name="viewElement"/> area translated to <see cref="ContentUpperBoundPosition"/>.</param>
        /// <param name="localLowerBound">Position in <paramref name="viewElement"/> area translated to <see cref="ContentLowerBoundPosition"/>.</param>
        public void SetBounds(ViewElement viewElement, float localUpperBound, float localLowerBound) {

            var upper = viewElement.PlacementMatrix.MapPoint(new SKPoint(0, localUpperBound)).Y;
            var lower = viewElement.PlacementMatrix.MapPoint(new SKPoint(0, localLowerBound)).Y;
            CheckSanity(upper, lower);

            ContentUpperBoundPosition = upper;
            ContentLowerBoundPosition = lower;
            LocalLowerBound = localLowerBound;
            LocalUpperBound = localUpperBound;
        }

        /// <summary>Places segment in <see cref="Drawing.Canvases.ContentDrawingCanvas"/>.</summary>
        /// <param name="globalUpperBound">Value of <see cref="ContentUpperBoundPosition"/>.</param>
        /// <param name="globalLowerBound">Value of <see cref="ContentLowerBoundPosition"/>.</param>
        /// <exception cref="StrategyException">If bounds does not form valid segment; lower is above upper.</exception>
        public void SetBounds(float globalUpperBound, float globalLowerBound) {

            CheckSanity(globalUpperBound, globalLowerBound);
            ContentLowerBoundPosition = globalLowerBound;
            ContentUpperBoundPosition = globalUpperBound;
            LocalLowerBound = globalLowerBound;
            LocalUpperBound = globalUpperBound;
        }

        private void CheckSanity(float upper, float lower) {
            if (upper > lower) throw new StrategyException(
                $"{ nameof(ContentUpperBoundPosition) } (visually) can't be higher than { nameof(ContentLowerBoundPosition) }, y-axis is increasing from visual top to down");
        }
    }
}
