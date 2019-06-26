// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using SkiaSharp;

namespace GTTG.Model.Lines {

    /// <summary>
    /// Line with modifiable stroke width.
    /// </summary>
    public class LinePaint {

        /// <summary>
        /// Default stroke width.
        /// </summary>
        public float DesiredStrokeWidth { get; protected set; }

        /// <summary>
        /// Actual stroke width.
        /// </summary>
        public float ArrangedStrokeWidth { get; protected set; }

        /// <summary>
        /// Wrapped paint with modified stroke width.
        /// </summary>
        public SKPaint Paint { get; protected set; }

        /// <summary>
        /// Creates line with desired size from paint by <see cref="SKPaint.Clone"/>.
        /// </summary>
        /// <param name="desiredStrokeWidth">Desired height of line.</param>
        /// <param name="paint">Paint with stroke width to modify.</param>
        public LinePaint(float desiredStrokeWidth, SKPaint paint) {

            DesiredStrokeWidth = desiredStrokeWidth;
            ArrangedStrokeWidth = desiredStrokeWidth;
            Paint = paint.Clone();
            Paint.StrokeWidth = DesiredStrokeWidth;
        }

        /// <summary>
        /// Creates line and paint with desired size from color.
        /// </summary>
        /// <param name="desiredStrokeWidth">Desired height of line.</param>
        /// <param name="color">Color set to paint.</param>
        public LinePaint(float desiredStrokeWidth, SKColor color) {

            DesiredStrokeWidth = desiredStrokeWidth;
            ArrangedStrokeWidth = desiredStrokeWidth;
            Paint = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = desiredStrokeWidth, Color = color };
        }

        /// <summary>
        /// Assigns new value to actual stroke width <see cref="ArrangedStrokeWidth"/>.
        /// </summary>
        /// <param name="height">Arranged stroke width to use. Modifies <see cref="Paint"/> stroke width.</param>
        public void Arrange(float height) {
            ArrangedStrokeWidth = height;
            Paint.StrokeWidth = height;
        }

        /// <summary>
        /// Measures desired stroke width.
        /// </summary>
        /// <returns>Value of <see cref="DesiredStrokeWidth"/>.</returns>
        public float Measure() {
            return DesiredStrokeWidth;
        }

        /// <summary>
        /// Creates new <see cref="LinePaint"/>.
        /// </summary>
        /// <returns><see cref="LinePaint"/> with cloned value and instance of <see cref="Paint"/></returns>
        public LinePaint Clone() {
            return new LinePaint(DesiredStrokeWidth, Paint);
        }
    }
}
