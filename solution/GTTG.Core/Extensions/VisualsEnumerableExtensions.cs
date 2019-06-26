// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Drawing.Layers;

namespace GTTG.Core.Extensions {

    /// <summary>
    /// Extensions method for hit test of <see cref="IVisual"/> collections.
    /// </summary>
    public static class VisualsEnumerableExtensions {

        /// <summary>Hit-tests collection of <see cref="IVisual"/> elements.</summary>
        /// <param name="targets">Collection of <see cref="IVisual"/> elements to be hit-tested.</param>.
        /// <param name="contentPoint">Point in <see cref="Drawing.Canvases.ContentDrawingCanvas"/> tested for hit.</param>
        /// <typeparam name="THitTestTarget">Type of <see cref="IVisual"/> elements in collection to be processed.</typeparam>
        /// <returns>Elements which were hit. Elements are returned by order in <paramref name="targets"/>.</returns>
        public static IEnumerable<THitTestTarget> HitTest<THitTestTarget>(this IEnumerable<THitTestTarget> targets, SKPoint contentPoint) where THitTestTarget : IVisual {
            return targets.Where(v => v.HasHit(contentPoint));
        }

        /// <summary>Orders collection of <typeparamref name="TVisualType"/> elements by their <see cref="IDrawingLayer"/>.</summary>
        /// <param name="visuals">Collection of <typeparamref name="TVisualType"/> elements to be sorted, each belonging to some <see cref="IDrawingLayer"/>.</param>
        /// <param name="drawingLayersOrder"><see cref="IDrawingLayer"/>s are ordered by order in this enumeration. Returns first elements from first layer in enumeration.</param>
        /// <param name="sourceDrawingLayer">If <see cref="IDrawingLayer"/> of visual element is <see cref="DefaultDrawingLayer"/>, treat this visual element as one belonging to <paramref name="sourceDrawingLayer"/> layer.</param>
        /// <typeparam name="TVisualType">SegmentType of visual elements in collection to be processed.</typeparam>
        /// <exception cref="ArgumentException">If <paramref name="sourceDrawingLayer"/> is type of <see cref="DefaultDrawingLayer"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="sourceDrawingLayer"/> is not present in <paramref name="drawingLayersOrder"/>.</exception>
        /// <exception cref="ArgumentException">Throws during enumeration if <see cref="IVisual.CurrentDrawingLayer"/> of visual from <paramref name="visuals"/> is not present in <paramref name="drawingLayersOrder"/>.</exception>
        /// <returns>Sorted collection of visual elements by their drawing layers. First element returned is in layer which comes first in <paramref name="drawingLayersOrder"/> enumeration. If drawing layers are same, order is defined by order in <paramref name="visuals"/>.</returns>
        public static IEnumerable<TVisualType> OrderByLayers<TVisualType>(this IEnumerable<TVisualType> visuals, IEnumerable<IDrawingLayer> drawingLayersOrder, IDrawingLayer sourceDrawingLayer)
            where TVisualType : IVisual {

            if (sourceDrawingLayer is DefaultDrawingLayer) {
                throw new ArgumentException("SegmentType of source drawing layer can't be default drawing layer.");
            }

            var drawingLayersList = drawingLayersOrder.ToList();
            if (!drawingLayersList.Contains(sourceDrawingLayer)) {
                throw new ArgumentException($"Instance of type of source drawing layer is not present in {nameof(drawingLayersOrder)}.");
            }

            return OrderByLayersSanitized(visuals, drawingLayersList, sourceDrawingLayer);
        }

