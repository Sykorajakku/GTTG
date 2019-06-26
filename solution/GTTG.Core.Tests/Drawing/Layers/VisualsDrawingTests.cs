using System.Collections.Generic;
using SkiaSharp;
using Xunit;

using GTTG.Core.Base;
using GTTG.Core.Drawing.Canvases;
using GTTG.Core.Drawing.Layers;

namespace GTTG.Core.Tests.Drawing.Layers {

    public class VisualsDrawingTests {

        private readonly Visual _visualA;
        private readonly Visual _visualB;
        private readonly Visual _visualC;
        private readonly Visual _visualD;

        public VisualsDrawingTests() { // called for each fact

            var orderedCallbacks = new List<Visual>();
            _visualA = new TestVisual(orderedCallbacks);
            _visualB = new TestVisual(orderedCallbacks);
            _visualC = new TestVisual(orderedCallbacks);
            _visualD = new TestVisual(orderedCallbacks);
        }

        [Fact]
        [Trait("Req.no", "DL9")]
        public void VisualInitializedToDefaultDrawingLayer() {
            Assert.Equal(DefaultDrawingLayer.Get, _visualA.CurrentDrawingLayer);
        }

        [Fact]
        [Trait("Req.no", "DL10")]
        public void NoDrawingLayerStackPopUnderflow() {

            _visualA.PopDrawingLayer();
            _visualA.PopDrawingLayer();
            Assert.Equal(DefaultDrawingLayer.Get, _visualA.CurrentDrawingLayer);
        }
    }

    public class TestVisual : Visual {

        private readonly List<Visual> _callbackList;

        public TestVisual(List<Visual> callbackList) {
            _callbackList = callbackList;
        }

        protected override void OnDraw(DrawingCanvas drawingCanvas) {
            _callbackList.Add(this);
        }

        public override bool HasHit(SKPoint contentPoint) {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<IVisual> ProvideVisuals() {
            throw new System.NotImplementedException();
        }
    }
}
