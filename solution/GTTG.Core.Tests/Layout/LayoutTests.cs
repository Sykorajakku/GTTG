using System;
using System.Collections.Generic;
using SkiaSharp;
using Xunit;

using GTTG.Core.Base;

namespace GTTG.Core.Tests.Layout {

    public class OuterViewElement : ViewElement {

        public TestElement InnerViewElement { get; }

        public OuterViewElement(TestElement innerViewElement) {
            InnerViewElement = innerViewElement;
        }

        protected override SKSize MeasureOverride(SKSize availableSize) {
            return availableSize;
        }

        protected override SKSize ArrangeOverride(SKSize finalSize) {
            
            InnerViewElement.Arrange(SKPoint.Empty, finalSize, this);
            return new SKSize(InnerViewElement.ArrangedWidth, InnerViewElement.ArrangedHeight); 
        }

        public override IEnumerable<IVisual> ProvideVisuals() {
            yield break;
        }
    }

    public class LayoutTests {

        [Fact]
        [Trait("Req.no", "VE1")]
        public void ArrangeWithScale() {

            var innerViewElement = new TestElement();
            var outerViewElement = new OuterViewElement(innerViewElement);

            outerViewElement.Measure(new SKSize(100, 100));
            outerViewElement.Arrange(SKPoint.Empty, outerViewElement.DesiredSize);

            var outerElementGlobalHeight = outerViewElement.ContentHeight;
            var outerElementGlobalWidth = outerViewElement.ContentWidth;
            var innerElementGlobalHeight = innerViewElement.ContentHeight;
            var innerElementGlobalWidth = innerViewElement.ContentWidth;

            outerViewElement.Scale(0.5f);

            Assert.Equal(outerElementGlobalWidth / 2, outerViewElement.ContentWidth);
            Assert.Equal(outerElementGlobalHeight / 2, outerViewElement.ContentHeight);
            Assert.Equal(innerElementGlobalWidth / 2, innerViewElement.ContentWidth);
            Assert.Equal(innerElementGlobalHeight / 2, innerViewElement.ContentHeight);
        }

        [Fact]
        [Trait("Req.no", "VE2")]
        public void ArrangeScaleThenRepositionScenario() {

            var innerViewElement = new TestElement();
            var outerViewElement = new OuterViewElement(innerViewElement);

            var size = new SKSize(100, 100);

            outerViewElement.Measure(size);
            outerViewElement.Arrange(SKPoint.Empty, outerViewElement.DesiredSize);

            var newOrigin = new SKPoint(100, 100);
            outerViewElement.Scale(0.5f);
            outerViewElement.Reposition(newOrigin);

            Assert.Equal(newOrigin, innerViewElement.ContentLeftTop);
            Assert.Equal(size.Width / 2, innerViewElement.ContentWidth);
        }

        [Fact]
        [Trait("Req.no", "VE3")]
        public void ArrangeRepositionThenScaleScenario() {

            var innerViewElement = new TestElement();
            var outerViewElement = new OuterViewElement(innerViewElement);

            var size = new SKSize(100, 100);

            outerViewElement.Measure(size);
            outerViewElement.Arrange(SKPoint.Empty, outerViewElement.DesiredSize);

            var newOrigin = new SKPoint(100, 100);
            outerViewElement.Reposition(newOrigin);
            outerViewElement.Scale(0.5f);

            Assert.Equal(newOrigin, innerViewElement.ContentLeftTop);
            Assert.Equal(size.Width / 2, innerViewElement.ContentWidth);
        }

        [Fact]
        [Trait("Req.no", "VE4")]
        public void ArrangeWithScaleAndInnerMargin() {

            var innerViewElement = new TestElement() {
                LeftMargin = 5, RightMargin = 5, TopMargin = 5, BottomMargin = 5
            };
            var outerViewElement = new OuterViewElement(innerViewElement);

            outerViewElement.Measure(new SKSize(100, 100));
            outerViewElement.Arrange(SKPoint.Empty, outerViewElement.DesiredSize);

            outerViewElement.Scale(0.5f);

            Assert.Equal(0, innerViewElement.ContentLeftTop.X);
            Assert.Equal(0, innerViewElement.ContentLeftTop.Y);
            Assert.Equal(50, innerViewElement.ContentWidth);
            Assert.Equal(50, innerViewElement.ContentHeight);
        }

