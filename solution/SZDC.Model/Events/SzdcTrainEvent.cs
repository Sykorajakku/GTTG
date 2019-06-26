using System;

using GTTG.Model.Model.Events;
using GTTG.Model.Model.Infrastructure;

namespace SZDC.Model.Events {

    public class SzdcTrainEvent : TrainEvent {

        public TrainEventFlags TrainEventFlags { get; set; }

        public SzdcTrainEvent(DateTime dateTime,
                              Station station,
                              Track track,
                              TrainEventType trainEventType,
                              TrainEventFlags trainEventFlags = TrainEventFlags.None)
            : base(dateTime, station, track, trainEventType) {

            TrainEventFlags = trainEventFlags;
        }

        public SzdcTrainEvent Clone(DateTime dateTime) {
            return new SzdcTrainEvent(dateTime, Station, Track, TrainEventType, TrainEventFlags);
        }
    }
}
