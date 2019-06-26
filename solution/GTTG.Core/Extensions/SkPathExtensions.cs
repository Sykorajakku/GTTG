// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using GTTG.Core.Utils;
using SkiaSharp;

namespace GTTG.Core.Extensions {
    
    /// <summary>
    /// Extensions method for <see cref="SKPath"/>.
    /// </summary>
    public static class SkPathExtensions {

        /// <summary>Calculates distance of <see cref="SKPath"/> from <see cref="SKPoint"/> by finding closest point on the path relative to the point.</summary>
        public static float CalculateDistanceFromPath(this SKPath path, SKPoint point) {

            if (path.IsEmpty) {
                return float.MaxValue;
            }
            if (path.Points.Length == 1) {
                return PlacementUtils.ComputesVectorLength(point - path.Points[0]);
            }

            var minDistance = float.MaxValue;

            for (var i = 0; i < path.PointCount; ++i) {

                if (i == path.PointCount - 1) { // last point with no following point i + 1
                    break;
                }
                else { // line segment [i, i+1]
                    var distance = CalculateDistanceFromSegment(path.Points[i], path.Points[i + 1], point);
                    if (distance < minDistance) {
                        minDistance = distance;
                    }
                }
            }

            return minDistance;
        }

        private static float CalculateDistanceFromSegment(SKPoint firstSegmentPoint, SKPoint secondSegmentPoint, SKPoint point) {

            var squaredLength = DistanceSquared(firstSegmentPoint - secondSegmentPoint);
            if (firstSegmentPoint == secondSegmentPoint) {
                return PlacementUtils.ComputesVectorLength(firstSegmentPoint - point);
            }

            var u = point - firstSegmentPoint;
            var v = secondSegmentPoint - firstSegmentPoint;
            var t = Math.Max(0, Math.Min(1, (u.X * v.X + u.Y * v.Y) / squaredLength));
            var projection = firstSegmentPoint + new SKPoint(v.X * t, v.Y * t);

            return PlacementUtils.ComputesVectorLength(point - projection);
        }

        private static float DistanceSquared(SKPoint vector) {
            return (vector.X * vector.X + vector.Y * vector.Y);
        }
    }
}
