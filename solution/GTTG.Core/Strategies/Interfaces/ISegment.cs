// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace GTTG.Core.Strategies.Interfaces {

    /// <summary>
    /// Represents segment as horizontal stripe bounded by upper and lower lines in <see cref="Drawing.Canvases.ContentDrawingCanvas"/>.
    /// As <see cref="Drawing.Canvases.ContentDrawingCanvas"/> Y-axis is increasing downwards, <see cref="ContentLowerBoundPosition"/> is always greater
    /// than <see cref="ContentUpperBoundPosition"/>.
    /// </summary>
    public interface ISegment {

        /// <summary>Position of segment's horizontal line bounding it's content from above.</summary>
        float ContentUpperBoundPosition { get; }

        /// <summary>Position of segment's horizontal line bounding it's content from below.</summary>
        float ContentLowerBoundPosition { get; }

        /// <summary>Distance between <see cref="ContentUpperBoundPosition"/> and <see cref="ContentLowerBoundPosition"/>.</summary>
        float SegmentContentHeight { get; }
    }
}
