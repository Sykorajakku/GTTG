using System;
using System.Collections.Generic;

using SZDC.Data.Model;
using SZDC.Model.Infrastructure;

namespace SZDC.Data.Import {

    public class TrackGenerator {

        public static readonly string PassengerStationPostfixDescription = " z";
        public static readonly string BranchingOffPointPrefixDescription = "Odb";

        private readonly Random _random = new Random();

        public List<OrderedTrack> GenerateTracks(string stationName) {

            var trackCount = _random.Next(1, 5);
            var tracks = new List<OrderedTrack>();
            var trackType = DetermineTrackType(stationName);

            for (var i = 0; i < trackCount; i++) {

                var track = new Track { Number = $"A{i + 1}", TrackType = trackType };
                tracks.Add(new OrderedTrack { Order = i, Track = track });
            }
            return tracks;
        }

        public static TrackType DetermineTrackType(string stationName) {

            if (stationName.StartsWith(BranchingOffPointPrefixDescription)) {
                return TrackType.BranchingOffPoint;
            }

            if (stationName.EndsWith(PassengerStationPostfixDescription)) {
                return TrackType.PassengerStation;
            }

            // default
            return TrackType.PassengerStation;
        }
    }
}
