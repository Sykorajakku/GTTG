// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using GTTG.Core.Drawing.Canvases;

namespace GTTG.Core.Drawing.Layers {

    /// <summary>
    /// Singleton helper structure representing layer which is always positive on check in <see cref="Base.Visual"/>.
    /// </summary>
    public sealed class DefaultDrawingLayer : DrawingLayer {

        /// <summary>
        /// Gets singleton instance.
        /// </summary>
        public static DefaultDrawingLayer Get { get; }

        static DefaultDrawingLayer() {
            Get = new DefaultDrawingLayer();
        }

        private DefaultDrawingLayer() { }

        /// <inheritdoc />
        protected override void OnDraw(DrawingCanvas drawingCanvas) {
        }
    }
}
