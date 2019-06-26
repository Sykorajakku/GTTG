// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using SkiaSharp;

using GTTG.Core.Component;
using GTTG.Core.Drawing.Layers;

namespace GTTG.Core.Drawing.Canvases {

    /// <summary>Represents canvas covering currently displayed content.</summary>
    public sealed class ViewDrawingCanvas : SpecificDrawingCanvas {
        
        /// <inheritdoc />
        public override void Update(IViewProvider viewProvider, SKCanvas canvas) {

            var canvasRect = SKRect.Create(SKPoint.Empty, new SKSize(viewProvider.ViewWidth, viewProvider.ViewHeight));
            DrawingCanvas = new DrawingCanvas(DefaultDrawingLayer.Get, canvas, canvasRect.Size, canvasRect);
        }
    }
}
