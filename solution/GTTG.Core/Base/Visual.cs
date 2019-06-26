// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using SkiaSharp;

using GTTG.Core.Drawing.Canvases;
using GTTG.Core.Drawing.Layers;

namespace GTTG.Core.Base {

    /// <summary>Base class for drawable object with draw access checking and draw ownership with hit-testing.</summary>
    public abstract class Visual : ObservableObject, IVisual {

        private readonly Stack<IDrawingLayer> _drawingOwnerStack;

        /// <inheritdoc />
        public IDrawingLayer CurrentDrawingLayer { get; private set; }

        /// <summary>Creates visual with <see cref="CurrentDrawingLayer"/> initialized to <see cref="DefaultDrawingLayer"/>.</summary>
        protected Visual() {
            _drawingOwnerStack = new Stack<IDrawingLayer>();
            _drawingOwnerStack.Push(DefaultDrawingLayer.Get);
            CurrentDrawingLayer = DefaultDrawingLayer.Get;
        }

        /// <inheritdoc />
        public void PushDrawingLayer(IDrawingLayer drawingOwner) {

            if (drawingOwner == null) throw new ArgumentNullException($"Argument {nameof(drawingOwner)} does not accept null.");

            foreach (var visual in ProvideVisualsInSameLayer()) {
                visual.PushDrawingLayer(drawingOwner);    
            }

            _drawingOwnerStack.Push(drawingOwner);
            CurrentDrawingLayer = drawingOwner;
        }

        /// <inheritdoc />
        public void PopDrawingLayer() {
            if (_drawingOwnerStack.Count == 1) return;

            foreach (var visuals in ProvideVisualsInSameLayer()) {
                visuals.PopDrawingLayer();
            }

            _drawingOwnerStack.Pop();
            CurrentDrawingLayer = _drawingOwnerStack.Peek();
        }

        /// <summary>Returns true if visual lies in <paramref name="drawingLayer"/>.</summary>
        /// <returns>True if <see cref="CurrentDrawingLayer"/> equals to <param name="drawingLayer"/>. If <see cref="CurrentDrawingLayer"/> is <see cref="DefaultDrawingLayer"/>, returns also true; otherwise false.</returns>
        public bool IsInDrawingLayer(IDrawingLayer drawingLayer) {

            return CurrentDrawingLayer is DefaultDrawingLayer ||
                   CurrentDrawingLayer.Equals(drawingLayer);
        }

        /// <summary>Determines if visual can be drawn by <paramref name="drawingCanvas"/>.</summary>
        /// <returns>True if <see cref="IsInDrawingLayer"/> returns true for <see cref="DrawingCanvas.DrawingLayer"/>.</returns>
        public bool IsDrawableOnCanvas(DrawingCanvas drawingCanvas) {
            return IsInDrawingLayer(drawingCanvas.DrawingLayer);
        }

        /// <inheritdoc />
        public virtual void Draw(DrawingCanvas drawingCanvas) {

            if (!IsDrawableOnCanvas(drawingCanvas))  return;
            OnDraw(drawingCanvas);
        }

        /// <summary>Determines if visual has same layer as this instance.</summary>
        /// <param name="visual"><see cref="IVisual"/> to compare.</param>
        /// <returns>True if has same layer; otherwise false.</returns>
        public bool IsInSameLayer(IVisual visual) {
            return CurrentDrawingLayer == visual.CurrentDrawingLayer;
        }

        /// <summary>After drawing checks are done, this method is called for user to apply drawing on <param name="drawingCanvas"/>.</summary>
        protected virtual void OnDraw(DrawingCanvas drawingCanvas) { }

        /// <inheritdoc />
        public abstract bool HasHit(SKPoint contentPoint);

        /// <inheritdoc />
        public IEnumerable<IVisual> ProvideVisualsInSameLayer() {

            foreach (var child in ProvideVisuals()) {
                if (IsInSameLayer(child)) {
                    yield return child;
                }
            }
        }

        /// <summary>Provides elements drawn in this element in draw order.</summary>
        public abstract IEnumerable<IVisual> ProvideVisuals();
    }
}
