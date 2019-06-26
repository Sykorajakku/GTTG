// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace GTTG.Model.Strategies.Types {

    /// <summary>
    /// Determines placement by vertical position above or below some object.
    /// </summary>
    public struct SegmentType<T> : IEquatable<SegmentType<T>> {

        /// <summary>
        /// Instance of object above or below which is placement determined. 
        /// </summary>
        public T Type { get; }

        /// <summary>
        /// Vertical position of placement above or below some object.
        /// </summary>
        public SegmentPlacement SegmentPlacement { get; }

        /// <summary>
        /// Creates placement determined by vertical position.
        /// </summary>
        /// <param name="type">Type of object above or below is placement determined</param>
        /// <param name="segmentPlacement">Type of vertical placement</param>
        public SegmentType(T type, SegmentPlacement segmentPlacement) {
            Type = type;
            SegmentPlacement = segmentPlacement;
        }

        /// <inheritdoc/>
        public bool Equals(SegmentType<T> other) => other.SegmentPlacement == SegmentPlacement && other.Type.Equals(Type);

        /// <inheritdoc/>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            return obj is SegmentType<T> other && Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode() {
            return (Type != null ? Type.GetHashCode() : 0);
        }
    }
}
