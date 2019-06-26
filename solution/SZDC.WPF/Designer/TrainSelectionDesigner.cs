using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using GTTG.Model.Model.Events;
using SZDC.Model.Events;
using SZDC.Model.Infrastructure;
using SZDC.Model.Infrastructure.Trains;
using SZDC.Wpf.Converters;

namespace SZDC.Wpf.Designer {

    public class TrainSelectionDesigner : SelectionModel {

        public static readonly List<SzdcTrack> Tracks = new List<SzdcTrack> {
            new SzdcTrack(TrackType.BlockSystem, "1a", "Pardubice hl.n"),
            new SzdcTrack(TrackType.BranchLine, "1b", "Medlešice"),
            new SzdcTrack(TrackType.Halt, "0", "Chrudim"),
            new SzdcTrack(TrackType.BlockSystem, "1", "Nasavrky")
        };

        public static readonly List<SzdcStation> Stations = new List<SzdcStation> {
            new SzdcStation(new [] {Tracks[0]}, "Pardubice hl.n"),
            new SzdcStation(new [] {Tracks[1]}, "Medlešice"),
            new SzdcStation(new [] {Tracks[2]}, "Chrudim"),
            new SzdcStation(new [] {Tracks[3]}, "Nasavrky")
        };

        public static readonly List<TrainEvent> TrainEvents = new List<TrainEvent> {
            new SzdcTrainEvent(DateTime.Today.AddHours(13).AddMinutes(42), Stations[0], Stations[0].Tracks[0], TrainEventType.Departure),
            new SzdcTrainEvent(DateTime.Today.AddHours(13).AddMinutes(52), Stations[1], Stations[1].Tracks[0], TrainEventType.Passage),
            new SzdcTrainEvent(DateTime.Today.AddHours(14).AddMinutes(00), Stations[2], Stations[2].Tracks[0], TrainEventType.Passage),
            new SzdcTrainEvent(DateTime.Today.AddHours(14).AddMinutes(10), Stations[3], Stations[3].Tracks[0], TrainEventType.Arrival),
        };

        public TrainSelectionDesigner() : base(null, null) {

            SelectedTrain = new SzdcTrain(66, TrainType.Ex, TrainDecorationType.FollowsValidDirection, true, true,
                ImmutableList<TrainEvent>.Empty, ImmutableList<TrainEvent>.Empty) {
                Schedule = ImmutableArray.CreateRange(TrainEvents.Skip(2))
            };
            Trains = ImmutableArray.Create(SelectedTrain);
        }
    }
}