        /// <summary>Orders collection of <typeparamref name="TVisualType"/> elements by their <see cref="IDrawingLayer"/>.</summary>
        /// <param name="visuals">Collection of <typeparamref name="TVisualType"/> elements to be sorted, each belonging to some <see cref="IDrawingLayer"/>.</param>
        /// <param name="drawingLayersOrder">Layers are ordered by order in this enumeration. Returns first elements from first layer in enumeration.</param>
        /// <typeparam name="TSourceDrawingLayer">If <see cref="IDrawingLayer"/> of visual element is <see cref="DefaultDrawingLayer"/>, treat this visual element as one belonging to instance of layer of this type.</typeparam>
        /// <typeparam name="TVisualType">SegmentType of visual elements in collection to be processed.</typeparam>
        /// <exception cref="ArgumentException">If <typeparamref name="TSourceDrawingLayer"/> is type of <see cref="DefaultDrawingLayer"/>.</exception>
        /// <exception cref="ArgumentException">If instance of type <typeparamref name="TSourceDrawingLayer"/> occurs multiple times in <paramref name="drawingLayersOrder"/>.</exception>
        /// <exception cref="ArgumentException">If instance of type <typeparamref name="TSourceDrawingLayer"/> is not present in <paramref name="drawingLayersOrder"/>.</exception>
        /// <exception cref="ArgumentException">Throws during enumeration if <see cref="IVisual.CurrentDrawingLayer"/> of visual from <paramref name="visuals"/> is not present in <paramref name="drawingLayersOrder"/>.</exception>
        /// <returns>Sorted collection of visual elements by their drawing layers. First element returned is in layer which comes first in <paramref name="drawingLayersOrder"/> enumeration. If drawing layers are same, order is defined by order in <paramref name="visuals"/>.</returns>
        public static IEnumerable<TVisualType> OrderByLayers<TVisualType, TSourceDrawingLayer>(this IEnumerable<TVisualType> visuals, IEnumerable<IDrawingLayer> drawingLayersOrder)
            where TVisualType : IVisual where TSourceDrawingLayer : IDrawingLayer {

            if (typeof(TSourceDrawingLayer) == typeof(DefaultDrawingLayer)) {
                throw new ArgumentException("SegmentType of source drawing layer can't be default drawing layer.");
            }

            IDrawingLayer sourceDrawingLayer = null;

            var drawingLayers = new List<IDrawingLayer>();
            foreach (var drawingLayer in drawingLayersOrder) {

                if (drawingLayer.GetType() == typeof(TSourceDrawingLayer)) {

                    if (sourceDrawingLayer != null) {
                        throw new ArgumentException("Repeated occurence of same type of source layer not allowed. For more instances of same types, use overload with instance parameter.");
                    }
                    sourceDrawingLayer = drawingLayer;
                }
                drawingLayers.Add(drawingLayer);
            }

            if (sourceDrawingLayer == null) {
                throw new ArgumentException($"Instance of type of source drawing layer is not present in {nameof(drawingLayersOrder)}.");
            }

            return OrderByLayersSanitized(visuals, drawingLayers, sourceDrawingLayer);
        }

        private static IEnumerable<TVisualType> OrderByLayersSanitized<TVisualType>(this IEnumerable<TVisualType> visuals, IEnumerable<IDrawingLayer> drawingLayersOrder, IDrawingLayer sourceLayer)
            where TVisualType : IVisual {

            var orderedVisualsByLayer = drawingLayersOrder.ToDictionary<IDrawingLayer, IDrawingLayer, List<TVisualType>>(s => s, _ => null);
            foreach (var visual in visuals) {

                var layer = visual.CurrentDrawingLayer;
                if (layer is DefaultDrawingLayer) {
                    layer = sourceLayer;
                }

                try {
                    if (orderedVisualsByLayer[layer] == null) orderedVisualsByLayer[layer] = new List<TVisualType>();
                } catch (KeyNotFoundException) {
                    throw new ArgumentException($"{nameof(IVisual.CurrentDrawingLayer)} { visual.CurrentDrawingLayer } of {visual} was not present in order definition { nameof(drawingLayersOrder)}.");
                }
                orderedVisualsByLayer[layer].Add(visual);
            }

            foreach (var layer in orderedVisualsByLayer) {

                if (layer.Value == null) continue;
                foreach (var visual in layer.Value) {
                    yield return visual;
                }
            }
        }
    }
}
