// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace GTTG.Core.Time {

    /// <summary>Represents day interval as sequence of hours from which are determined available hour windows.</summary>
    public class DayHoursInterval {

        /// <summary>Representation of whole day.</summary>
        public static DayHoursInterval WholeDay = new DayHoursInterval(0, 24);

        /// <summary>All possible hour windows of day.</summary>
        public static readonly ImmutableArray<int> AllIntervalHours = Enumerable.Range(1, 24).ToImmutableArray();

        /// <summary>Start hour of day hours sequence.</summary>
        public int StartHour { get; }

        /// <summary>End hour of day hours sequence.</summary>
        public int EndHour { get; }

        /// <summary>Longest available hour window.</summary>
        public int MaxWindowHour => WindowHours.LastOrDefault();

        /// <summary>Available hour windows.</summary>
        public IReadOnlyList<int> WindowHours { get; }

        private DayHoursInterval(int startHour, int endHour) {

            if (!(startHour < endHour && endHour <= 24)) {
                throw new ArgumentException($"Arguments {nameof(startHour)} {startHour} to {nameof(EndHour)} {endHour} does not form hours interval.");
            }

            StartHour = startHour;
            EndHour = endHour;
        }

        /// <summary>Creates hour windows for provided hour interval.</summary>
        /// <param name="startHour">Value assigned to <see cref="StartHour"/>.</param>
        /// <param name="endHour">Value assigned to <see cref="EndHour"/>.</param>
        /// <param name="windowHours">If provided, only those hour windows are allowed to be in <see cref="WindowHours"/>.</param>
        /// <exception cref="ArgumentException"><paramref name="startHour"/> and <paramref name="endHour"/> does not form valid hour interval of day.</exception>
        public DayHoursInterval(int startHour, int endHour, ICollection<int> windowHours = null)
            : this(startHour, endHour) {
            windowHours = windowHours ?? AllIntervalHours;
            WindowHours = Enumerable.Range(1, (endHour - startHour)).Where(windowHours.Contains).ToList();
        }

        /// <summary>Converts this day interval to <see cref="DateTimeInterval"/>.</summary>
        /// <param name="date">Date value <see cref="DateTime"/> with date representing this interval.</param>
        /// <returns><see cref="DateTimeInterval"/> with <paramref name="date"/> date and day hours interval of this instance.</returns>
        public DateTimeInterval ToDateTimeInterval(DateTime date = new DateTime()) {
            date = date.Date;
            return new DateTimeInterval(date.AddHours(StartHour), date.AddHours(EndHour));
        }

        /// <summary>
        /// Compares intervals by start and end values.
        /// </summary>
        public bool Equals(DayHoursInterval other) {
            return StartHour == other.StartHour && EndHour == other.EndHour;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            return obj is DayHoursInterval other && Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode() {
            unchecked {
                var hashCode = (int)StartHour;
                hashCode = (hashCode * 397) ^ (int)EndHour;
                hashCode = (hashCode * 397) ^ (WindowHours != null ? WindowHours.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
