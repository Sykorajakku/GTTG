// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using GTTG.Core.Drawing.Canvases;

namespace GTTG.Core.Drawing.Layers {
    
    /// <inheritdoc />
    public abstract class DrawingLayer : IDrawingLayer {

        /// <inheritdoc />
        public virtual void Draw(DrawingCanvas drawingCanvas) {
            OnDraw(drawingCanvas);
        }

        /// <summary>
        /// Draws layer's inner content.
        /// </summary>
        protected abstract void OnDraw(DrawingCanvas drawingCanvas);
    }
}
