using System;
using System.Collections.Immutable;

using GTTG.Model.Model.Events;
using SZDC.Model.Events;
using SZDC.Model.Infrastructure.Trains;

namespace SZDC.Editor.Services {

    /// <summary>
    /// Manages schedule change by specific implementation. If schedule modified, event raised.
    /// </summary>
    public interface IEventManagementService {

        /// <summary>
        /// Raised when some schedule changed with train number as first argument and new schedule as second.
        /// </summary>
        event Action<int, ImmutableList<TrainEvent>> ScheduleChanged;
    }

    public class EventManagementService : IEventManagementService {

        public void ModifyTrainScheduleEventDateTime(SzdcTrain train, SzdcTrainEvent modifiedEvent, DateTime newDateTime) {

            var scheduleIndex = train.Schedule.IndexOf(modifiedEvent);
            if (scheduleIndex == -1) {
                throw new ArgumentException($"Event {nameof(modifiedEvent)} {modifiedEvent} is not present in schedule of train { train }.");
            }
            
            var newMovementEvent = new SzdcTrainEvent(newDateTime, modifiedEvent.Station, modifiedEvent.Track, modifiedEvent.TrainEventType, modifiedEvent.TrainEventFlags);
            var newSchedule = train.CompleteSchedule.Replace(modifiedEvent, newMovementEvent);
            
            ScheduleChanged?.Invoke(train.TrainNumber, newSchedule);
        }

        public event Action<int, ImmutableList<TrainEvent>> ScheduleChanged;
    }
}