        [Fact]
        [Trait("Req.no", "VE5")]
        public void ArrangeWithRotationScaleMargin() {

            const float margin = 5;

            var innerViewElement = new TestElement {
                LeftMargin = margin, RightMargin = margin, TopMargin = margin, BottomMargin = margin
            };
            var outerViewElement = new OuterViewElement(innerViewElement);

            outerViewElement.Arrange(new SKPoint(100, 100), new SKSize(100, 100));

            var innerContentHeight = innerViewElement.ContentHeight;
            var innerContentWidth = innerViewElement.ContentWidth;

            outerViewElement.Rotate((float) Math.PI);
            outerViewElement.Scale(0.5f);

            outerViewElement.Scale(0.5f);
            
            Assert.Equal(100, outerViewElement.ContentLeftTop.X, precision: 1);
            Assert.Equal(100, outerViewElement.ContentLeftTop.Y, precision: 1);

            Assert.Equal(innerContentWidth / 4, innerViewElement.ContentWidth, 1);
            Assert.Equal(innerContentHeight / 4, innerViewElement.ContentHeight, 1);
        }

        [Fact]
        [Trait("Req.no", "VE6")]
        public void MultipleRotationsWithReset() {

            var element = new TestContentElement {
                ReturnedFromArrangeOverride = new SKSize(1000, 1000)
            };

            element.Arrange(SKPoint.Empty, element.ReturnedFromArrangeOverride);
            element.Rotate((float) Math.PI / 2);
            element.Rotate((float) Math.PI / 2);

            Assert.Equal(- 1000, element.ContentRightBottom.Y, 1);
            Assert.Equal(- 1000, element.ContentRightBottom.X, 1);
        }

        [Fact]
        [Trait("Req.no", "VE7")]
        public void MultipleRotationsWithoutReset() {

            var element = new TestContentElement {
                ReturnedFromArrangeOverride = new SKSize(1000, 1000)
            };

            element.Arrange(SKPoint.Empty, element.ReturnedFromArrangeOverride);
            element.Rotate((float) Math.PI / 2, false);
            element.Rotate((float) Math.PI / 2, false);

            Assert.Equal( 1000, element.ContentRightBottom.Y, 1);
            Assert.Equal(-1000, element.ContentRightBottom.X, 1);
        }

        [Fact]
        [Trait("Req.no", "VE8")]
        public void MultipleScalingWithoutReset() {

            var element = new TestContentElement {
                ReturnedFromArrangeOverride = new SKSize(1000, 1000)
            };

            element.Arrange(SKPoint.Empty, element.ReturnedFromArrangeOverride);
            element.Scale(0.5f);
            element.Scale(0.5f);

            Assert.Equal(250, element.ContentRightBottom.X, 1);
            Assert.Equal(250, element.ContentRightBottom.Y, 1);
        }

        [Fact]
        [Trait("Req.no", "VE9")]
        public void MultipleScalingWithReset() {

            var element = new TestContentElement {
                ReturnedFromArrangeOverride = new SKSize(1000, 1000)
            };

            element.Arrange(SKPoint.Empty, element.ReturnedFromArrangeOverride);
            element.Scale(0.5f, false);
            element.Scale(0.5f, false);

            Assert.Equal(500, element.ContentRightBottom.X, 1);
            Assert.Equal(500, element.ContentRightBottom.Y, 1);
        }

        [Fact]
        [Trait("Req.no", "VE10")]
        public void ArrangeWithScaledElement() {

            var element = new Element();
            element.Arrange(SKPoint.Empty, new SKSize(100, 100));
            element.Scale(0.5f);
            element.Reposition(new SKPoint(100, 100));
        }

