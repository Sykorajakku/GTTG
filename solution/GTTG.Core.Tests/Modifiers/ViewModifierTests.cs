// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using SkiaSharp;
using Xunit;

using GTTG.Core.Component;
using GTTG.Core.Time;

namespace GTTG.Core.Tests.Modifiers {

    public class ViewModifierTests {

        [Fact]
        [Trait("Req.no", "COMP1")]
        public void ConvertPointInViewAfterTranslation() {

            const int viewWidth = 1024;
            var viewModifier = new ViewModifier(viewWidth, 1024, viewWidth * 4, 1024);

            var viewPoint = new SKPoint(0, 0);
            var expectedBorderPoint = new SKPoint(0 + viewWidth, 0);

            viewModifier.TryTranslate(new SKPoint(viewWidth, 0));
            var computedBorderPoint = viewModifier.ConvertViewPositionToContentPosition(viewPoint);

            Assert.Equal(expectedBorderPoint, computedBorderPoint);
        }

        [Fact]
        [Trait("Req.no", "COMP2")]
        public void ConvertPointInView() {

            const int viewWidth = 1024;
            var viewModifier = new ViewModifier(viewWidth, 1024, viewWidth * 4, 1024);

            var viewPoint = new SKPoint(1024, 0);
            var expectedBorderPoint = new SKPoint(1024, 0);

            var computedBorderPoint = viewModifier.ConvertViewPositionToContentPosition(viewPoint);
            Assert.Equal(expectedBorderPoint, computedBorderPoint);
        }

        [Fact]
        [Trait("Req.no", "COMP3")]
        public void InvalidPointToConvert() {

            var viewModifier = new ViewModifier(1024, 1024, 1024, 1024);
            var viewPoint = new SKPoint(2048, 0);
            var exception = Record.Exception(() => viewModifier.ConvertViewPositionToContentPosition(viewPoint));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentOutOfRangeException>(exception);
        }

        [Fact]
        [Trait("Req.no", "COMP4")]
        public void NegativeInvalidPointToConvert() {

            var viewModifier = new ViewModifier(1024, 1024, 1024, 1024);
            var viewPoint = new SKPoint(-2, 0);
            var exception = Record.Exception(() => viewModifier.ConvertViewPositionToContentPosition(viewPoint));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentOutOfRangeException>(exception);
        }

        [Fact]
        [Trait("Req.no", "COMP5")]
        public void PositiveDeltaKeepsSameBorderPositionOfPoint() {

            var viewModifier = new ViewModifier(1024, 1024, 1024, 1024);
            var viewPoint = new SKPoint(200, 200);
            var borderLocationBeforeScale = viewModifier.ConvertViewPositionToContentPosition(viewPoint);

            viewModifier.TryScale(viewPoint, 1.0f);
            var borderLocationAfterScale = viewModifier.ConvertViewPositionToContentPosition(viewPoint);

            Assert.Equal(borderLocationAfterScale, borderLocationBeforeScale);
        }

        [Fact]
        [Trait("Req.no", "COMP6")]
        public void NegativeDeltaWithScaleLowerOneIsNotApplied() {

            var viewTransformer = new ViewModifier(1024, 1024, 1024, 1024);
            var scaleTransformationResult = viewTransformer.TryScale(new SKPoint(0,0), -1.0f);

            Assert.Equal(ScaleTransformationResult.ViewUnmodified, scaleTransformationResult);
        }

        [Fact]
        [Trait("Req.no", "COMP7")]
        public void TranslationOutOfBoundsOnLeftNotApplied() {

            var viewTransformer = new ViewModifier(1024, 1024, 1024, 1024);
            var translationTransformationResult = viewTransformer.TryTranslate(new SKPoint(-2, 0));

            Assert.Equal(TranslationTransformationResult.ViewUnmodified, translationTransformationResult);
        }

        [Fact]
        [Trait("Req.no", "COMP8")]
        public void TranslationOutOfBoundsOnRightNotApplied() {

            var viewTransformer = new ViewModifier(1024, 1024, 1024, 1024);
            var translationTransformationResult = viewTransformer.TryTranslate(new SKPoint(2, 0));

            Assert.Equal(TranslationTransformationResult.ViewUnmodified, translationTransformationResult);
        }

        [Fact]
        [Trait("Req.no", "COMP9")]
        public void TranslationOutOfBoundsOnTopNotApplied() {

            var viewTransformer = new ViewModifier(1024, 1024, 1024, 1024);
            var translationTransformationResult = viewTransformer.TryTranslate(new SKPoint(0, 2));

            Assert.Equal(TranslationTransformationResult.ViewUnmodified, translationTransformationResult);
        }

