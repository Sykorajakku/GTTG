// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace GTTG.Core.Strategies.Implementations {
    
    /// <summary>Represents measurable segment used for strategies. Measures content from added event handlers.</summary>
    public class MeasureableSegment : Segment {
        
        /// <summary>Height of segment's content set from <see cref="MeasureHeight"/> calls.</summary>
        public float DesiredHeight { get; protected set; }

        /// <summary>Add <see cref="HeightMeasureHelper"/> method to measure content of particular element to be placed in this segment; invoked on <see cref="MeasureHeight"/>.</summary>
        public event HeightMeasureHelper HeightMeasureHelpers;

        /// <summary>Measures height of segment by selecting maximum from <see cref="HeightMeasureHelpers"/> invocations. Measured value is set to <see cref="DesiredHeight"/>.</summary>
        public void MeasureHeight() {

            var max = 0f;
            if (HeightMeasureHelpers == null) {
                DesiredHeight = max;
                return;
            }

            foreach (var measureHelper in HeightMeasureHelpers?.GetInvocationList()) {
                max = Math.Max(max, ((HeightMeasureHelper) measureHelper)());
            }

            DesiredHeight = max;
        }
    }
}