        [Fact]
        [Trait("Req.no", "VE11")]
        public void ArrangeHandlesArrangeCoreOverrideFail() {

            var element = new TestArrangeCoreOverrideElement();
            element.Arrange(new SKPoint(42, 42), new SKSize(100, 100));

            Assert.Equal(0, element.UnscaledWidth);
            Assert.Equal(0, element.UnscaledHeight);
        }

        [Fact]
        [Trait("Req.no", "VE12")]
        public void ArrangeWithoutMargin() {

            var element = new TestElement();
            element.Arrange(new SKPoint(42, 42), new SKSize(100, 100));

            Assert.Equal(100, element.UnscaledWidth);
            Assert.Equal(100, element.UnscaledHeight);
        }

        [Fact]
        [Trait("Req.no", "VE13")]
        public void ArrangeWithMargin() {

            float margin = 10;
            float height = 50;
            float width = 100;

            var element = new TestElement { TopMargin = margin, LeftMargin = margin, RightMargin = margin, BottomMargin = margin };
            element.Arrange(new SKPoint(42, 42), new SKSize(width, height));

            Assert.Equal(width - 2 * margin, element.UnscaledWidth);
            Assert.Equal(height - 2 * margin, element.UnscaledHeight);
            Assert.Equal(width - 2 * margin, element.UnscaledWidth);
            Assert.Equal(height - 2 * margin, element.UnscaledHeight);
        }

        [Fact]
        [Trait("Req.no", "VE14")]
        public void ArrangeWithMax() {

            const float maxWidth = 100;
            const float maxHeight = 100;

            var element = new TestElement {MaxWidth = maxWidth, MaxHeight = maxHeight};
            element.Arrange(new SKPoint(42, 42), new SKSize(float.PositiveInfinity, float.PositiveInfinity));

            Assert.Equal(maxWidth, element.UnscaledWidth);
            Assert.Equal(maxHeight, element.UnscaledHeight);
        }

        [Fact]
        [Trait("Req.no", "VE15")]
        public void ArrangeWithMin() {

            const float minWidth = 10;
            const float minHeight = 10;

            var element = new TestMinElement {MinWidth = minWidth, MinHeight = minHeight};
            element.Arrange(new SKPoint(42, 42), new SKSize(30, 30));

            Assert.Equal(minWidth, element.UnscaledWidth);
            Assert.Equal(minHeight, element.UnscaledHeight);
        }

        [Fact]
        [Trait("Req.no", "VE16")]
        public void SetSizeFallingBetweenMinMaxTruncatesIfArrangeOverrideHigher() {

            const float minHeight = 40;
            const float maxHeight = 70;
            const float height = 50;

            const float minWidth = 30;
            const float maxWidth = 90;
            const float width = 60;

            var element = new TestElement {
                Height = height,
                Width = width,
                MinHeight = minHeight,
                MinWidth = minWidth,
                MaxHeight = maxHeight,
                MaxWidth = maxWidth
            };

            element.Arrange(SKPoint.Empty, new SKSize(maxWidth * 2, maxHeight * 2));

            Assert.Equal(height, element.UnscaledHeight);
            Assert.Equal(width, element.UnscaledWidth);
        }

        [Fact]
        [Trait("Req.no", "VE17")]
        public void SetSizeFallingBetweenMinMaxDoesTruncatesIfArrangeOverrideLower() {

            const float minHeight = 40;
            const float maxHeight = 70;
            const float height = 50;

            const float minWidth = 30;
            const float maxWidth = 90;
            const float width = 60;

            var element = new TestContentElement {
                Height = height,
                Width = width,
                MinHeight = minHeight,
                MinWidth = minWidth,
                MaxHeight = maxHeight,
                MaxWidth = maxWidth,
                ReturnedFromArrangeOverride = new SKSize(30, 30)
            };

            element.Arrange(SKPoint.Empty, new SKSize(maxWidth * 2, minHeight * 2));

            Assert.Equal(element.Width, element.UnscaledWidth);
            Assert.Equal(element.Height, element.UnscaledHeight);
        }