        [Fact]
        [Trait("Req.no", "COMP10")]
        public void TranslationOutOfBoundsOnBottomNotApplied() {

            var viewTransformer = new ViewModifier(1024, 1024, 1024, 1024);
            var translationTransformationResult = viewTransformer.TryTranslate(new SKPoint(0, -2));

            Assert.Equal(TranslationTransformationResult.ViewUnmodified, translationTransformationResult);
        }

        [Fact]
        [Trait("Req.no", "COMP11")]
        public void PointOutOfViewBoundsNotSupported() {

            var viewTransformer = new ViewModifier(1024, 1024, 1024, 1024);
            var exception = Record.Exception(() => viewTransformer.TryScale(new SKPoint(-1, -1), 0.0f));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentOutOfRangeException>(exception);
        }

        [Fact]
        [Trait("Req.no", "COMP12")]
        public void ComputesBorderWidthCorrectly() {

            var testViewTimeContext = new DateTimeContext(
                new DateTimeInterval(DateTime.Today, DateTime.Today.AddHours(24)),
                new DateTimeInterval(DateTime.Today.AddHours(12), DateTime.Today.AddHours(18))
            );

            const int viewWidth = 1000;
            var borderWidth = ViewModifier.ComputeBorderWidth(viewWidth, testViewTimeContext);
            Assert.Equal(4000, borderWidth);
        }

        [Fact]
        [Trait("Req.no", "COMP13")]
        public void ResizesBorderCorrectly() {

            var viewModifier = new ViewModifier(1024, 512, 1024, 512);
            var modificationResult = viewModifier.TryResizeBorder(2048, 512, 1024, 0);

            Assert.Equal(ResizeTransformationResult.ViewModified, modificationResult);
            Assert.Equal(-1024, viewModifier.ViewMatrix.TransX);
            Assert.Equal(0, viewModifier.ViewMatrix.TransY);
        }

        [Fact]
        [Trait("Req.no", "COMP14")]
        public void ResizesScaledBorderCorrectlyOnSameOrigin() {

            var newBorderWidth = 4096;
            var newBorderHeight = 1024;

            var viewModifier = new ViewModifier(1024, 512, 2048, 512);
            viewModifier.TryScale(new SKPoint(512, 256), 1);

            Assert.Equal(viewModifier.ViewMatrix.TransX, -512, 0);
            Assert.Equal(viewModifier.ViewMatrix.TransY, -256, 0);

            var modificationResult = viewModifier.TryResizeBorder(newBorderWidth, newBorderHeight, 512, 256);

            Assert.Equal(newBorderWidth, viewModifier.BorderWidth, 0);
            Assert.Equal(newBorderHeight, viewModifier.BorderHeight, 0);
            Assert.Equal(viewModifier.ViewMatrix.TransX, -1024, 0);
            Assert.Equal(viewModifier.ViewMatrix.TransY, -512, 0);
            Assert.Equal(ResizeTransformationResult.ViewModified, modificationResult);
        }

        [Fact]
        [Trait("Req.no", "COMP15")]
        public void ResizesBorderCorrectlyWithOutOfBoundsCorrection() {

            var viewModifier = new ViewModifier(1024, 512, 1024, 512);
            viewModifier.TryResizeBorder(1024, 1024);
            viewModifier.TryTranslate(new SKPoint(0, - 512));
            viewModifier.TryResizeBorder(1024, 512);
            
            Assert.Equal(SKMatrix.MakeIdentity(), viewModifier.ViewMatrix);
        }

        [Fact]
        [Trait("Req.no", "COMP16")]
        public void TranslateScaleOutOfBounds() {

            var viewModifier = new ViewModifier(1024, 512, 1024, 512);
            var dateTimeContext = new DateTimeContext(
                new DateTimeInterval(DateTime.Today, DateTime.Today.AddHours(8)),
                new DateTimeInterval(DateTime.Today, DateTime.Today.AddHours(8))
            );
            var viewTimeModifier = new GraphicalComponent(viewModifier, dateTimeContext);
            var scaleTransformationResult = viewTimeModifier.TryScale(new SKPoint(512, 256), 1);

            Assert.Equal(ScaleTransformationResult.ViewModifiedWithSameOrigin, scaleTransformationResult);
            Assert.Equal(new DateTimeInterval(DateTime.Today.AddHours(2), DateTime.Today.AddHours(6)), viewTimeModifier.DateTimeContext.ViewDateTimeInterval);
        }
    }
}
