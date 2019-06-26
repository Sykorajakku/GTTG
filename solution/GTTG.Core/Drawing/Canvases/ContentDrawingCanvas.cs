// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using SkiaSharp;

using GTTG.Core.Component;
using GTTG.Core.Drawing.Layers;

namespace GTTG.Core.Drawing.Canvases {

    /// <summary>Represents canvas covering whole displayable content.</summary>
    public sealed class ContentDrawingCanvas : SpecificDrawingCanvas {

        /// <inheritdoc />
        public override void Update(IViewProvider viewProvider, SKCanvas canvas) {

            var globalMatrix = viewProvider.ContentMatrix;
            var unscaledView = SKRect.Create(
                (-globalMatrix.TransX) / globalMatrix.ScaleX,
                (-globalMatrix.TransY) / globalMatrix.ScaleX,
                viewProvider.ViewWidth / globalMatrix.ScaleX,
                viewProvider.ViewHeight / globalMatrix.ScaleY);

            SKMatrix.Concat(ref globalMatrix, globalMatrix, canvas.TotalMatrix);
            canvas.SetMatrix(globalMatrix);

            DrawingCanvas = new DrawingCanvas(DefaultDrawingLayer.Get, canvas, new SKSize(viewProvider.ContentWidth, viewProvider.ContentHeight), unscaledView);
        }
    }
}
