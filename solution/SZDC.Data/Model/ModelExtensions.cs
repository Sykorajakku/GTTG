using System;
using System.Linq;
using SZDC.Editor.Interfaces;

namespace SZDC.Data.Model {

    public static class ConverterExtensions {

        public static StaticTrainMovementEvent ToStaticTrainMovementEvent(this StaticTrainEvent staticEvent, Station station) {

            return new StaticTrainMovementEvent {
                Station = new StationDescription { Name = station.Name, Tracks = station.Tracks.Select(ToTrackDescription).ToList() },
                Hours = staticEvent.Hours,
                Minutes = staticEvent.Minutes,
                HasMoreThan30Seconds = staticEvent.HasMoreThan30Seconds
            };
        }

        public static TrackDescription ToTrackDescription(this OrderedTrack orderedTrack) {

            var track = orderedTrack.Track;
            var trackType = track.TrackType ?? throw new ArgumentException("Track type cannot be null.");
            return new TrackDescription {TrackName = track.Number, TrackType = trackType};
        }
    }
}
