// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace GTTG.Core.Strategies.Interfaces {

    /// <summary>Converts type to different representation.</summary>
    public interface ITypeConverter<in TPlacementType, out TSegmentType> {

        /// <summary>Converts of <typeparamref name="TPlacementType"/> to <typeparamref name="TSegmentType"/>.</summary>
        TSegmentType Convert(TPlacementType placementType);
    }
}
