// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using SkiaSharp;

using GTTG.Core.Component;
using GTTG.Core.Drawing.Layers;

namespace GTTG.Core.Drawing.Canvases {

    /// <summary>
    /// Implementation of <see cref="ICanvasFactory"/> for <see cref="ContentDrawingCanvas"/>, <see cref="ViewDrawingCanvas"/> and <see cref="DefaultDrawingLayer"/>.
    /// </summary>
    public class CanvasFactory : ICanvasFactory {

        private readonly IViewProvider _viewProvider;
        private readonly ContentDrawingCanvas _contentDrawingCanvas;
        private readonly ViewDrawingCanvas _viewDrawingCanvas;
        private readonly DefaultDrawingCanvas _defaultDrawingCanvas;

        /// <summary>
        /// Allows configuration of supported canvases by state of <see cref="GraphicalComponent"/>.
        /// </summary>
        /// <param name="viewProvider">Receives state of <see cref="GraphicalComponent"/> represented by this instance.</param>
        public CanvasFactory(IViewProvider viewProvider) {
            _viewProvider = viewProvider;
            _contentDrawingCanvas = new ContentDrawingCanvas();
            _viewDrawingCanvas = new ViewDrawingCanvas();
            _defaultDrawingCanvas = DefaultDrawingCanvas.Get;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentException">If <paramref name="drawingLayer"/> instance is not recognized.</exception>
        public DrawingCanvas CreateCanvas(IDrawingLayer drawingLayer, SKCanvas skCanvas) {

            SpecificDrawingCanvas specificDrawingCanvas = null;

            if (drawingLayer is ViewDrawingLayer) {
                specificDrawingCanvas = _viewDrawingCanvas;
            }

            if (drawingLayer is ContentDrawingLayer) {
                specificDrawingCanvas = _contentDrawingCanvas;
            }

            if (drawingLayer is DefaultDrawingLayer) {
                specificDrawingCanvas = _defaultDrawingCanvas;
            }

            if (specificDrawingCanvas == null) {
                throw new ArgumentException($"{drawingLayer} not recognized");
            }

            return CreateDrawingCanvas(specificDrawingCanvas, drawingLayer, skCanvas);
        }

        private DrawingCanvas CreateDrawingCanvas(SpecificDrawingCanvas drawingCanvas, IDrawingLayer drawingLayer, SKCanvas skCanvas) {
            drawingCanvas.Update(_viewProvider, skCanvas);
            drawingCanvas.ChangeDrawingLayer(drawingLayer);
            return drawingCanvas.DrawingCanvas;
        }
    }
}
