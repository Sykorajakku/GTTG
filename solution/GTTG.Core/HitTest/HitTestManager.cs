// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Drawing.Layers;

namespace GTTG.Core.HitTest {

    /// <summary>Provides tools for hit testing of visual elements.</summary>
    public class HitTestManager {

        private static readonly HitTestFilterCallback DefaultFilterCallback = (_, __) => HitTestFilterBehavior.Continue;
        private static readonly HitTestResultCallback DefaultResultCallback = _ => HitTestResultBehavior.Continue;
        private static readonly Func<IVisual, IEnumerable<IVisual>> SameLayerProvider = v => v.ProvideVisualsInSameLayer();
        private static readonly Func<IVisual, IEnumerable<IVisual>> AllVisualsProvider = v => v.ProvideVisuals();

        private readonly DrawingManager _drawingManager;

        /// <summary>Creates hit test manager with provided drawing layers order.</summary>
        /// <param name="drawingManager">Drawing manager with layers for <see cref="DrawingLayers"/> to enumerate.</param>
        public HitTestManager(DrawingManager drawingManager) {
            _drawingManager = drawingManager;
        }

        /// <summary>Provides order of drawing layers in visual order with undermost one being enumerated as first.</summary>
        public IEnumerable<IDrawingLayer> DrawingLayers => _drawingManager.GetDrawingLayersFromUndermostOne();

        /// <summary>Hit tests a element tree with provided element as root to find elements with positive hit test by enumerating it's children with <see cref="IVisual.ProvideVisuals"/>.</summary>
        /// <param name="target">Target as root of hit tested tree.</param>
        /// <param name="contentPoint">Point in <see cref="Drawing.Canvases.ContentDrawingCanvas"/> tested for hit.</param>
        /// <param name="resultTraversalOrder">Determines if element should be first or last from order defined in default tree traversal.</param>
        /// <returns>First hit tested element defined by <paramref name="resultTraversalOrder"/>. By default, last.</returns>
        public static IVisual HitTest(IVisual target, SKPoint contentPoint, ResultTraversalOrder resultTraversalOrder = ResultTraversalOrder.Last) {

            IVisual result = null;

            var callbackBehaviour = resultTraversalOrder == ResultTraversalOrder.First
                ? HitTestResultBehavior.Stop
                : HitTestResultBehavior.Continue;

            HitTestResultBehavior ResultCallback(IVisual element) {
                result = element;
                return callbackBehaviour;
            }

            HitTestInternal(target, null, ResultCallback, contentPoint, AllVisualsProvider);
            return result;
        }

        /// <summary>
        /// Hit tests tree of <see cref="IVisual"/> elements, with provided callbacks for tree pruning and positive hit tests. Order of tree traversal is defined as follows:
        /// Apply <paramref name="filterCallback"/> on root element. If tree should be processed, hit tests the root element.
        /// If the root element is positive on the hit-test, first process the root element and then children enumerated from <see cref="IVisual.ProvideVisuals"/> (considering applied filters).
        /// Else return from the tree traversal.
        /// </summary>
        /// <param name="hitTestRoot">Root of element tree also hit tested by default.</param>
        /// <param name="filterCallback">Called when element is found traversing the tree.</param>
        /// <param name="resultCallback">Called when hit test on element in tree is positive.</param>
        /// <param name="contentPoint">Point in <see cref="Drawing.Canvases.ContentDrawingCanvas"/> tested for hit.</param>
        public static void HitTest(IVisual hitTestRoot, HitTestFilterCallback filterCallback, HitTestResultCallback resultCallback, SKPoint contentPoint) {
            HitTestInternal(hitTestRoot, filterCallback, resultCallback, contentPoint, AllVisualsProvider);
        }

        /// <summary>
        /// Hit tests tree of <see cref="DrawingManager"/> of <see cref="ContentDrawingLayer"/> layers content with provided callbacks for tree pruning and positive hit tests. Order of tree traversal is defined as follows:
        /// Apply <paramref name="filterCallback"/> on root element. If tree should be processed, hit tests the root element.
        /// If the root element is positive on the hit-test, first process the root element and then children enumerated from <see cref="IVisual.ProvideVisualsInSameLayer"/> (considering applied filters).
        /// Else return from the tree traversal.
        /// </summary>
        /// <param name="filterCallback">Called when element is found traversing the tree.</param>
        /// <param name="resultCallback">Called when hit test on element in tree is positive.</param>
        /// <param name="contentPoint">Point in <see cref="Drawing.Canvases.ContentDrawingCanvas"/> tested for hit.</param>
        public void HitTest(HitTestFilterCallback filterCallback, HitTestResultCallback resultCallback, SKPoint contentPoint) {

            foreach (var layer in _drawingManager.GetDrawingLayersFromUndermostOne()) {

                if (layer is ContentDrawingLayer contentDrawingLayer) {
                    HitTestInternal(contentDrawingLayer, filterCallback, resultCallback, contentPoint, SameLayerProvider);
                }
            }
        }

        private static bool HitTestInternal(IVisual hitTestRoot,
                                            HitTestFilterCallback filterCallback,
                                            HitTestResultCallback resultCallback,
                                            SKPoint globalPoint,
                                            Func<IVisual, IEnumerable<IVisual>> provider) {

            filterCallback = filterCallback ?? DefaultFilterCallback;
            resultCallback = resultCallback ?? DefaultResultCallback;

            var filterCallbackResult = filterCallback(hitTestRoot, globalPoint);

            var cantCallChildrenHitTest = false;
            var cantCallRootCallback = false;

            switch (filterCallbackResult) {

                case HitTestFilterBehavior.Continue:
                    break;
                case HitTestFilterBehavior.ContinueSkipChildren:
                    cantCallChildrenHitTest = true;
                    break;
                case HitTestFilterBehavior.ContinueSkipSelf:
                    cantCallRootCallback = true;
                    break;
                case HitTestFilterBehavior.ContinueSkipSelfAndChildren:
                    return true;
                case HitTestFilterBehavior.Stop:
                    return false;
            }

            var isHit = hitTestRoot.HasHit(globalPoint);
            if (!isHit) return true;

            if (!cantCallRootCallback) {

                var result = resultCallback(hitTestRoot);
                if (result == HitTestResultBehavior.Stop) return false;
            }

            if (!cantCallChildrenHitTest) {

                foreach (var childViewElement in provider(hitTestRoot)) {

                    var canContinue = HitTestInternal(childViewElement, filterCallback, resultCallback, globalPoint, provider);
                    if (!canContinue) return false;
                }
            }

            return true;
        }
    }
}
