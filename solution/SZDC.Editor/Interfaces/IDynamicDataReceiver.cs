using System.Collections.Generic;
using System.Collections.Immutable;

using GTTG.Core.Time;
using GTTG.Model.Model.Events;

namespace SZDC.Editor.Interfaces {

    /// <summary>
    /// Contract for receiving updates of content of dynamic timetable.
    /// </summary>
    public interface IDynamicDataReceiver {

        /// <summary>
        /// Provides timetable content description of the receiver.
        /// </summary>
        RailwaySegmentDetailedDescription ReceiverDescription { get; }

        /// <summary>
        /// Updates train timetable of receiver with new content time interval and set od trains to be visualized.s 
        /// </summary>
        void Update(DateTimeInterval contentInterval, Dictionary<int, (StaticTrainDescription, ImmutableArray<TrainEvent>)> schedules);

        /// <summary>
        /// Updates train timetable with updates of schedule of particular train.
        /// </summary>
        void Modify(int trainNumber, ImmutableArray<TrainEvent> schedule);
    }
}
