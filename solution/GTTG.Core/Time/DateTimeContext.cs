// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace GTTG.Core.Time {

    /// <summary>
    /// <see cref="DateTimeInterval"/> intervals as representation of <see cref="Drawing.Canvases.ContentDrawingCanvas"/> and <see cref="Drawing.Canvases.ViewDrawingCanvas"/> scope.
    /// </summary>
    public class DateTimeContext {

        /// <summary>
        /// Time scope of data available to display in <see cref="Drawing.Canvases.ContentDrawingCanvas"/>. Contains <see cref="ContentDateTimeInterval"/>.
        /// </summary>
        public DateTimeInterval ContentDateTimeInterval { get; }

        /// <summary>
        /// Time scope of view, the content being displayed in <see cref="Drawing.Canvases.ViewDrawingCanvas"/>.
        /// </summary>
        public DateTimeInterval ViewDateTimeInterval { get; }

        /// <summary>Constructs <see cref="DateTimeContext"/>.</summary>
        /// <param name="contentDateTimeInterval">Value representing <see cref="ContentDateTimeInterval"/>.</param>
        /// <param name="viewDateTimeInterval">Value representing <see cref="ViewDateTimeInterval"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">Interval <paramref name="contentDateTimeInterval"/> does not contain <paramref name="viewDateTimeInterval"/>.</exception>
        public DateTimeContext(DateTimeInterval contentDateTimeInterval, DateTimeInterval viewDateTimeInterval) {

            if (!contentDateTimeInterval.Contains(viewDateTimeInterval)) {
                throw new ArgumentOutOfRangeException($"Interval {nameof(contentDateTimeInterval)} {contentDateTimeInterval} does not contain {nameof(viewDateTimeInterval)} {viewDateTimeInterval}");
            }

            ViewDateTimeInterval = viewDateTimeInterval;
            ContentDateTimeInterval = contentDateTimeInterval;
        }
    }
}
