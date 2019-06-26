using System;
using System.Collections.Generic;

using GTTG.Core.Time;
using GTTG.Model.Model.Events;
using GTTG.Model.Model.Infrastructure;
using GTTG.Model.Model.Traffic;
using GTTG.Traffic.Model;

namespace GTTG.Traffic {

    public static class TrainTimetableData {

        public static DateTime Start { get; } = DateTime.Today.AddHours(10);

        public static DateTimeContext DateTimeContext = new DateTimeContext(
            new DateTimeInterval(Start, Start.AddHours(4)),
            new DateTimeInterval(Start.AddHours(1), Start.AddHours(3))
            );

        public static Railway Railway { get; } =
            
            new Railway(
            
                new List<Station> {

                    new Station(
                        new List<Track> {
                            new TutorialTrack(TrackType.Cargo),
                            new TutorialTrack(TrackType.Passenger),
                            new TutorialTrack(TrackType.Cargo)
                        }),

                    new Station(
                        new List<Track> {
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

        public static Traffic<Train> Traffic { get; } = 
            new Traffic<Train>(
                
                new List<Train> {

                    new Train(new List<TrainEvent> {
                        new TrainEvent(Start.AddMinutes(90), Railway.Stations[0], Railway.Stations[0].Tracks[0], TrainEventType.Departure),
                        new TrainEvent(Start.AddMinutes(105), Railway.Stations[1], Railway.Stations[1].Tracks[1], TrainEventType.Arrival),
                        new TrainEvent(Start.AddMinutes(107), Railway.Stations[1], Railway.Stations[1].Tracks[1], TrainEventType.Departure),
                        new TrainEvent(Start.AddMinutes(110), Railway.Stations[2], Railway.Stations[2].Tracks[0], TrainEventType.Arrival),
                    }),

                    new Train(new List<TrainEvent> {
                        new TrainEvent(Start.AddMinutes(140), Railway.Stations[2], Railway.Stations[2].Tracks[0], TrainEventType.Departure),
                        new TrainEvent(Start.AddMinutes(145), Railway.Stations[1], Railway.Stations[1].Tracks[2], TrainEventType.Passage),
                        new TrainEvent(Start.AddMinutes(160), Railway.Stations[0], Railway.Stations[0].Tracks[2], TrainEventType.Arrival),
                    })
                });
    }
}
