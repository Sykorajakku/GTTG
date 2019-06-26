// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using GTTG.Core.Strategies.Implementations;

namespace GTTG.Core.Strategies.Interfaces {

    /// <summary>Helper structure for fluent syntax registration in <see cref="SegmentRegistry{TSegmentType,TSegment}"/>.</summary>
    public interface ISegmentRegistrationBuilder<in T> {

        /// <summary>Register added segment under type instance of <typeparamref name="T"/>.</summary>
        ISegmentRegistrationBuilder<T> As(T segmentType);
    }
}
