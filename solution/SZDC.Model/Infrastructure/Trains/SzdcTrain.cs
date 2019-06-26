using System.Collections.Generic;
using System.Collections.Immutable;

using GTTG.Model.Model.Events;
using GTTG.Model.Model.Traffic;

namespace SZDC.Model.Infrastructure.Trains {

    /// <inheritdoc />
    public class SzdcTrain : Train {

        private ImmutableList<TrainEvent> _completeSchedule;

        public int TrainNumber { get; }

        public TrainDecorationType TrainTypeDecoration { get; }

        public bool IsDepartingToOtherRailway { get; }

        public bool IsArrivingFromOtherRailway { get; }

        public TrainType TrainType { get; }

        /// <summary>
        /// Schedule of the train across multiple railways
        /// </summary>
        public ImmutableList<TrainEvent> CompleteSchedule {
            get => _completeSchedule;
            set => Update(ref _completeSchedule, value);
        }

        public SzdcTrain(int trainNumber, 
                         TrainType trainType,
                         TrainDecorationType trainTypeDecoration,
                         bool isDepartingToOtherRailway,
                         bool isArrivingFromOtherRailway,
                         IList<TrainEvent> schedule, // schedule being visualized in railway
                         IList<TrainEvent> completeSchedule)
            : base(schedule) {

            TrainType = trainType;
            IsDepartingToOtherRailway = isDepartingToOtherRailway;
            IsArrivingFromOtherRailway = isArrivingFromOtherRailway;
            TrainNumber = trainNumber;
            TrainTypeDecoration = trainTypeDecoration;
            CompleteSchedule = ImmutableList.CreateRange(completeSchedule);
        }
    }
}
