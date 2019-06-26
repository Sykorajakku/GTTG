// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using GTTG.Core.Drawing.Canvases;

namespace GTTG.Core.Drawing.Layers {

    /// <summary>Represents drawing of one layer as logical set of components.</summary>
    public interface IDrawingLayer {

        /// <summary>Draws the layer onto <paramref name="drawingCanvas"/>.</summary>
        void Draw(DrawingCanvas drawingCanvas);
    }
}
