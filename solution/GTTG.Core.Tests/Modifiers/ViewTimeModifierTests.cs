// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using SkiaSharp;
using Xunit;

using GTTG.Core.Component;
using GTTG.Core.Time;

namespace GTTG.Core.Tests.Modifiers {

    public class ViewTimeModifierTests {

        [Fact]
        [Trait("Req.no", "DTI10")]
        public void CorrectlyChangesCanvasTime() {

            var viewModifier = new ViewModifier(1024, 512, 4096, 512);
            var dateTimeContext = new DateTimeContext(
                new DateTimeInterval(DateTime.Today, DateTime.Today.AddHours(8)),
                new DateTimeInterval(DateTime.Today.AddHours(4), DateTime.Today.AddHours(6))
            );
            var viewTimeModifier = new GraphicalComponent(viewModifier, dateTimeContext);

            var result = viewTimeModifier.TryChangeViewTime(new DateTimeInterval(DateTime.Today.AddHours(2), DateTime.Today.AddHours(6)));

            Assert.Equal(TimeModificationResult.TimeModified, result);
            Assert.Equal(new DateTimeInterval(DateTime.Today.AddHours(2), DateTime.Today.AddHours(6)), viewTimeModifier.DateTimeContext.ViewDateTimeInterval);
        }

        [Fact]
        [Trait("Req.no", "DTI11")]
        public void CorrectlyChangesCanvasTimeMultipleTimes() {

            var viewModifier = new ViewModifier(1024, 512, 4096, 512);
            var dateTimeContext = new DateTimeContext(
                new DateTimeInterval(DateTime.Today, DateTime.Today.AddHours(8)),
                new DateTimeInterval(DateTime.Today.AddHours(4), DateTime.Today.AddHours(6))
            );
            var viewTimeModifier = new GraphicalComponent(viewModifier, dateTimeContext);

            var dateTimeInterval = new DateTimeInterval(DateTime.Today.AddHours(2), DateTime.Today.AddHours(6));
            var firstResult = viewTimeModifier.TryChangeViewTime(dateTimeInterval);
            Assert.Equal(TimeModificationResult.TimeModified, firstResult);
            Assert.Equal(2048, viewModifier.BorderWidth);
            Assert.Equal(dateTimeInterval, viewTimeModifier.DateTimeContext.ViewDateTimeInterval);

            dateTimeInterval = new DateTimeInterval(DateTime.Today.AddHours(2), DateTime.Today.AddHours(6));
            var secondResult = viewTimeModifier.TryChangeViewTime(dateTimeInterval);
            Assert.Equal(TimeModificationResult.TimeUnmodified, secondResult);
            Assert.Equal(2048, viewModifier.BorderWidth);
            Assert.Equal(dateTimeInterval, viewTimeModifier.DateTimeContext.ViewDateTimeInterval);

            dateTimeInterval = new DateTimeInterval(DateTime.Today.AddHours(0), DateTime.Today.AddHours(8));
            var thirdResult = viewTimeModifier.TryChangeViewTime(dateTimeInterval);
            Assert.Equal(TimeModificationResult.TimeModified, thirdResult);
            Assert.Equal(1024, viewModifier.BorderWidth);
            Assert.Equal(dateTimeInterval, viewTimeModifier.DateTimeContext.ViewDateTimeInterval);
        }

        [Fact]
        [Trait("Req.no", "DTI12")]
        public void TimeModifiedOnTransformOperations() {

            var viewModifier = new ViewModifier(1024, 512, 1024, 512);
            var dateTimeContext = new DateTimeContext(
                new DateTimeInterval(DateTime.Today, DateTime.Today.AddHours(8)),
                new DateTimeInterval(DateTime.Today, DateTime.Today.AddHours(8))
            );
            var viewTimeModifier = new GraphicalComponent(viewModifier, dateTimeContext);
            var scaleTransformationResult = viewTimeModifier.TryScale(new SKPoint(512, 256), 1);

            Assert.Equal(ScaleTransformationResult.ViewModifiedWithSameOrigin, scaleTransformationResult);
            Assert.Equal(new DateTimeInterval(DateTime.Today.AddHours(2), DateTime.Today.AddHours(6)), viewTimeModifier.DateTimeContext.ViewDateTimeInterval);

            var translationTransformationResult = viewTimeModifier.TryTranslate(new SKPoint(512, 0));

            Assert.Equal(TranslationTransformationResult.ViewModified, translationTransformationResult);
            Assert.Equal(new DateTimeInterval(DateTime.Today.AddHours(4), DateTime.Today.AddHours(8)), viewTimeModifier.DateTimeContext.ViewDateTimeInterval);
        }
    }
}
