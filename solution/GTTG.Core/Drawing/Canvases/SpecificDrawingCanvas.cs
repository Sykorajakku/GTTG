// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using SkiaSharp;

using GTTG.Core.Component;
using GTTG.Core.Drawing.Layers;

namespace GTTG.Core.Drawing.Canvases {

    /// <summary>
    /// Represents specific canvas used for some scenario of drawing.
    /// </summary>
    public abstract class SpecificDrawingCanvas {

        /// <summary>
        /// Instance of <see cref="DrawingCanvas"/> representing this canvas.
        /// </summary>
        public DrawingCanvas DrawingCanvas { get; protected set; }
       
        /// <summary>Updates drawing layer of <see cref="DrawingCanvas"/>.</summary>
        /// <param name="drawingLayer">New <paramref name="drawingLayer"/> of <see cref="DrawingCanvas"/>.</param>
        public void ChangeDrawingLayer(IDrawingLayer drawingLayer) {
            DrawingCanvas = new DrawingCanvas(drawingLayer, DrawingCanvas.Canvas, DrawingCanvas.Size, DrawingCanvas.View);
        }

        /// <summary>Updates <see cref="DrawingCanvas"/> with provided parameters.</summary>
        /// <param name="viewProvider">Information about state of <see cref="GraphicalComponent"/>.</param>
        /// <param name="canvas">Canvas which state can be modified as used for transformed drawing.</param>
        public abstract void Update(IViewProvider viewProvider, SKCanvas canvas);
    }
}
