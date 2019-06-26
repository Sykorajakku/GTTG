using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using SkiaSharp;
using Xunit;

using GTTG.Core.Base;
using GTTG.Core.Drawing.Canvases;
using GTTG.Core.Drawing.Layers;
using GTTG.Core.Extensions;

namespace GTTG.Core.Tests.HitTest {

    public class HitTestExtensionsTests {

        private readonly IDrawingLayer _firstDrawingLayer;
        private readonly IDrawingLayer _secondDrawingLayer;

        private readonly IVisual _visualInFirstDrawingLayer;
        private readonly IVisual _visualInSecondDrawingLayer;
        private readonly IVisual _visualInDefaultDrawingLayer;

        private readonly IVisual _hitTestPositiveFirstLayer;
        private readonly IVisual _hitTestPositiveSecondLayer;

        public HitTestExtensionsTests() {

            _firstDrawingLayer = new Mock<IDrawingLayer>().Object;
            _secondDrawingLayer = new Mock<IDrawingLayer>().Object;

            _visualInFirstDrawingLayer = InitElement(false, _firstDrawingLayer);
            _visualInSecondDrawingLayer = InitElement(false, _secondDrawingLayer);
            _visualInDefaultDrawingLayer = InitElement(false, DefaultDrawingLayer.Get);

            _hitTestPositiveFirstLayer = InitElement(true, _firstDrawingLayer);
            _hitTestPositiveSecondLayer = InitElement(true, _secondDrawingLayer);
        }

        private static IVisual InitElement(bool alwaysHit, IDrawingLayer currentDrawingLayer) {

            var mock = new Mock<IVisual>();
            mock.Setup(v => v.CurrentDrawingLayer).Returns(currentDrawingLayer);
            mock.Setup(v => v.HasHit(It.IsAny<SKPoint>())).Returns(alwaysHit);
            return mock.Object;
        }

        [Fact]
        [Trait("Req.no", "HT1")]
        public void TreatsElementsDefaultDrawingLayersAsProvidedSourceDrawingLayer() {

            var drawingLayersOrder = new List<IDrawingLayer> {
                _firstDrawingLayer,
                _secondDrawingLayer
            };

            var visuals = new List<IVisual> { _visualInDefaultDrawingLayer, _visualInFirstDrawingLayer, _visualInSecondDrawingLayer };

            var result = visuals.OrderByLayers(drawingLayersOrder, _secondDrawingLayer);
            var expected = new List<IVisual> { _visualInFirstDrawingLayer, _visualInDefaultDrawingLayer, _visualInSecondDrawingLayer };

            Assert.Equal(expected, result);
        }

        [Fact]
        [Trait("Req.no", "HT2")]
        public void ThrowsOnDefaultDrawingLayerAssignedToSourceDrawingLayerInstance() {

            var drawingLayerOrder = new List<IDrawingLayer> {
                _firstDrawingLayer,
                _secondDrawingLayer
            };

            var visuals = new List<IVisual>();
            Assert.Throws<ArgumentException>(() => visuals.OrderByLayers(drawingLayerOrder, DefaultDrawingLayer.Get));
        }

        [Fact]
        [Trait("Req.no", "HT3")]
        public void ThrowsOnDefaultDrawingLayerAssignedToSourceDrawingLayerType() {

            var drawingLayerOrder = new List<IDrawingLayer> {
                _firstDrawingLayer,
                _secondDrawingLayer
            };

            var visuals = new List<IVisual>();
            Assert.Throws<ArgumentException>(() =>
                visuals.OrderByLayers<IVisual, DefaultDrawingLayer>(drawingLayerOrder));
        }

        [Fact]
        [Trait("Req.no", "HT4")]
        public void ThrowsOnTypeDrawingLayerSourceRegistrationWithItsMultipleOccurenceInOrder() {

            var drawingLayerOrder = new List<IDrawingLayer> {
                _firstDrawingLayer,
                new SpecificDrawingLayer(),
                _secondDrawingLayer,
                _firstDrawingLayer,
                new SpecificDrawingLayer()
            };

            var visuals = new List<IVisual>
                {_visualInSecondDrawingLayer, _visualInSecondDrawingLayer, _visualInFirstDrawingLayer};

            Assert.Throws<ArgumentException>(() =>
                visuals.OrderByLayers<IVisual, SpecificDrawingLayer>(drawingLayerOrder));
        }

