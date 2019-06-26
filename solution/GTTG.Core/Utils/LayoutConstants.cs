// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using SkiaSharp;

namespace GTTG.Core.Utils {
    
    /// <summary>
    /// Constant values used for layout cycle.
    /// </summary>
    public static class LayoutConstants {
        
        /// <summary>
        /// Vector of horizontal line.
        /// </summary>
        public static readonly SKPoint HorizontalLineVector = new SKPoint(1, 0);

        /// <summary>
        /// <see cref="SKSize"/> with <see cref="float.PositiveInfinity"/> values.
        /// </summary>
        public static readonly SKSize InfiniteSize = new SKSize(float.PositiveInfinity, float.PositiveInfinity);
    }
}