        [Fact]
        [Trait("Req.no", "VE18")]
        public void SetMinSizeHasHigherPriorityThanLowerArrangeOverride() {

            var element = new TestContentElement {
                MinHeight = 100,
                MinWidth = 100,
                ReturnedFromArrangeOverride = new SKSize(50, 50)
            };

            element.Arrange(SKPoint.Empty, new SKSize(element.MinWidth * 2, element.MinHeight * 2));
            
            Assert.Equal(element.MinWidth, element.UnscaledWidth);
            Assert.Equal(element.MinHeight, element.UnscaledWidth);
        }

        [Fact]
        [Trait("Req.no", "VE19")]
        public void SetMaxSizeHasHigherPriorityThanHigherArrangeOverride() {

            var element = new TestContentElement {
                MaxHeight = 10,
                MaxWidth = 10,
                ReturnedFromArrangeOverride = new SKSize(50, 50)
            };

            element.Arrange(SKPoint.Empty, new SKSize(200, 200));

            Assert.Equal(element.MaxWidth, element.UnscaledWidth);
            Assert.Equal(element.MaxHeight, element.UnscaledWidth);
        }

        [Fact]
        [Trait("Req.no", "VE20")]
        public void ArrangeOverrideOutOfAvailableSize() {

            var element = new TestContentElement {
                ReturnedFromArrangeOverride = new SKSize(100, 100)
            };

            element.Arrange(SKPoint.Empty, new SKSize(element.ReturnedFromArrangeOverride.Width / 2, element.ReturnedFromArrangeOverride.Height / 2));

            Assert.Equal(0, element.UnscaledWidth);
            Assert.Equal(0, element.UnscaledHeight);
            Assert.Equal(0, element.ContentHeight);
            Assert.Equal(0, element.ContentWidth);
        }
    }

    public class TestDockableViewElement : ViewElement {

        protected override SKSize MeasureOverride(SKSize availableSize) {
            return new SKSize(100, 100);
        }

        public override IEnumerable<IVisual> ProvideVisuals() {
            yield break;
        }
    }

    public class Element : ViewElement {

        public TestElement TestElement;

        public Element() {
            TestElement = new TestElement();
        }

        protected override SKSize ArrangeOverride(SKSize finalSize) {
            TestElement.Arrange(new SKPoint(20,20), new SKSize(40,40), this);
            return new SKSize(40,40);
        }

        public override IEnumerable<IVisual> ProvideVisuals() {
            yield break;
        }
    }

    public class TestMinElement : ViewElement {

        protected override SKSize ArrangeOverride(SKSize finalSize) {
            return SKSize.Empty;
        }

        public override IEnumerable<IVisual> ProvideVisuals() {
            yield break;
        }
    }

    public class TestArrangeCoreOverrideElement : ViewElement {

        protected override (SKRect Content, SKSize Size) ArrangeCore(SKSize size) {
            
            // return content out of bounds
            return (SKRect.Create(SKPoint.Empty, size + size), size);
        }

        public override IEnumerable<IVisual> ProvideVisuals() {
            yield break;
        }
    }

    public class TestElement : ViewElement {

        protected override SKSize ArrangeOverride(SKSize finalSize) {
            return finalSize;
        }

        protected override SKSize MeasureOverride(SKSize availableSize) {
            return availableSize;
        }

        public override IEnumerable<IVisual> ProvideVisuals() {
            yield break;
        }
    }

    public class TestContentElement : ViewElement {

        public SKSize ReturnedFromArrangeOverride { get; set; }

        protected override SKSize ArrangeOverride(SKSize finalSize) {
            return ReturnedFromArrangeOverride;
        }

        protected override SKSize MeasureOverride(SKSize availableSize) {
            return availableSize;
        }

        public override IEnumerable<IVisual> ProvideVisuals() {
            yield break;
        }
    }
}
