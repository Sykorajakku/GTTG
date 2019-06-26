// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

using GTTG.Model.Model.Infrastructure;

namespace GTTG.Model.Strategies.Types {

    /// <summary>
    /// Represents horizontal line of track.
    /// </summary>
    public struct LineType : IEquatable<LineType> {

        /// <summary>
        /// <see cref="Track"/> to which horizontal line belongs.
        /// </summary>
        public Track Track { get; }

        private LineType(Track track) {
            Track = track;
        }

        /// <summary>
        /// Creates line type of particular track.
        /// </summary>
        public static LineType Of(Track track) {
            return new LineType(track);
        }

        /// <inheritdoc/>
        public bool Equals(LineType other) {
            return other.Track.Equals(Track);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            return obj is TrainEventPlacement other && Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode() {
            return (Track != null ? Track.GetHashCode() : 0);
        }
    }
}
