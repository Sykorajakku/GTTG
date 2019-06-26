// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using GTTG.Core.Base;

namespace GTTG.Core.HitTest {

    /// <summary>
    /// Represents a callback that is used to customize hit testing. GTTG invokes the HitTestResultCallback to report hit test intersections to the user.
    /// </summary>
    /// <param name="target">The <see cref="IVisual"/> object that is returned from a hit test.</param>
    /// <returns>A <see cref="HitTestResultBehavior"/> that represents the action resulting from the hit test.</returns>
    public delegate HitTestResultBehavior HitTestResultCallback(IVisual target);
}
