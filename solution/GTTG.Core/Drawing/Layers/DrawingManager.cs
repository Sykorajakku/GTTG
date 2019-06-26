// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using SkiaSharp;

using GTTG.Core.Drawing.Canvases;

namespace GTTG.Core.Drawing.Layers {

    /// <summary>Manages drawing of added <see cref="IDrawingLayer"/>s by defined order.</summary>
    public class DrawingManager {

        /// <summary>Ordered list of layers starting from the undermost one.</summary>
        public IReadOnlyList<(IDrawingLayer DrawingLayer, bool IsRegistered)> Layers => _drawingLayers.AsReadOnly();

        private readonly ICanvasFactory _canvasFactory;
        private readonly List<(IDrawingLayer DrawingLayer, bool IsRegistered)> _drawingLayers;
        private readonly IRegisteredLayersOrder _registeredLayersOrder;

        /// <summary>Creates empty <see cref="DrawingManager"/> with registered layers from <see cref="IRegisteredLayersOrder"/>.</summary>
        /// <param name="canvasFactory">Factory to create <see cref="DrawingCanvas"/> for each <see cref="IDrawingLayer"/>.</param>
        /// <param name="registeredLayersOrder">Ordered list of types of registered layers to be placed in defined order by <see cref="ReplaceRegisteredDrawingLayer"/></param>
        public DrawingManager(ICanvasFactory canvasFactory, IRegisteredLayersOrder registeredLayersOrder) {

            _canvasFactory = canvasFactory;
            _drawingLayers = new List<(IDrawingLayer, bool)>();
            _registeredLayersOrder = registeredLayersOrder;

            for (var i = 0; i < registeredLayersOrder.DrawingLayerTypeList.Count; i++) {
                _drawingLayers.Add((DefaultDrawingLayer.Get, true));
            }
        }

        /// <summary>Get layer by type.</summary>
        /// <typeparam name="T">SegmentType of layer to get.</typeparam>
        /// <returns>Instance of <see cref="IDrawingLayer"/> of type <typeparamref name="T"/> if found; otherwise default of <typeparamref name="T"/>.</returns>
        public T GetDrawingLayer<T>() where T : IDrawingLayer {

            foreach (var (layer, _) in Layers) {

                if (layer.GetType() == typeof(T)) {
                    return (T) layer;
                }
            }

            return default(T);
        }

        /// <summary>
        /// Replaces instance of drawing layer in registration of type of <paramref name="drawingLayer"/> from <see cref="IRegisteredLayersOrder"/>.
        /// </summary>
        /// <param name="drawingLayer">Instance of drawing layer that replaces previous instance under the registration.</param>
        /// <param name="registeredIndex">In case of multiple registrations of same type, zero-based index selects the registration.</param>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="drawingLayer"/> was not registered in <see cref="IRegisteredLayersOrder"/>
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="registeredIndex"/> is present in <see cref="IRegisteredLayersOrder"/>, but count of registrations does not corresponds to the registered index.
        /// </exception>
        public void ReplaceRegisteredDrawingLayer(IDrawingLayer drawingLayer, int registeredIndex = 0) {

            var typeRegistrationIndices = FindTypeRegistrations(drawingLayer);

            if (typeRegistrationIndices.Count == 0) {
                throw new ArgumentException($"SegmentType of { nameof(drawingLayer) } is not present in registered drawing layers.");
            }

            if (registeredIndex >= typeRegistrationIndices.Count || registeredIndex < 0) {
                throw new ArgumentOutOfRangeException($"Index { nameof(registeredIndex)} is out of range under of count of registrations of type of { nameof(drawingLayer) }.");
            }

            var typeRegistrationIndex = typeRegistrationIndices[registeredIndex];
            _drawingLayers[typeRegistrationIndex] = (drawingLayer, true);
        }

        private List<int> FindTypeRegistrations(IDrawingLayer drawingLayer) {

            var registrationIndex = 0;
            var indices = new List<int>();

            for (var i = 0; i < _drawingLayers.Count; ++i) {

                if (_drawingLayers[i].IsRegistered) {

                    var registeredType = _registeredLayersOrder.DrawingLayerTypeList[registrationIndex];

                    if (drawingLayer.GetType().IsSubclassOf(registeredType) || drawingLayer.GetType() == registeredType) {
                        indices.Add(i);
                    }
                    registrationIndex++;
                }
            }

            return indices;
        }

