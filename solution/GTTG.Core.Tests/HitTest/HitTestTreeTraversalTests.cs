using System.Collections.Generic;
using System.Linq;
using Moq;
using SkiaSharp;
using Xunit;

using GTTG.Core.Base;
using GTTG.Core.HitTest;

namespace GTTG.Core.Tests.HitTest {

    public class HitTestTreeTraversalTests {

        private readonly Mock<IVisual> _hitTestRoot;
        private readonly Mock<IVisual> _hitTestA;
        private readonly Mock<IVisual> _hitTestB;
        private readonly Mock<IVisual> _hitTestC;

        public HitTestTreeTraversalTests() {

            _hitTestA = SetupHitTestMock(false, Enumerable.Empty<IVisual>(), nameof(_hitTestA));
            _hitTestB = SetupHitTestMock(true, Enumerable.Empty<IVisual>(), nameof(_hitTestB));
            _hitTestC = SetupHitTestMock(true, new List<IVisual> { _hitTestA.Object, _hitTestB.Object }, nameof(_hitTestC));
            _hitTestRoot = SetupHitTestMock(true, new List<IVisual> {_hitTestC.Object }, nameof(_hitTestRoot));
        }

        private Mock<IVisual> SetupHitTestMock(bool isHitTestPositive, IEnumerable<IVisual> target, string debugToStringName) {
            
            var hitTestMock = new Mock<IVisual>();
            hitTestMock.Setup(h => h.HasHit(It.IsAny<SKPoint>())).Returns(isHitTestPositive);
            hitTestMock.Setup(h => h.ProvideVisualsInSameLayer()).Returns(target);
            hitTestMock.Setup(h => h.ProvideVisuals()).Returns(target);
            hitTestMock.Setup(h => h.ToString()).Returns(debugToStringName);
            return hitTestMock;
        }

        [Fact]
        [Trait("Req.no", "HT10")]
        public void TraverseInExpectedOrder() {

            var filterOrder = new List<IVisual>();
            var resultOrder = new List<IVisual>();

            HitTestFilterBehavior FilterCallback(IVisual target, SKPoint _) {
                filterOrder.Add(target);
                return HitTestFilterBehavior.Continue;
            }

            HitTestResultBehavior ResultCallback(IVisual target) {
                resultOrder.Add(target);
                return HitTestResultBehavior.Continue;
            }

            HitTestManager.HitTest(_hitTestRoot.Object, FilterCallback, ResultCallback, SKPoint.Empty);

            var expectedFilterOrder = new List<IVisual> { _hitTestRoot.Object, _hitTestC.Object, _hitTestA.Object, _hitTestB.Object };
            var expectedResultOrder = new List<IVisual> { _hitTestRoot.Object, _hitTestC.Object, _hitTestB.Object };

            Assert.Equal(expectedFilterOrder, filterOrder);
            Assert.Equal(expectedResultOrder, resultOrder);
        }

        [Fact]
        [Trait("Req.no", "HT11")]
        public void ReturnsLastHitTestPositive() {

            _hitTestA.Setup(h => h.HasHit(It.IsAny<SKPoint>())).Returns(true);

            var result = HitTestManager.HitTest(_hitTestRoot.Object, SKPoint.Empty, ResultTraversalOrder.Last);

            Assert.Equal(_hitTestB.Object, result);
        }

        [Fact]
        [Trait("Req.no", "HT12")]
        public void BehavesCorrectlyOnFilterStop() {

            var actualResult = new List<IVisual>();

            HitTestFilterBehavior FilterCallback(IVisual target, SKPoint _) {

                if (target == _hitTestA.Object) {
                    return HitTestFilterBehavior.Stop;
                }
                else return HitTestFilterBehavior.Continue;
            }

            HitTestResultBehavior ResultCallback(IVisual target) {
                actualResult.Add(target);
                return HitTestResultBehavior.Continue;
            }

            HitTestManager.HitTest(_hitTestRoot.Object, FilterCallback, ResultCallback, SKPoint.Empty);
            var expectedResult = new List<IVisual> {_hitTestRoot.Object, _hitTestC.Object };

            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        [Trait("Req.no", "HT13")]
        public void BehavesCorrectlyOnFilterTreePruning() {

            var actualResult = new List<IVisual>();

            Mock<IVisual> childOfA = SetupHitTestMock(true, Enumerable.Empty<IVisual>(), nameof(childOfA));

            _hitTestA.Setup(h => h.HasHit(It.IsAny<SKPoint>())).Returns(true);
            _hitTestA.Setup(h => h.ProvideVisualsInSameLayer()).Returns(new List<IVisual> { childOfA.Object });

            HitTestFilterBehavior FilterCallback(IVisual target, SKPoint _) {

                if (target == childOfA.Object) {
                    return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
                } else return HitTestFilterBehavior.Continue;
            }

            HitTestResultBehavior ResultCallback(IVisual target) {
                actualResult.Add(target);
                return HitTestResultBehavior.Continue;
            }

            HitTestManager.HitTest(_hitTestRoot.Object, FilterCallback, ResultCallback, SKPoint.Empty);
            var expectedResult = new List<IVisual> { _hitTestRoot.Object, _hitTestC.Object, _hitTestA.Object, _hitTestB.Object };

            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        [Trait("Req.no", "HT14")]
        public void BehavesCorrectlyOnResultCallbackStop() {

            var actualList = new List<IVisual>();

            HitTestResultBehavior ResultCallback(IVisual target) {

                actualList.Add(target);

                if (_hitTestC.Object == target) {
                    return HitTestResultBehavior.Stop;
                }

                return HitTestResultBehavior.Continue;
            }

            HitTestManager.HitTest(_hitTestRoot.Object, null, ResultCallback, SKPoint.Empty);
            var expected = new List<IVisual> { _hitTestRoot.Object, _hitTestC.Object };

            Assert.Equal(expected, actualList);
        }

        [Fact]
        [Trait("Req.no", "HT15")]
        public void BehavesCorrectlyOnSkipSelfAndChildren() {

            var actualList = new List<IVisual>();
            Mock<IVisual> childOfA = SetupHitTestMock(true, Enumerable.Empty<IVisual>(), nameof(childOfA));

            HitTestFilterBehavior FilterCallback(IVisual target, SKPoint point) {

                if (_hitTestRoot.Object == target) { return HitTestFilterBehavior.ContinueSkipSelf; }
                if (_hitTestA.Object == target) { return HitTestFilterBehavior.ContinueSkipChildren; }
                return HitTestFilterBehavior.Continue;
            }

            HitTestResultBehavior ResultCallback(IVisual target) {

                actualList.Add(target);
                if (target == childOfA.Object) {
                    return HitTestResultBehavior.Stop;
                }

                return HitTestResultBehavior.Continue;
            }

            _hitTestA.Setup(h => h.HasHit(It.IsAny<SKPoint>())).Returns(true);
            _hitTestA.Setup(h => h.ProvideVisualsInSameLayer()).Returns(new List<IVisual> { childOfA.Object });

            HitTestManager.HitTest(_hitTestRoot.Object, FilterCallback, ResultCallback, SKPoint.Empty);
            var expected = new List<IVisual> { _hitTestC.Object, _hitTestA.Object, _hitTestB.Object };

            Assert.Equal(expected, actualList);
        }
    }
}
