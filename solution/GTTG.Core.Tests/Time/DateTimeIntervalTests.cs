// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Xunit;

using GTTG.Core.Time;

namespace GTTG.Core.Tests.Time {

    public class DateTimeIntervalTests {

        [Fact]
        [Trait("Req.no","DTI1")]
        public void IntervalsDoesNotIntersect() {

            var firstInterval = new DateTimeInterval(
                DateTime.Today, DateTime.Today.AddHours(8)
            );
            var secondInterval = new DateTimeInterval(
                DateTime.Today.AddHours(12), DateTime.Today.AddHours(16)
            );

            Assert.False(firstInterval.IntersectsWith(secondInterval));
        }

        [Fact]
        [Trait("Req.no", "DTI2")]
        public void IntervalsDoesIntersectInSubset() {

            var firstInterval = new DateTimeInterval(
                DateTime.Today, DateTime.Today.AddHours(8)
            );
            var secondInterval = new DateTimeInterval(
                DateTime.Today.AddHours(4), DateTime.Today.AddHours(6)
            );

            Assert.True(firstInterval.IntersectsWith(secondInterval));
            Assert.True(secondInterval.IntersectsWith(firstInterval));
        }

        [Fact]
        [Trait("Req.no", "DTI3")]
        public void IntersectsWithItself() {

            var interval = new DateTimeInterval(
                DateTime.Today, DateTime.Today.AddHours(8)
            );

            Assert.True(interval.IntersectsWith(interval));
        }

        [Fact]
        [Trait("Req.no", "DTI4")]
        public void DoesIntersects() {

            var firstInterval = new DateTimeInterval(
                DateTime.Today, DateTime.Today.AddHours(8)
            );
            var secondInterval = new DateTimeInterval(
                DateTime.Today.AddHours(4), DateTime.Today.AddHours(16)
            );

            Assert.True(firstInterval.IntersectsWith(secondInterval));
        }

        [Fact]
        [Trait("Req.no", "DTI5")]
        public void ReturnsCorrectMultiple() {

            var interval = new DateTimeInterval(
                DateTime.Today, DateTime.Today.AddMilliseconds(100)
            );

            var startMultiple = interval.GetMultiple(interval.Start);
            var halfMultiple = interval.GetMultiple(interval.Start.AddMilliseconds(50));
            var endMultiple = interval.GetMultiple(interval.End);

            Assert.Equal(0.0f, startMultiple);
            Assert.Equal(0.5f, halfMultiple);
            Assert.Equal(1.0f, endMultiple);
        }

        [Fact]
        [Trait("Req.no", "DTI6")]
        public void AcceptsOutOfBoundsDateTimeOnGetMultiple() {

            var interval = new DateTimeInterval(
                DateTime.Today, DateTime.Today.AddHours(8)
            );

            var outOfBoundsMultipleBeforeInterval = interval.GetMultiple(interval.Start.AddHours(-4));
            var outOfBoundsMultipleAfterInterval = interval.GetMultiple(interval.End.AddHours(4));

            Assert.Equal(-0.5f, outOfBoundsMultipleBeforeInterval);
            Assert.Equal(1.5f, outOfBoundsMultipleAfterInterval);
        }

        [Fact]
        [Trait("Req.no", "DTI7")]
        public void ReturnsDateTimesDefinedByTimeSpanPeriodCorrectly() {

            var intervalStart = DateTime.Today;
            const int hoursIntervalLength = 3;

            var interval = new DateTimeInterval(
                intervalStart, intervalStart.AddHours(hoursIntervalLength)
            );
            var periodStart = intervalStart.AddMinutes(10);

            var expectedDateTimes = new List<DateTime> { periodStart };
            for (var i = 1; i < hoursIntervalLength; ++i) {
                expectedDateTimes.Add(periodStart.AddHours(i));
            }
            
            var dateTimesByPeriod = interval.GetDateTimesByPeriod(periodStart, new TimeSpan(hours: 1, minutes: 0, seconds: 0));

            Assert.Equal(expectedDateTimes, dateTimesByPeriod);
        }

        [Fact]
        [Trait("Req.no", "DTI8")]
        public void ReturnsDateTimesDefinedByTimeSpanPeriodOnPeriodOutOfRange() {

            var intervalStart = DateTime.Today;
            var interval = new DateTimeInterval(intervalStart, intervalStart.AddHours(1));

            var period = new TimeSpan(hours: 10, minutes: 0, seconds: 0);
            var periodStart = intervalStart.AddMinutes(10);

            var expectedDateTimes = new List<DateTime> { periodStart };
            var dateTimesByPeriod = interval.GetDateTimesByPeriod(periodStart, period);

            Assert.Equal(expectedDateTimes, dateTimesByPeriod);
        }

        [Fact]
        [Trait("Req.no", "DTI9")]
        public void ThrowsIfStartOutOfRangeWhenGettingDateTimesByPeriod() {

            var intervalStart = DateTime.Today;
            var interval = new DateTimeInterval(intervalStart, intervalStart.AddHours(1));

            var period = new TimeSpan(hours: 0, minutes: 10, seconds: 0);
            var periodStart = DateTime.Today.AddDays(1);

            void Act() => interval.GetDateTimesByPeriod(periodStart, period);
            Assert.Throws<ArgumentOutOfRangeException>((Action) Act);
        }
    }
}