        /// <summary>
        /// Removes instance of drawing layer from <see cref="Layers"/>.
        /// </summary>
        /// <param name="index">
        /// If entry under index is registered, entry is not removed and instance is set to <see cref="DefaultDrawingLayer"/>.
        /// Otherwise, entry is removed from <see cref="Layers"/>.
        /// </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// Provided <paramref name="index"/> out of range in <see cref="Layers"/>.
        /// </exception>
        public void RemoveDrawingLayer(int index) {

            if (index < 0 || index >= _drawingLayers.Count) {
                throw new ArgumentOutOfRangeException($"{nameof(Layers)} does not contain drawing layer under index: { index }.");
            }

            if (_drawingLayers[index].IsRegistered) {
                _drawingLayers[index] = (DefaultDrawingLayer.Get, true);
            }
            else {
                var entry = _drawingLayers.ElementAt(index);
                _drawingLayers.Remove(entry);
            }
        }

        /// <summary>Inserts <paramref name="drawingLayer"/> into the drawn layers as the current topmost layer.</summary>
        /// <param name="drawingLayer">The drawing layer to insert.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="drawingLayer"/> is null reference.</exception>
        /// <returns><see langword="true" /> if <paramref name="drawingLayer"/> is successfully added; otherwise, <see langword="false" />. This method also returns <see langword="false" /> if <paramref name="drawingLayer" /> is already present in the manager.</returns>
        public void AddOnCurrentTop(IDrawingLayer drawingLayer) {

            AddNewLayer(drawingLayer, layer => _drawingLayers.Add((layer, false)));
        }

        /// <summary>Inserts <paramref name="drawingLayer"/> into the drawn layers as the current undermost layer.</summary>
        /// <param name="drawingLayer">The drawing layer to insert.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="drawingLayer"/> is null reference.</exception>
        /// <returns><see langword="true" /> if <paramref name="drawingLayer"/> is successfully added; otherwise, <see langword="false" />.This method also returns <see langword="false" /> if <paramref name="drawingLayer" /> is already present in the manager.</returns>
        public void AddOnCurrentBottom(IDrawingLayer drawingLayer) {

            AddNewLayer(drawingLayer, layer => _drawingLayers.Insert(0, (layer, false)));
        }

        /// <summary>Inserts <paramref name="drawingLayer"/> into the drawn layers at the specified <paramref name="index"/>.</summary>
        /// <param name="index">The zero-based index at which <paramref name="drawingLayer" /> should be inserted.</param>
        /// <param name="drawingLayer">The drawing layer to insert.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="drawingLayer"/> is null reference.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.-or-
        /// <paramref name="index" /> is greater than <see cref="P:System.Collections.Generic.List`1.Count" />.</exception>
        /// <returns><see langword="true" /> if <paramref name="drawingLayer"/> is successfully added; otherwise, <see langword="false" />. This method also returns <see langword="false" /> if <paramref name="drawingLayer" /> is already present in the manager.</returns>
        public void Insert(int index, IDrawingLayer drawingLayer) {

            AddNewLayer(drawingLayer, layer => _drawingLayers.Insert(index, (layer, false)));
        }

        /// <summary>Returns all drawing layers in order from undermost to topmost one.</summary>
        public IEnumerable<IDrawingLayer> GetDrawingLayersFromUndermostOne() {

            foreach (var drawingLayer in _drawingLayers) {
                yield return drawingLayer.DrawingLayer;
            }
        }

        /// <summary>Draws layers in order defined in <see cref="Layers"/> on surface.</summary>
        public void Draw(SKSurface surface) {

            var matrix = surface.Canvas.TotalMatrix;
            surface.Canvas.Clear();

            foreach (var layerEntry in _drawingLayers) {

                var layer = layerEntry.DrawingLayer;
                var canvas = _canvasFactory.CreateCanvas(layer, surface.Canvas);
                layer.Draw(canvas);
                surface.Canvas.SetMatrix(matrix);
            }
        }

        private static void AddNewLayer(IDrawingLayer drawingLayer, Action<IDrawingLayer> addDrawingLayer) {
            addDrawingLayer(drawingLayer);
        }
    }
}
