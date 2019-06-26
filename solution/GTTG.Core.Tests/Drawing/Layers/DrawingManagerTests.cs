// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Moq;
using SkiaSharp;
using Xunit;

using GTTG.Core.Base;
using GTTG.Core.Drawing.Canvases;
using GTTG.Core.Drawing.Layers;

namespace GTTG.Core.Tests.Drawing.Layers {

    public class DrawingManagerTests {

        private readonly IRegisteredLayersOrder _drawingLayerOrder;
        private readonly ICanvasFactory _canvasFactory;

        public DrawingManagerTests() {

            var viewProvider = new Mock<ICanvasFactory>();
            var drawingLayerOrder =  new Mock<IRegisteredLayersOrder>();
            drawingLayerOrder
                .Setup(e => e.DrawingLayerTypeList)
                .Returns(ImmutableList.CreateRange(new List<Type> { typeof(SecondDrawingLayer), typeof(FirstDrawingLayer) }));

            _drawingLayerOrder = drawingLayerOrder.Object;
            _canvasFactory = viewProvider.Object;
        }

        [Fact]
        [Trait("Req.no", "DL1")]
        public void PlacesRegistrationsInCorrectOrder() {

            var drawingManager = new DrawingManager(_canvasFactory, _drawingLayerOrder);
            
            var firstDrawingLayer = new FirstDrawingLayer();
            var secondDrawingLayer = new SecondDrawingLayer();

            drawingManager.ReplaceRegisteredDrawingLayer(firstDrawingLayer);
            drawingManager.ReplaceRegisteredDrawingLayer(secondDrawingLayer);
            
            Assert.Equal(secondDrawingLayer, drawingManager.Layers[0].DrawingLayer);
            Assert.Equal(firstDrawingLayer, drawingManager.Layers[1].DrawingLayer);
        }

        [Fact]
        [Trait("Req.no", "DL2")]
        public void PlacesRegistrationsInCorrectOrderWithUnregisteredLayer() {

            var drawingManager = new DrawingManager(_canvasFactory, _drawingLayerOrder);

            var unregisteredLayer = new UnregisteredDrawingLayer();
            drawingManager.Insert(1, unregisteredLayer);
            
            Assert.Equal(unregisteredLayer, drawingManager.Layers[1].DrawingLayer);
            Assert.False(drawingManager.Layers[1].IsRegistered);
        }

        [Fact]
        [Trait("Req.no", "DL3")]
        public void UnregisteredAndRegisteredSameInstanceInManager() {

            var drawingManager = new DrawingManager(_canvasFactory, _drawingLayerOrder);
            var drawingLayerInstance = new FirstDrawingLayer();
            
            drawingManager.ReplaceRegisteredDrawingLayer(drawingLayerInstance);
            drawingManager.AddOnCurrentTop(drawingLayerInstance);

            Assert.Equal(drawingLayerInstance, drawingManager.Layers[1].DrawingLayer);
            Assert.Equal(drawingLayerInstance, drawingManager.Layers[2].DrawingLayer);
            Assert.False(drawingManager.Layers[2].IsRegistered);
        }

        [Fact]
        [Trait("Req.no", "DL4")]
        public void MultipleRegistrationsOfSameType() {

            var sameTypeMultipleRegistrationsLayerOrder = new Mock<IRegisteredLayersOrder>();
            sameTypeMultipleRegistrationsLayerOrder
                .Setup(s => s.DrawingLayerTypeList)
                .Returns(ImmutableList.CreateRange(new List<Type> {typeof(SecondDrawingLayer), typeof(FirstDrawingLayer), typeof(SecondDrawingLayer)}));

            var drawingManager = new DrawingManager(_canvasFactory, sameTypeMultipleRegistrationsLayerOrder.Object);
            
            var secondDrawingLayer = new SecondDrawingLayer();
            drawingManager.ReplaceRegisteredDrawingLayer(secondDrawingLayer, 1);

            Assert.Equal(DefaultDrawingLayer.Get, drawingManager.Layers[0].DrawingLayer);
            Assert.Equal(secondDrawingLayer, drawingManager.Layers[2].DrawingLayer);
        }

        [Fact]
        [Trait("Req.no", "DL5")]
        public void SubtypeRegistration() {

            var drawingManager = new DrawingManager(_canvasFactory, _drawingLayerOrder);

            var subtypeOfFirstDrawingLayer = new DerivedFromFirstDrawingLayer();
            drawingManager.ReplaceRegisteredDrawingLayer(subtypeOfFirstDrawingLayer);

            Assert.Equal(subtypeOfFirstDrawingLayer, drawingManager.Layers[1].DrawingLayer);
        }

