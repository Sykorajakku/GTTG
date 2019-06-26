using System;
using SkiaSharp;
using Xunit;

using GTTG.Core.Utils;

namespace GTTG.Core.Tests.Placement {

    public class AnglePlacementTests {

        [Fact]
        public void RadAngleOnSameVector() {

            var vector = new SKPoint(1, 0);
            Assert.Equal(0, PlacementUtils.ComputeAcuteRadAngle(vector, vector));
        }

        [Fact]
        public void RadAngleOnPerpendicular() {

            Assert.Equal(Math.PI / 2, PlacementUtils.ComputeAcuteRadAngle(new SKPoint(0, -1), new SKPoint(1, 0)));
        }

        [Fact]
        public void MoveInSegmentTest1() {

            var origin = SKPoint.Empty;
            var length = 2 * (float) Math.Sqrt(2);
            var translatedOrigin = PlacementUtils.MoveInLine(new SKPoint(1, 1), origin, length);

            Assert.Equal(2, translatedOrigin.Y, 1);
            Assert.Equal(2, translatedOrigin.X, 1);
        }

        [Fact]
        public void MoveInSegmentTest2() {

            var origin = SKPoint.Empty;
            var length = 2 * (float) Math.Sqrt(2);
            var translatedOrigin = PlacementUtils.MoveInLine(new SKPoint(-1, -1), origin, length);

            Assert.Equal(-2, translatedOrigin.Y, 1);
            Assert.Equal(-2, translatedOrigin.X, 1);
        }

        [Fact]
        public void MoveInSegmentTest3() {

            var origin = SKPoint.Empty;
            var length = 5;
            var translatedOrigin = PlacementUtils.MoveInLine(new SKPoint(0, -1), origin, length);

            Assert.Equal(-5, translatedOrigin.Y, 1);
            Assert.Equal(0, translatedOrigin.X, 1);
        }
    }
}
