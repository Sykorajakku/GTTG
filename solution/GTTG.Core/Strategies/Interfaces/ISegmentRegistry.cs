// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace GTTG.Core.Strategies.Interfaces {

    /// <summary>Storage of <see cref="ISegment"/> of type <typeparamref name="TSegmentType"/>.</summary>
    /// <typeparam name="TSegmentType">Type of segments for distinguishing particular instances.</typeparam>
    /// <typeparam name="TSegment">Class type of segment.</typeparam>
    public interface ISegmentRegistry<in TSegmentType, TSegment> where TSegment : ISegment {

        /// <summary>Register segment and determine other values in subsequent calls by returned structure.</summary>
        /// <param name="segment">Instances of <typeparamref name="TSegment"/></param>
        /// <returns>Registration fluent syntax structure
        /// to determine particular <typeparamref name="TSegmentType"/> and other values.</returns>
        ISegmentRegistrationBuilder<TSegmentType> Register(TSegment segment);

        /// <summary>Get instance of <see cref="ISegment"/> previously registered by <see cref="Register"/>.</summary>
        /// <param name="segmentType">Type of registered instance.</param>
        /// <returns>Registered instance under <paramref name="segmentType"/>.</returns>
        TSegment Resolve(TSegmentType segmentType);
    }
}
