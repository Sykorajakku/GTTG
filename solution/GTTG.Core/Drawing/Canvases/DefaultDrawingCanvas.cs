// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using SkiaSharp;

using GTTG.Core.Component;
using GTTG.Core.Drawing.Layers;

namespace GTTG.Core.Drawing.Canvases {

    /// <summary>
    /// Singleton abstract canvas for <see cref="Layers.DefaultDrawingLayer"/> with no drawn content.
    /// </summary>
    public sealed class DefaultDrawingCanvas : SpecificDrawingCanvas {

        /// <summary>
        /// Get instance of singleton.
        /// </summary>
        public static DefaultDrawingCanvas Get { get; }

        static DefaultDrawingCanvas() {
            Get = new DefaultDrawingCanvas();
        }

        private DefaultDrawingCanvas() {
        }

        /// <inheritdoc />
        public override void Update(IViewProvider viewProvider, SKCanvas canvas) {
            DrawingCanvas = new DrawingCanvas(DefaultDrawingLayer.Get, canvas, SKSize.Empty, SKRect.Empty);
        }
    }
}