        [Fact]
        [Trait("Req.no", "HT5")]
        public void AllowsMultipleSameTypeLayersUsingSourceAsInstance() {

            var specificDrawingLayer = new SpecificDrawingLayer();

            var specificLayerVisual = new Mock<IVisual>();
            specificLayerVisual.Setup(v => v.CurrentDrawingLayer).Returns(specificDrawingLayer);

            var drawingLayerOrder = new List<IDrawingLayer> {
                specificDrawingLayer,
                _secondDrawingLayer,
                new SpecificDrawingLayer()
            };

            var visuals = new List<IVisual>
                { specificLayerVisual.Object, _visualInSecondDrawingLayer };

            var actualResult = visuals.OrderByLayers(drawingLayerOrder, specificDrawingLayer);
            var expectedResult = visuals;

            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        [Trait("Req.no", "HT6")]
        public void ElementCurrentDrawingLayerNotPresentInLayers() {

            var otherDrawingLayerElement = new Mock<IVisual>();
            otherDrawingLayerElement.Setup(c => c.CurrentDrawingLayer).Returns(new SpecificDrawingLayer());

            var drawingLayerOrder = new List<IDrawingLayer> {
                _firstDrawingLayer,
                _secondDrawingLayer,
            };

            var visuals = new List<IVisual>
                {_visualInSecondDrawingLayer, _visualInSecondDrawingLayer, _visualInFirstDrawingLayer, otherDrawingLayerElement.Object };

            Assert.Throws<ArgumentException>(() => {
                 var result = visuals.OrderByLayers(drawingLayerOrder, _firstDrawingLayer).ToList();
            });
        }

        [Fact]
        [Trait("Req.no", "HT7")]
        public void ThrowsIfSourceLayerIsNotPresentInManager() {

            var drawingLayerOrder = new List<IDrawingLayer> {
                _firstDrawingLayer,
                _secondDrawingLayer
            };

            var visuals = new List<IVisual>();

            Assert.Throws<ArgumentException>(() => 
                visuals.OrderByLayers(drawingLayerOrder, new SpecificDrawingLayer()));
        }

        [Fact]
        [Trait("Req.no", "HT8")]
        public void ExpectsToReturnHitTestPositiveInLayerOrder() {

            var drawingLayerOrder = new List<IDrawingLayer> {
                _firstDrawingLayer,
                _secondDrawingLayer
            };

            var visuals = new List<IVisual> {
                _visualInSecondDrawingLayer, _hitTestPositiveSecondLayer, _visualInDefaultDrawingLayer, _hitTestPositiveFirstLayer
            };

            var hitTestOrderedByLayers = visuals.HitTest(SKPoint.Empty).OrderByLayers(drawingLayerOrder, _secondDrawingLayer);

            var expectedHitTestPositive = new List<IVisual> { _hitTestPositiveFirstLayer, _hitTestPositiveSecondLayer };
            Assert.Equal(expectedHitTestPositive, hitTestOrderedByLayers);
        }

        [Fact]
        [Trait("Req.no", "HT9")]
        public void ExpectsToReturnHitTestCorrectly() {

            var hitTestPositiveFirstLayer = new Mock<IVisual>();
            hitTestPositiveFirstLayer.Setup(v => v.CurrentDrawingLayer).Returns(_firstDrawingLayer);
            hitTestPositiveFirstLayer.Setup(v => v.HasHit(It.IsAny<SKPoint>())).Returns(true);

            var hitTestPositiveSecondLayer = new Mock<IVisual>();
            hitTestPositiveSecondLayer.Setup(v => v.CurrentDrawingLayer).Returns(_secondDrawingLayer);
            hitTestPositiveSecondLayer.Setup(v => v.HasHit(It.IsAny<SKPoint>())).Returns(true);

            var visuals = new List<IVisual> {
                _visualInSecondDrawingLayer, hitTestPositiveSecondLayer.Object, _visualInDefaultDrawingLayer, hitTestPositiveFirstLayer.Object
            };

            var actualHitTestPositive = visuals.HitTest(SKPoint.Empty);
            var expectedHitTestPositive = new List<IVisual> { hitTestPositiveSecondLayer.Object, hitTestPositiveFirstLayer.Object };

            Assert.Equal(expectedHitTestPositive, actualHitTestPositive);
        }

        private class SpecificDrawingLayer : IDrawingLayer {
            public void Draw(DrawingCanvas drawingCanvas) {
            }
        }
    }
}
