// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using SkiaSharp;

using GTTG.Core.Drawing.Canvases;
using GTTG.Core.Drawing.Layers;

namespace GTTG.Core.Base {

    /// <summary>Contract for object which can be drawn and hit-tested.</summary>
    public interface IVisual {

        /// <summary>Current drawing layer which visual belongs to.</summary>
        IDrawingLayer CurrentDrawingLayer { get; }

        /// <summary>Change <see cref="CurrentDrawingLayer"/> to which visual belongs to by adding new value on drawing layer stack.</summary>
        /// <param name="drawingLayer">New drawing layer to which visual belongs to.</param>
        void PushDrawingLayer(IDrawingLayer drawingLayer);

        /// <summary>Change <see cref="CurrentDrawingLayer"/> which visual belongs by removing top layer from drawing layer stack. If no previous value available, <see cref="DefaultDrawingLayer"/> is selected.</summary>
        void PopDrawingLayer();
        
        /// <summary>Draws content of this visual to <paramref name="drawingCanvas"/> if <see cref="DrawingCanvas.DrawingLayer"/> is same as <see cref="CurrentDrawingLayer"/> or <see cref="CurrentDrawingLayer"/> is <see cref="DefaultDrawingLayer"/>.</summary>
        /// <param name="drawingCanvas">Canvas which belongs to this visual for drawing.</param>
        void Draw(DrawingCanvas drawingCanvas);

        /// <summary>Hit-tests this target against provided point.</summary>
        /// <param name="contentPoint">Point against which target is tested, in coordinate system of <see cref="ContentDrawingCanvas"/>.</param>
        /// <returns>True if target was hit; otherwise false.</returns>
        bool HasHit(SKPoint contentPoint);

        /// <summary>Returns visual children of this visual that has same <see cref="CurrentDrawingLayer"/>.</summary>
        IEnumerable<IVisual> ProvideVisualsInSameLayer();

        /// <summary>Returns all visual children of this visual.</summary>
        IEnumerable<IVisual> ProvideVisuals();
    }
}
