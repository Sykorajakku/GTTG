// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Drawing.Canvases;

namespace GTTG.Core.HitTest {

    /// <summary>
    /// Represents the callback method that specifies parts of the visual tree to omit from hit test processing
    /// </summary>
    /// <param name="target">The visual to hit test.</param>
    /// <param name="contentPoint">Point in <see cref="ContentDrawingCanvas"/> against which hit test.</param>
    /// <returns>A <see cref="HitTestResultBehavior"/> that represents the action resulting from the hit test.</returns>
    public delegate HitTestFilterBehavior HitTestFilterCallback(IVisual target, SKPoint contentPoint);
}
