// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Drawing.Canvases;

namespace GTTG.Model.Strategies {

    /// <summary>
    /// Flag for <see cref="Container"/> to determine order of added elements.
    /// </summary>
    public enum ElementsOrder {
        
        /// <summary>
        /// Adds elements from the left.
        /// </summary>
        FirstFromLeft,
        
        /// <summary>
        /// Adds elements from the right.
        /// </summary>
        FirstFromRight
    }

    /// <summary>
    /// Groups multiple <see cref="ViewElement"/> into one container to positioned in strategy.
    /// </summary>
    public class Container : ViewElement {

        /// <summary>
        /// Determines from which side are elements added.
        /// </summary>
        public ElementsOrder ElementsOrder { get; }

        /// <summary>
        /// Elements in container.
        /// </summary>
        protected readonly List<ViewElement> Components;

        /// <summary>
        /// Creates empty container with determined order for adding elements.
        /// </summary>
        public Container(ElementsOrder elementsOrder) {

            Components = new List<ViewElement>();
            ElementsOrder = elementsOrder;
        }

        /// <summary>
        /// Adds element to container.
        /// </summary>
        public virtual void AddComponent(ViewElement element) {
            Components.Add(element);
        }
        
        /// <summary>
        /// Measure width as sum of widths of all elements in container. Height is equal to maximal height from elements.
        /// </summary>
        protected override SKSize MeasureOverride(SKSize availableSize) {

            float width = 0;
            float height = 0;

            int index = 0;
            for (int count = Components.Count; index < count; ++index) {

                var component = Components[index];
                component.Measure(new SKSize(float.PositiveInfinity, float.PositiveInfinity));
                height = Math.Max(component.DesiredSize.Height, height);
                width += component.DesiredSize.Width;
            }

            return new SKSize(width, height);
        }

        /// <summary>
        /// Arranges elements in container. As managed by strategy, expects same size as DesiredSize.
        /// </summary>
        protected override SKSize ArrangeOverride(SKSize finalSize) {

            float currentComponentStart = 0;

            int index = 0;
            for (int count = Components.Count; index < count; ++index) {

                var component = Components[index];
                component.Arrange(new SKPoint(currentComponentStart, 0), component.DesiredSize, this);
                currentComponentStart += component.DesiredSize.Width;
            }
            return DesiredSize;
        }
        
        /// <inheritdoc/>
        protected override void OnDraw(DrawingCanvas drawingCanvas) {

            var count = Components.Count;
            for (var index = 0; index < count; index++) {
                drawingCanvas.Draw(Components[index]);
            }
        }

        /// <inheritdoc/>
        public override IEnumerable<IVisual> ProvideVisuals() {
            return Components;
        }
    }
}