        [Fact]
        [Trait("Req.no", "DL6")]
        public void DoesNotRemoveButReplaceRegistrationWithDefaultOnRemove() {

            var drawingManager = new DrawingManager(_canvasFactory, _drawingLayerOrder);
            var firstLayer = new FirstDrawingLayer();
            var secondLayer = new SecondDrawingLayer();

            drawingManager.ReplaceRegisteredDrawingLayer(firstLayer);
            drawingManager.ReplaceRegisteredDrawingLayer(secondLayer);
            drawingManager.RemoveDrawingLayer(0);

            Assert.Equal(DefaultDrawingLayer.Get, drawingManager.Layers[0].DrawingLayer);
            Assert.True(drawingManager.Layers[0].IsRegistered);
        }

        [Fact]
        [Trait("Req.no", "DL7")]
        public void DrawsInCorrectOrder() {

            var actualOrder = new List<IDrawingLayer>();

            var surface = SKSurface.Create(new SKImageInfo(10, 10, SKColorType.Alpha8, SKAlphaType.Opaque));

            var firstDrawingLayer = new Mock<IDrawingLayer>();
            firstDrawingLayer
                .Setup(c => c.Draw(It.IsAny<DrawingCanvas>()))
                .Callback(() => actualOrder.Add(firstDrawingLayer.Object));

            var secondDrawingLayer = new Mock<IDrawingLayer>();
            secondDrawingLayer
                .Setup(c => c.Draw(It.IsAny<DrawingCanvas>()))
                .Callback(() => actualOrder.Add(secondDrawingLayer.Object));

            var expectedOrder = new List<IDrawingLayer> { firstDrawingLayer.Object, secondDrawingLayer.Object };

            var layersOrder = new Mock<IRegisteredLayersOrder>();
            layersOrder
                .Setup(c => c.DrawingLayerTypeList)
                .Returns(ImmutableList.CreateRange(expectedOrder.Select(d => d.GetType())));

            var drawingManager = new DrawingManager(_canvasFactory, layersOrder.Object);
            drawingManager.ReplaceRegisteredDrawingLayer(firstDrawingLayer.Object, 0);
            drawingManager.ReplaceRegisteredDrawingLayer(secondDrawingLayer.Object, 1);

            drawingManager.Draw(surface);

            Assert.Equal(expectedOrder, actualOrder);
        }

        [Fact]
        [Trait("Req.no", "DL8")]
        public void DrawsSameLayerMultipleTimes() {

            var drawingLayer = new Mock<IDrawingLayer>();
            var drawingLayerOrder = new Mock<IRegisteredLayersOrder>();
            drawingLayerOrder.Setup(o => o.DrawingLayerTypeList).Returns(ImmutableList.Create(drawingLayer.Object.GetType()));

            var surface = SKSurface.Create(new SKImageInfo(10, 10, SKColorType.Alpha8, SKAlphaType.Opaque));
            var drawingManager = new DrawingManager(_canvasFactory, drawingLayerOrder.Object);

            drawingManager.ReplaceRegisteredDrawingLayer(drawingLayer.Object);
            drawingManager.AddOnCurrentBottom(drawingLayer.Object);

            drawingManager.Draw(surface);

            drawingLayer.Verify(c => c.Draw(It.IsAny<DrawingCanvas>()), Times.Exactly(2));
        }
    }

    public class FirstDrawingLayer : ContentDrawingLayer {
        protected override void OnDraw(DrawingCanvas drawingCanvas) {
        }

        public override IEnumerable<IVisual> ProvideVisuals() {
            throw new NotImplementedException();
        }
    }
    public class DerivedFromFirstDrawingLayer : FirstDrawingLayer { }

    public class SecondDrawingLayer : ContentDrawingLayer {
        protected override void OnDraw(DrawingCanvas drawingCanvas) {
        }

        public override IEnumerable<IVisual> ProvideVisuals() {
            throw new NotImplementedException();
        }
    }
    public class UnregisteredDrawingLayer : ContentDrawingLayer {
        protected override void OnDraw(DrawingCanvas drawingCanvas) {
        }

        public override IEnumerable<IVisual> ProvideVisuals() {
            throw new NotImplementedException();
        }
    }
}
