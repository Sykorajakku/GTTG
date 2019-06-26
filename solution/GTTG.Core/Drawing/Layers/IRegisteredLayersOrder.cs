// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;

namespace GTTG.Core.Drawing.Layers {

    /// <summary>
    /// Defines order of registered <see cref="IDrawingLayer"/> drawing layers.
    /// </summary>
    public interface IRegisteredLayersOrder {

        /// <summary>
        /// Ordered set of drawing layers types - <see cref="IDrawingLayer"/>. First index - 0 is visually on bottom.
        /// </summary>
        ImmutableList<Type> DrawingLayerTypeList { get; }
    }
}
