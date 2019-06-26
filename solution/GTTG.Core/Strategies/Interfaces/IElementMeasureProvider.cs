// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace GTTG.Core.Strategies.Interfaces {

    /// <summary>Contract for height measure of particular element with provided information about registered type and segment.</summary>
    public interface IElementMeasureProvider<in TPlacementType, in TElement, in TSegmentType> {

        /// <summary>Measures height of <typeparamref name="TElement"/> by provided parameters.</summary>
        float MeasureHeight(TPlacementType placementType, TElement element, TSegmentType segmentType, ISegment segment);
    }
}
