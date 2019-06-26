// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using SkiaSharp;
using System.Runtime.CompilerServices;

using GTTG.Core.Base;
using GTTG.Core.Drawing.Layers;

[assembly: InternalsVisibleTo("GTTG.ViewDateTimeInterval.Tests")]
    
namespace GTTG.Core.Drawing.Canvases {

    /// <summary>
    /// Represents abstract canvas wrapping <see cref="SKCanvas"/> with applied transformation of <see cref="SKMatrix"/> to enable
    /// drawing in specific area of <see cref="ContentDrawingCanvas"/> content.
    /// </summary>
    public struct DrawingCanvas {
        
        /// <summary>
        /// Width of this canvas.
        /// </summary>
        public float Width => Size.Width;

        /// <summary>
        /// Height of this canvas.
        /// </summary>
        public float Height => Size.Height;

        /// <summary>
        /// Size of this canvas.
        /// </summary>
        public SKSize Size { get; }

        /// <summary>
        /// <see cref="IDrawingLayer"/> where canvas was created.
        /// </summary>
        internal IDrawingLayer DrawingLayer { get; }

        /// <summary>
        /// Visible area of content canvas like <see cref="ContentDrawingCanvas"/> or <see cref="ViewDrawingCanvas"/> where this canvas resides.  
        /// </summary>
        internal SKRect View { get; }

        /// <summary>
        /// TransformationMatrix of content canvas like <see cref="ContentDrawingCanvas"/> or <see cref="ViewDrawingCanvas"/> where this canvas resides.
        /// </summary>
        public float[] SourceCanvasMatrix { get; }

        /// <summary>
        /// <see cref="SKCanvas"/> wrapped by this canvas.
        /// </summary>
        public readonly SKCanvas Canvas;

        private DrawingCanvas(DrawingCanvas drawingCanvas, ViewElement viewElement) {

            Canvas = drawingCanvas.Canvas;
            SourceCanvasMatrix = drawingCanvas.SourceCanvasMatrix;
            DrawingLayer = drawingCanvas.DrawingLayer;
            View = drawingCanvas.View;
            Size = new SKSize(Math.Min(viewElement.UnscaledWidth, drawingCanvas.Size.Width), Math.Min(viewElement.UnscaledHeight, drawingCanvas.Height));

            var placementMatrix = viewElement.PlacementMatrix;
            
            // multiply only scale, rotate, skew values, as others are not set
            var drawingMatrix = new SKMatrix {
                Persp2 = 1,
                ScaleX = placementMatrix.ScaleX * SourceCanvasMatrix[0],
                SkewY = placementMatrix.SkewY * SourceCanvasMatrix[4],
                SkewX = placementMatrix.SkewX * SourceCanvasMatrix[0],
                ScaleY = placementMatrix.ScaleY * SourceCanvasMatrix[4],
                TransX = placementMatrix.TransX * SourceCanvasMatrix[0] + SourceCanvasMatrix[2],
                TransY = placementMatrix.TransY * SourceCanvasMatrix[4] + SourceCanvasMatrix[5]
            };

            Canvas.SetMatrix(drawingMatrix);
        }

        /// <summary>
        /// Creates drawing canvas backed by Skia canvas.
        /// </summary>
        /// <param name="drawingLayer"><see cref="DrawingLayer"/> value.</param>
        /// <param name="canvas">Backing canvas. Should be transformation matrix modified, <see cref="SKCanvas.SetMatrix"/> is called outside.</param>
        /// <param name="size"><see cref="Size"/> value.</param>
        /// <param name="view">Positioned rectangle which is compared to placement of <see cref="ViewElement"/> to skip it's draw call out of canvas.</param>
        public DrawingCanvas(IDrawingLayer drawingLayer, SKCanvas canvas, SKSize size, SKRect view) {

            SourceCanvasMatrix = canvas.TotalMatrix.Values;
            Canvas = canvas;

            Size = size;
            View = view;
            DrawingLayer = drawingLayer;
        }

        internal DrawingCanvas Create(ViewElement viewElement) {
            return new DrawingCanvas(this, viewElement);
        }

        /// <summary>
        /// Draws visual element on canvas.
        /// </summary>
        public void Draw(IVisual visual) {
            visual.Draw(this);
        }
    }
}
