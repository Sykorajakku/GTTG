// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Xunit;

using GTTG.Core.Time;

namespace GTTG.Core.Tests.Time {

    public class DateTimeContextTests {

        [Fact]
        [Trait("Req.no", "DTI13")]
        public void IntervalsCanBeSame() {

            var dateTimeContext = new DateTimeContext(
                new DateTimeInterval(DateTime.Today, DateTime.Today.AddHours(4)),
                new DateTimeInterval(DateTime.Today, DateTime.Today.AddHours(4)));
        }

        [Fact]
        [Trait("Req.no", "DTI14")]
        public void BorderMustContainViews() {
            
            var exception = Record.Exception(() => new DateTimeContext(
                new DateTimeInterval(DateTime.Today, DateTime.Today.AddHours(4)),
                new DateTimeInterval(DateTime.Today.AddHours(3), DateTime.Today.AddHours(5))));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentOutOfRangeException>(exception);
        }
    }
}
