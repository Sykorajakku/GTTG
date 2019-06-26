using System;

using GTTG.Core.Time;

namespace GTTG.Integration {

    /*
     * Data visualized by this tutorial
     */
    public static class TrainTimetableData {

        /// <summary>
        /// Start <see cref="DateTime"/>, from which other values are created as offset.
        /// </summary>
        public static DateTime Start { get; } = DateTime.Today.AddHours(10);

        /// <summary>
        /// Time interval visible on tutorial startup.
        /// </summary>
        public static DateTimeInterval ViewDateTimeInterval { get; }
            = new DateTimeInterval(Start.AddHours(1), Start.AddHours(3));

        /// <summary>
        /// Time interval displayable in the tutorial. User can select <see cref="ViewDateTimeInterval"/> from this interval by interaction with the application.
        /// </summary>
        public static DateTimeInterval ContentDateTimeInterval { get; }
            = new DateTimeInterval(Start, Start.AddHours(4));
        
        /// <summary>
        /// Collects <see cref="ViewDateTimeInterval"/> and <see cref="ContentDateTimeInterval"/> for library use.
        /// </summary>
        public static DateTimeContext DateTimeContext = new DateTimeContext(
            ContentDateTimeInterval,
            ViewDateTimeInterval
        );
    }
}
