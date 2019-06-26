// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using GTTG.Core.Drawing.Layers;

using SkiaSharp;

namespace GTTG.Core.Drawing.Canvases {

    /// <summary>
    /// Contract for object creating <see cref="DrawingCanvas"/> from provided layer and canvas.
    /// </summary>
    public interface ICanvasFactory {

        /// <summary>
        /// Creates drawing canvas from drawing layer and canvas.
        /// </summary>
        /// <param name="drawingLayer"><see cref="IDrawingLayer"/> to create canvas from. <see cref="DrawingCanvas.DrawingLayer"/> is set to this value.</param>
        /// <param name="skCanvas"><see cref="SKCanvas"/> whose <see cref="SKCanvas.TotalMatrix"/> can be modified by calling this method.</param>
        /// <returns>Instance of <see cref="DrawingCanvas"/>.</returns>
        DrawingCanvas CreateCanvas(IDrawingLayer drawingLayer, SKCanvas skCanvas);
    }
}
