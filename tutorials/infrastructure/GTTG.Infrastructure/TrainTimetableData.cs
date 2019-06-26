using System;
using System.Collections.Generic;

using GTTG.Core.Time;
using GTTG.Infrastructure.Model;
using GTTG.Model.Model.Infrastructure;

namespace GTTG.Infrastructure {

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
        public static DateTimeInterval ViewDateTimeInterval { get; } = new DateTimeInterval(Start, Start.AddHours(4));

        /// <summary>
        /// Time interval displayable in the tutorial. User can select <see cref="ViewDateTimeInterval"/> from this interval by interaction with the application.
        /// </summary>
        public static DateTimeInterval ContentDateTimeInterval { get; } = new DateTimeInterval(Start.AddHours(1), Start.AddHours(3));
        
        /// <summary>
        /// Collects <see cref="ViewDateTimeInterval"/> and <see cref="ContentDateTimeInterval"/> for library use.
        /// </summary>
        public static DateTimeContext DateTimeContext = new DateTimeContext(
            ViewDateTimeInterval,
            ContentDateTimeInterval
        );

        public static Railway Railway { get; } =

            new Railway(

                new List<Station> {

                    new Station(
                        new List<TutorialTrack> {
                            new TutorialTrack(TrackType.Cargo),
                            new TutorialTrack(TrackType.Passenger),
                            new TutorialTrack(TrackType.Cargo)
                        }),

                    new Station(
                        new List<TutorialTrack> {
                            new TutorialTrack(TrackType.Cargo),
                            new TutorialTrack(TrackType.Passenger),
                            new TutorialTrack(TrackType.Cargo),
                            new TutorialTrack(TrackType.Cargo)
                        }),

                    new Station( 
                        new List<Track> {
                            new TutorialTrack(TrackType.Cargo)
                        })
                });
    }
}
