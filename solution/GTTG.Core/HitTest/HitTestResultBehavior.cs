// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace GTTG.Core.HitTest {

    /// <summary>
    /// Determines whether to continue the enumeration of any remaining visual objects during a hit test.
    /// </summary>
    public enum HitTestResultBehavior {

        /// <summary>
        ///  Continue hit testing against the next visual in the visual tree hierarchy.
        /// </summary>
        Continue,

        /// <summary>
        /// Stop any further hit testing and return from the callback.
        /// </summary>
        Stop
    }
}
