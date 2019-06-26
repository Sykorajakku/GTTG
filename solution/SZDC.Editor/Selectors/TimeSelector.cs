using System.Collections.Generic;

using GTTG.Core.Base;
using GTTG.Core.Time;
using SZDC.Editor.Interfaces;
using SZDC.Editor.TrainTimetables;

namespace SZDC.Editor.Selectors {

    /// <summary>
    /// Manages different states of time interval selection by selected mode.
    /// </summary>
    public class TimeSelector : ObservableObject {

        private TrainTimetableType _timetableType;
        private ITimeSelector _currentSelector;
        private readonly Dictionary<TrainTimetableType, ITimeSelector> _selectorsByTimetableType;

        public ITimeSelector CurrentSelector {
            get => _currentSelector;
            private set => Update(ref _currentSelector, value);
        }

        public TrainTimetableType TimetableType {
            get => _timetableType;
            set {
                Update(ref _timetableType, value);
                CurrentSelector = _selectorsByTimetableType[value];
            }
        }

        public TimeSelector() {
            
            var viewHours = new[] {2, 3, 4, 6, 12, 16};
            var dayHours = new List<DayHoursInterval> {
                new DayHoursInterval(0, 12, viewHours),
                new DayHoursInterval(12, 24, viewHours),
                new DayHoursInterval(0, 24, viewHours)
            };

            _selectorsByTimetableType = new Dictionary<TrainTimetableType, ITimeSelector> {
                {TrainTimetableType.Static, new StaticViewDayHoursSelector(dayHours)},
                {TrainTimetableType.Realtime, new DynamicSelector() }
            };
            TimetableType = TrainTimetableType.Static;
        }
    }
}
