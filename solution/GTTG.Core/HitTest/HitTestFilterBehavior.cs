// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using GTTG.Core.Base;

namespace GTTG.Core.HitTest {

    /// <summary>
    /// Specifies the return behavior of a hit test in a hit test filter callback method.
    /// </summary>
    public enum HitTestFilterBehavior {

        /// <summary>
        ///  Hit test against the current <see cref="IVisual"/> and its descendants.
        /// </summary>
        Continue,

        /// <summary>
        /// Hit test against the current <see cref="IVisual"/>, but not its descendants.
        /// </summary>
        ContinueSkipChildren,

        /// <summary>
        /// Do not hit test against the current <see cref="IVisual"/>, but hit test against its descendants.
        /// </summary>
        ContinueSkipSelf,

        /// <summary>
        ///  Do not hit test against the current <see cref="IVisual"/> or its descendants.
        /// </summary>
        ContinueSkipSelfAndChildren,

        /// <summary>
        /// Stop hit testing at the current <see cref="IVisual"/>.
        /// </summary>
        Stop
    }
}
