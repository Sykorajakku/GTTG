// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace GTTG.Core.Time {

    /// <summary>Represents interval of <see cref="DateTime"/> values from <see cref="Start"/> to <see cref="End"/>.</summary>
    public struct DateTimeInterval {

        /// <summary>Start of <see cref="DateTimeInterval"/>. This value is also in the interval of this instance.</summary>
        public DateTime Start { get; }

        /// <summary>End of <see cref="DateTimeInterval"/>. This value is also in interval of this instance.</summary>
        public DateTime End { get; }

        /// <summary>Time elapsed between <see cref="Start"/> and <see cref="End"/>.</summary>
        public TimeSpan TimeSpan => End - Start;

        /// <summary>Creates <see cref="DateTimeInterval"/> of <see cref="DateTime"/> values.</summary>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="end"/> is earlier or same as <paramref name="start"/>.</exception>
        public DateTimeInterval(DateTime start, DateTime end) {

            Start = start;
            End = end;

            if (start >= end) {
                throw new ArgumentOutOfRangeException($"{nameof(end)} {end} is earlier or same as {nameof(start)} {start}");
            }
        }

        /// <summary>Determines whether an provided <paramref name="dateTimeInterval"/> is in the interval of this instance.</summary>
        /// <param name="dateTimeInterval">The provided <see cref="DateTimeInterval"/>.</param>
        /// <returns><see langword="true" /> if <paramref name="dateTimeInterval" /> is in interval of this instance; otherwise, <see langword="false" />.</returns>
        public bool Contains(DateTimeInterval dateTimeInterval) {
            return Start <= dateTimeInterval.Start
                && End >= dateTimeInterval.End;
        }

        /// <summary> Determines whether an provided <paramref name="dateTime"/> is in the <see cref="DateTimeInterval"/>.</summary>
        /// <param name="dateTime">The provided <see cref="DateTime"/>.</param>
        /// <returns><see langword="true" /> if <paramref name="dateTime" /> is found in the <see cref="DateTimeInterval" />; otherwise, <see langword="false" />.</returns>
        public bool Contains(DateTime dateTime) {
            return Start <= dateTime && End >= dateTime;
        }

        /// <summary>Determines whether an provided <paramref name="dateTimeInterval"/> contains at least one same <see cref="DateTime"/> value from <see cref="DateTimeInterval"/>.</summary>
        /// <param name="dateTimeInterval">The provided <see cref="DateTimeInterval"/>.</param>
        /// <returns><see langword="true" /> if <paramref name="dateTimeInterval" /> intersects with the <see cref="DateTimeInterval" />; otherwise, <see langword="false" />.</returns>
        public bool IntersectsWith(DateTimeInterval dateTimeInterval) {
            return dateTimeInterval.Contains(Start) ||
                   dateTimeInterval.Contains(End) ||
                   Contains(dateTimeInterval.Start) ||
                   Contains(dateTimeInterval.End);
        }

        /// <summary>Converts <see cref="DateTime"/> to multiple of this instance interval.</summary>
        /// <param name="dateTime"><see cref="DateTime"/> to convert.</param>
        /// <returns>Values [0.00f - 1.00f] for <see cref="DateTime"/> for which <see cref="Contains(DateTimeInterval)"/> returns true. Otherwise for <see cref="DateTime"/> outside the interval returns values greater or lower than mentioned return value interval.</returns>
        public float GetMultiple(DateTime dateTime) {
            return (float)((dateTime - Start).TotalMilliseconds / TimeSpan.TotalMilliseconds);
        }

        /// <summary>Provides <see cref="DateTime"/> values in interval of this instance defined by start and repeating period.</summary>
        /// <param name="start">Start included in returned values from which enumeration starts.</param>
        /// <param name="period"><see cref="TimeSpan"/> period separating returned values.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="start"/> is not in <see cref="DateTime"/>.</exception>
        /// <returns><see cref="DateTime"/> values in <see cref="DateTimeInterval"/> separated by <paramref name="period"/>. Contains <paramref name="start"/>.</returns>
        public IEnumerable<DateTime> GetDateTimesByPeriod(DateTime start, TimeSpan period) {

            if (!Contains(start)) {
                throw new ArgumentOutOfRangeException($"{nameof(start)} {start} is not in {this}");
            }

            return GetDateTimesByPeriodSanitized(start, period);
        }

        private IEnumerable<DateTime> GetDateTimesByPeriodSanitized(DateTime start, TimeSpan period) {

            yield return start;

            while (start + period < End) {
                yield return start + period;
                start += period;
            }
        }
        
        /// <summary>Converts this interval to string representation using default <see cref="DateTime.ToString()"/>.</summary>
        /// <returns>String representation of <see cref="DateTimeInterval"/>.</returns>
        public override string ToString() {
            return $"({Start} - {End})";
        }

        /// <summary>Converts this interval to string using format <see cref="DateTime"/> pattern.</summary>
        /// <param name="format">A standard or custom date and time format string to be used as format on both <see cref="Start"/> and <see cref="End"/>.</param>
        /// <exception cref="T:System.FormatException">The length of <paramref name="format" /> is 1, and it is not one of the format specifier characters defined for <see cref="T:System.Globalization.DateTimeFormatInfo" />.-or- <paramref name="format" /> does not contain a valid custom format pattern. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The date and time is outside the range of dates supported by the calendar used by the current culture.</exception>
        /// <returns>String representation of this interval.</returns>
        public string ToString(string format) {
            return $"{Start.ToString(format)} - {End.ToString(format)}";
        }
    }
}
