// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

using GTTG.Core.Strategies.Interfaces;

namespace GTTG.Core.Strategies.Implementations {

    /// <inheritdoc />
    public class SegmentRegistry<TSegmentType, TSegment> : ISegmentRegistry<TSegmentType, TSegment> 
        where TSegment : Segment {

        /// <summary>
        /// Internal structure to book keep the registered segments.
        /// </summary>
        protected readonly Dictionary<TSegmentType, TSegment> RegisteredSegments;

        /// <summary>Creates empty <see cref="SegmentRegistry{TSegmentType,TSegment}"/> with no segment registrations.</summary>
        public SegmentRegistry() {
            RegisteredSegments = new Dictionary<TSegmentType, TSegment>();
        }

        /// <inheritdoc />
        public virtual ISegmentRegistrationBuilder<TSegmentType> Register(TSegment segment) {
            return new SegmentRegistrationBuilder(RegisteredSegments, segment);
        }

        /// <inheritdoc />
        public virtual TSegment Resolve(TSegmentType segmentType) {

            if (!RegisteredSegments.ContainsKey(segmentType)) {
                throw new StrategyException("No segment registered under this value.");
            }
            return RegisteredSegments[segmentType];
        }

        /// <summary>Implementation of registration builder for adding instances after specified type.</summary>
        protected class SegmentRegistrationBuilder : ISegmentRegistrationBuilder<TSegmentType> {

            /// <summary>
            /// Internal structure to book keep the registered segments.
            /// </summary>
            protected readonly Dictionary<TSegmentType, TSegment> RegisteredSegments;

            /// <summary>
            /// New segment added be added to <see cref="RegisteredSegments"/>.
            /// </summary>
            protected readonly TSegment NewSegment;

            /// <summary>
            /// Creates builder with registration instances.
            /// </summary>
            /// <param name="registeredSegments">Dictionary of segments where instance under new type is added</param>
            /// <param name="newSegment">Instance of segment whose type must be specified.</param>
            public SegmentRegistrationBuilder(Dictionary<TSegmentType, TSegment> registeredSegments, TSegment newSegment) {
                RegisteredSegments = registeredSegments;
                NewSegment = newSegment;
            }

            ISegmentRegistrationBuilder<TSegmentType> ISegmentRegistrationBuilder<TSegmentType>.As(TSegmentType segmentType) {

                if (RegisteredSegments.ContainsKey(segmentType)) {
                    throw new StrategyException($"Segment registered under value { segmentType } already exists.");
                }

                RegisteredSegments.Add(segmentType, NewSegment);

                return this;
            }
        }
    }
}
