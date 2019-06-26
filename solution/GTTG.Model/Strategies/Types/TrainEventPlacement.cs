// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

using GTTG.Model.Model.Events;

namespace GTTG.Model.Strategies.Types {

    /// <summary>
    /// Determines placement by event which is represented as point on horizontal line
    /// through which passes intersecting line. Determines placement more accurately by
    /// selecting angle of formed line intersection.
    /// </summary>
    public struct TrainEventPlacement : IEquatable<TrainEventPlacement> {

        /// <summary>
        /// Placement to angle.
        /// </summary>
        public AnglePlacement AnglePlacement { get; }

        /// <summary>
        /// Placement to train event represented as point. 
        /// </summary>
        public TrainEvent TrainEvent { get; }

        /// <summary>
        /// Creates placement determined by event and angle.
        /// </summary>
        /// <param name="trainEvent">Placement to train event.</param>
        /// <param name="anglePlacement">Placement to angle.</param>
        public TrainEventPlacement(TrainEvent trainEvent, AnglePlacement anglePlacement) {
            TrainEvent = trainEvent;
            AnglePlacement = anglePlacement;
        }

        /// <inheritdoc/>
        public bool Equals(TrainEventPlacement other) {
            return AnglePlacement == other.AnglePlacement && other.TrainEvent.Equals(TrainEvent);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            return obj is TrainEventPlacement other && Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode(){
            return (TrainEvent != null ? TrainEvent.GetHashCode() : 0);
        }
    }
}
