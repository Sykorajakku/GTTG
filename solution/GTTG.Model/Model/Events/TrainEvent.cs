// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

using GTTG.Core.Base;
using GTTG.Model.Model.Infrastructure;

namespace GTTG.Model.Model.Events {

    /// <summary>
    /// Type of train movement event in station.
    /// </summary>
    public enum TrainEventType {

        /// <summary>
        /// Event of train arriving to a station.
        /// </summary>
        Arrival,

        /// <summary>
        /// Event of train leaving a station.
        /// </summary>
        Departure,

        /// <summary>
        /// Event of train passing through a station.
        /// </summary>
        Passage
    }

    /// <summary>
    /// Base class for train events.
    /// </summary>
    public class TrainEvent : ObservableObject {

        /// <summary>
        /// Movement type of event in station.
        /// </summary>
        public TrainEventType TrainEventType { get; }

        /// <summary>
        /// <see cref="System.DateTime"/> when event occurs.
        /// </summary>
        public DateTime DateTime { get; }

        /// <summary>
        /// <see cref="Infrastructure.Track"/> where event occurs.
        /// </summary>
        public Track Track { get; }

        /// <summary>
        /// <see cref="Infrastructure.Station"/> where event occurs.
        /// </summary>
        public Station Station { get; }

        /// <summary>
        /// Determines whether event is arrival to the station.
        /// </summary>
        public bool IsArrival => TrainEventType == TrainEventType.Arrival;

        /// <summary>
        /// Determines whether the event is passage through the station.
        /// </summary>
        public bool IsPassage => TrainEventType == TrainEventType.Passage;

        /// <summary>
        /// Determines whether the event is departure from the station.
        /// </summary>
        public bool IsDeparture => TrainEventType == TrainEventType.Departure;

        /// <summary>
        /// Creates a new event.
        /// </summary>
        /// <param name="dateTime">Time value of event.</param>
        /// <param name="station">Station where event occurs.</param>
        /// <param name="track">Track of the station where event occurs.</param>
        /// <param name="trainEventType">Type of the event.</param>
        /// <exception cref="ArgumentException"><paramref name="station"/> does not contain <paramref name="track"/>.</exception>
        public TrainEvent(DateTime dateTime, Station station, Track track, TrainEventType trainEventType) {

            DateTime = dateTime;
            Station = station;
            Track = track;
            TrainEventType = trainEventType;
            
            if (!station.Tracks.Contains(track)) {
                throw new ArgumentException($"{nameof(station)} does not contain provided {nameof(track)}.");
            }
        }
    }
}
