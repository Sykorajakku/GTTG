// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

using GTTG.Core.Base;
using GTTG.Core.Strategies.Interfaces;

namespace GTTG.Core.Strategies.Implementations {

    /// <summary>
    /// Represents strategy manager for segments with measureable height.
    /// Manager registers measure methods to segments as it's resources.
    /// </summary>
    public class MeasurableStrategyManager<TPlacementType, TElement, TSegmentType>
        : StrategyManager<TPlacementType, TElement, TSegmentType, MeasureableSegment>
        where TElement : IVisual {

        /// <summary>Implementation of measure method for <typeparamref name="TElement"/> values.</summary>
        protected readonly IElementMeasureProvider<TPlacementType, TElement, TSegmentType> MeasureProvider;

        /// <summary>Instances of <see cref="HeightMeasureHelper"/> under registration of <typeparamref name="TPlacementType"/>.</summary>
        protected readonly Dictionary<TPlacementType, HeightMeasureHelper> MeasureHelperRegistrations;

        /// <summary>Creates strategy manager with measureable content.</summary>
        /// <param name="measureProvider">Interface to which are passed added elements to be measured.</param>
        /// <param name="segmentRegistry">Segments registry to receive segments of <typeparamref name="TSegmentType"/>.</param>
        /// <param name="typeConverter">Instance of converter between specified types.</param>
        public MeasurableStrategyManager(ISegmentRegistry<TSegmentType, MeasureableSegment> segmentRegistry,
                                         ITypeConverter<TPlacementType, TSegmentType> typeConverter,
                                         IElementMeasureProvider<TPlacementType, TElement, TSegmentType> measureProvider)
            
            : base(segmentRegistry, typeConverter) {

            MeasureProvider = measureProvider;
            MeasureHelperRegistrations = new Dictionary<TPlacementType, HeightMeasureHelper>();
        }

        /// <inheritdoc />
        public override void Add(KeyValuePair<TPlacementType, TElement> item) {

            if (Elements.ContainsKey(item.Key)) {
                throw new StrategyException($"Item {item} is already in manager");
            }

            var segmentType = TypeConverter.Convert(item.Key);
            var segment = SegmentRegistry.Resolve(segmentType);
            float MeasureHelper() => MeasureProvider.MeasureHeight(item.Key, item.Value, segmentType, segment);
            segment.HeightMeasureHelpers += MeasureHelper;

            Segments.Add(item.Key, segment);
            MeasureHelperRegistrations.Add(item.Key, MeasureHelper);
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

            foreach (var segmentEntry in SegmentTypes) {
                SegmentRegistry.Resolve(segmentEntry.Value).HeightMeasureHelpers -= MeasureHelperRegistrations[segmentEntry.Key];
            }

            base.Clear();
            MeasureHelperRegistrations.Clear();
        }

        /// <inheritdoc />
        public override bool Remove(KeyValuePair<TPlacementType, TElement> item) {
            return Remove(item.Key);
        }

        /// <inheritdoc />
        public override bool Remove(TPlacementType key) {

            var segmentType = SegmentTypes[key];
            var isRemoved = base.Remove(key);
            if (isRemoved) {
                SegmentRegistry.Resolve(segmentType).HeightMeasureHelpers -= MeasureHelperRegistrations[key];
            }
            return isRemoved;
        }

    }
}
