// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Drawing.Canvases;

namespace GTTG.Core.Drawing.Layers {

    /// <summary><see cref="IDrawingLayer"/> representing layer of <see cref="ContentDrawingCanvas"/>.</summary>
    public abstract class ContentDrawingLayer : DrawingLayer, IVisual {

        /// <inheritdoc/>
        public IDrawingLayer CurrentDrawingLayer => this;

        /// <inheritdoc/>
        /// <summary>Does nothing as object represents.</summary>
        public void PushDrawingLayer(IDrawingLayer drawingLayer) { }

        /// <inheritdoc/>
        /// <summary>Does nothing as object represents.</summary>
        public void PopDrawingLayer() { }

        /// <inheritdoc/>
        public bool HasHit(SKPoint contentPoint) {
            return true;
        }

        /// <inheritdoc/>
        public IEnumerable<IVisual> ProvideVisualsInSameLayer() {

            foreach (var visual in ProvideVisuals()) {
                yield return visual;
            }
        }

        /// <inheritdoc/>
        public abstract IEnumerable<IVisual> ProvideVisuals();
    }
}
