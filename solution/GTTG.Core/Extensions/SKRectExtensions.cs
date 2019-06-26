// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using SkiaSharp;

namespace GTTG.Core.Extensions {

    /// <summary>
    /// Extension methods for <see cref="SKRect"/>.
    /// </summary>
    public static class SkRectExtensions {

        /// <summary>Determines if rectangle contains another rectangle.</summary>
        /// <param name="containing">Rectangle that should contain <paramref name="inside"/> rectangle.</param>
        /// <param name="inside">Rectangle that should be in <paramref name="containing"/> rectangle.</param>
        /// <param name="comparisonDelta">Tolerance of floating point subtraction.</param>
        public static bool ContainsWithDelta(this SKRect containing, SKRect inside, float comparisonDelta = 0.001f) {
            
            // "correct" lines, if less than delta, consider the same
            var deltaModifiedRect = inside;
            if (containing.Left - inside.Left < comparisonDelta) deltaModifiedRect.Left = containing.Left;
            if (containing.Top - inside.Top < comparisonDelta) deltaModifiedRect.Top = containing.Top;
            if (inside.Right - containing.Right < comparisonDelta) deltaModifiedRect.Right = containing.Right;
            if (inside.Bottom - containing.Bottom < comparisonDelta) deltaModifiedRect.Bottom = containing.Bottom;

            return containing.Contains(deltaModifiedRect);
        }
    }
}
