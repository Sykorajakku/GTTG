// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using SkiaSharp;

using GTTG.Core.Strategies.Interfaces;

namespace GTTG.Core.Utils {

    /// <summary>
    /// Math functions used for <see cref="IStrategyDocker"/> implementation.
    /// </summary>
    public static class PlacementUtils {

        /// <summary>
        /// Computes cosine of two vectors.
        /// </summary>
        /// <param name="u">First vector.</param>
        /// <param name="v">Second vector.</param>
        /// <returns>Cosine value in radians.</returns>
        public static double ComputeCosine(SKPoint u, SKPoint v) {

            return (u.X * v.X + u.Y * v.Y) / (Math.Sqrt(Math.Pow(u.X, 2) + Math.Pow(u.Y, 2)) *
                                              Math.Sqrt(Math.Pow(v.X, 2) + Math.Pow(v.Y, 2)));
        }

        /// <summary>
        /// Computes position of intersection of line and horizontal line.
        /// </summary>
        /// <param name="vector">Vector which sets direction of the intersecting line.</param>
        /// <param name="vectorPoint">Point found on <paramref name="vector"/>, forming the intersecting line.</param>
        /// <param name="horizontalLineY">Vertical position of the horizontal line.</param>
        /// <returns>Point on the horizontal line.</returns>
        public static SKPoint ComputeHorizontalLineIntersection(SKPoint vector, SKPoint vectorPoint, float horizontalLineY) {
            return new SKPoint((vector.X * horizontalLineY + vector.Y * vectorPoint.X - vector.X * vectorPoint.Y) / vector.Y, horizontalLineY);
        }

        /// <summary>
        /// Computes acute angle of two vectors in radians.
        /// </summary>
        /// <param name="u">First vector.</param>
        /// <param name="v">Second vector.</param>
        /// <returns>Acute angle in radians.</returns>
        public static double ComputeAcuteRadAngle(SKPoint u, SKPoint v) {
            return Math.Acos(Math.Abs(ComputeCosine(u, v)));
        }

        /// <summary>
        /// Computes length of leg adjacent to acute angle of two vectors in perpendicular triangle.
        /// </summary>
        /// <param name="u">First vector forming the acute angle.</param>
        /// <param name="v">Second vector forming the acute angle.</param>
        /// <param name="opposedToAngleLegLength">Length of leg opposed to the formed angle.</param>
        /// <returns>Length of adjacent leg to the formed angle.</returns>
        public static float ComputeLegLength(SKPoint u, SKPoint v, float opposedToAngleLegLength) {

            var cosine = Math.Abs(ComputeCosine(u, v));
            var angle = Math.Acos(cosine);
            return (float) Math.Abs(opposedToAngleLegLength / Math.Tan(angle));
        }

        /// <summary>
        /// Computes length of hypotenuse in perpendicular triangle formed by acute angle of two vectors.
        /// </summary>
        /// <param name="u">First vector forming the acute angle.</param>
        /// <param name="v">Second vector forming the acute angle.</param>
        /// <param name="opposedToAngleLegLength">Length of the leg opposed to the formed angle.</param>
        /// <returns>Length of hypotenuse in formed triangle.</returns>
        public static float ComputeHypotenuseLength(SKPoint u, SKPoint v, float opposedToAngleLegLength) {

            var cosine = Math.Abs(ComputeCosine(u, v));
            var angle = Math.Acos(cosine);
            return (float) Math.Abs(opposedToAngleLegLength / Math.Sin(angle));
        }

        /// <summary>
        /// Moves point in line by specified length.
        /// </summary>
        /// <param name="vector">Vector describing the line; in which the translation of <paramref name="vectorPoint"/> happens.</param>
        /// <param name="vectorPoint">The point in line which is translated.</param>
        /// <param name="length">The length of translation.</param>
        /// <returns>Translated <paramref name="vectorPoint"/> by specified length.</returns>
        public static SKPoint MoveInLine(SKPoint vector, SKPoint vectorPoint, float length) {

            var angleX = ComputeAcuteRadAngle(vector, new SKPoint(1, 0));

            var xTranslation = (float) Math.Cos(angleX) * length;
            var yTranslation = (float) Math.Sin(angleX) * length;

            var x = vector.X < 0 ? vectorPoint.X - xTranslation : vectorPoint.X + xTranslation;
            var y = vector.Y < 0 ? vectorPoint.Y - yTranslation : vectorPoint.Y + yTranslation;
            return new SKPoint(x, y);
        }

        /// <summary>
        /// Computes diagonal of rectangle.
        /// </summary>
        /// <param name="rect">The rectangle whose diagonal is determined.</param>
        /// <returns>Diagonal of the <paramref name="rect"/>.</returns>
        public static float ComputeDiagonal(SKSize rect) {
            return (float) Math.Sqrt(rect.Width * rect.Width + rect.Height * rect.Height);
        }

        /// <summary>
        /// Computes length of vector.
        /// </summary>
        /// <param name="vector">The vector whose length is determined.</param>
        /// <returns>Length of <paramref name="vector"/>.</returns>
        public static float ComputesVectorLength(SKPoint vector) {
            return (float) Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
        }
    }
}