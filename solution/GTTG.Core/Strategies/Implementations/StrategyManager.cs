// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

using GTTG.Core.Base;
using GTTG.Core.Strategies.Interfaces;

namespace GTTG.Core.Strategies.Implementations {

    /// <summary>
    /// Represents strategy manager that maps content to particular segment instance.
    /// </summary>
    /// <typeparam name="TPlacementType">Type of placement type.</typeparam>
    /// <typeparam name="TElement">Type of added elements.</typeparam>
    /// <typeparam name="TSegmentType">Type of segment type.</typeparam>
    /// <typeparam name="TSegment">Type of segment instances.</typeparam>
    public class StrategyManager<TPlacementType, TElement, TSegmentType, TSegment>
        : BasicStrategyManager<TPlacementType, TElement, TSegmentType>
        where TElement : IVisual
        where TSegment : ISegment {

        /// <summary>Maps registered <typeparamref name="TPlacementType"/> types to it's <see cref="ISegment"/>.</summary>
        public IReadOnlyDictionary<TPlacementType, ISegment> ManagedSegments => Segments;

        /// <summary>A registry from where segments are received.</summary>
        protected readonly ISegmentRegistry<TSegmentType, TSegment> SegmentRegistry;

        /// <summary>Collection of information about <typeparamref name="TElement"/> registration under <typeparamref name="TPlacementType"/>.</summary>
        protected readonly Dictionary<TPlacementType, ISegment> Segments;
        
        /// <summary>Creates empty <see cref="StrategyManager{TPlacementType,TElement,TSegmentType, TSegment}"/>.</summary>
        /// <param name="segmentRegistry">Segments registry to receive segments of <typeparamref name="TSegmentType"/>.</param>
        /// <param name="typeConverter">Instance of converter between specified types.</param>
        public StrategyManager(ISegmentRegistry<TSegmentType, TSegment> segmentRegistry,
                               ITypeConverter<TPlacementType, TSegmentType> typeConverter)
            : base(typeConverter) {

            SegmentRegistry = segmentRegistry;
            Segments = new Dictionary<TPlacementType, ISegment>();
        }

        /// <inheritdoc />
        public override void Add(KeyValuePair<TPlacementType, TElement> item) {

            if (Elements.Contains(item)) {
                throw new StrategyException($"Item {item} is already in manager");
            }

            var segmentType = TypeConverter.Convert(item.Key);
            var segment = SegmentRegistry.Resolve(segmentType);

            Segments.Add(item.Key, segment);
            Elements.Add(item.Key, item.Value);
            SegmentTypes.Add(item.Key, segmentType);
        }

        /// <inheritdoc />
        public override void Add(TPlacementType key, TElement value) {
            var item = new KeyValuePair<TPlacementType, TElement>(key, value);
            Add(item);
        }

        /// <inheritdoc />
        public override void Clear() {

            base.Clear();
            Segments.Clear();
        }
        
        /// <inheritdoc />
        public override bool Remove(KeyValuePair<TPlacementType, TElement> item) {
            return Remove(item.Key);
        }

        /// <inheritdoc />
        public override bool Remove(TPlacementType key) {

            var isRemoved = Elements.Remove(key);
            if (isRemoved) {
                Segments.Remove(key);
                SegmentTypes.Remove(key);
            }
            return isRemoved;
        }
    }
}
