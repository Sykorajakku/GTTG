using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;

using GTTG.Core.Base;
using GTTG.Core.Time;
using SZDC.Editor.Interfaces;

namespace SZDC.Editor.Selectors {

    /// <summary>
    /// Manages selection of time intervals of opened timetable.
    /// </summary>
    public class StaticViewDayHoursSelector : ObservableObject, ITimeSelector {

        private ImmutableArray<DayHoursInterval> _availableViewHours;
        private ImmutableArray<int> _availableWindowHours;
        private ImmutableArray<int> _startHours;
        private DayHoursInterval _selectedViewHours;
        private int _selectedWindowHour;
        private int _startHour;

        /// <summary>
        /// Selected value from <see cref="AvailableViewHours"/>.
        /// </summary>
        public DayHoursInterval SelectedViewHours {
            get => _selectedViewHours;
            set => Update(ref _selectedViewHours, value, true);
        }

        /// <summary>
        /// Selected value from <see cref="AvailableWindowHours"/>.
        /// </summary>
        public int SelectedWindowHour {
            get => _selectedWindowHour;
            set => Update(ref _selectedWindowHour, value, true);
        }

        /// <summary>
        /// Timespan of viewable intervals. Sub-interval of those values are <see cref="AvailableWindowHours"/>.
        /// </summary>
        public ImmutableArray<DayHoursInterval> AvailableViewHours {
            get => _availableViewHours;
            set => Update(ref _availableViewHours, value, true);
        }
        
        /// <summary>
        /// Timespans displayable in window to user. 
        /// </summary>
        public ImmutableArray<int> AvailableWindowHours {
            get => _availableWindowHours;
            set => Update(ref _availableWindowHours, value, true);
        }

        /// <summary>
        /// Available hours where <see cref="SelectedWindowHour"/> can start int <see cref="SelectedViewHours"/>.
        /// </summary>
        public ImmutableArray<int> StartHours {
            get => _startHours;
            set => Update(ref _startHours, value, true);
        }

        /// <summary>
        /// Hour where <see cref="SelectedWindowHour"/> starts in <see cref="SelectedViewHours"/>.
        /// </summary>
        public int StartHour {
            get => _startHour;
            set => Update(ref _startHour, value, true);
        }

        public static DayHoursInterval WholeDay = new DayHoursInterval(0, 24);

        public StaticViewDayHoursSelector(IEnumerable<DayHoursInterval> dayHourIntervals) {

            AvailableViewHours = ImmutableArray.CreateRange(dayHourIntervals.DefaultIfEmpty(WholeDay));
            var defaultViewHours = AvailableViewHours[0];

            AvailableWindowHours = ImmutableArray.CreateRange(defaultViewHours.WindowHours);
            PropertyChanged += OnPropertyChanged;

            SelectedViewHours = defaultViewHours;
            SelectedWindowHour = SelectedViewHours.MaxWindowHour;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {

            if (e.PropertyName == nameof(SelectedViewHours)) {

                AvailableWindowHours = SelectedViewHours.WindowHours.ToImmutableArray();
                SelectedWindowHour = SelectedViewHours.MaxWindowHour;
            }

            if (e.PropertyName == nameof(SelectedWindowHour)) {

                var startHours = new List<int>();
                for (var i = SelectedViewHours.StartHour; i <= SelectedViewHours.EndHour - SelectedWindowHour; ++i) {
                    startHours.Add(i);
                }

                StartHours = startHours.ToImmutableArray();
                StartHour = SelectedViewHours.StartHour;
            }
        }

        /// <summary>
        /// Creates <see cref="DateTimeContext"/> from selected value.
        /// </summary>
        public DateTimeContext ToDateTimeContext() {

            var startDate = DateTime.Today.Date;
            var canvasInterval = new DateTimeInterval(startDate.AddHours(_selectedViewHours.StartHour), startDate.AddHours(_selectedViewHours.EndHour));
            var viewInterval = new DateTimeInterval(startDate.AddHours(StartHour), startDate.AddHours(StartHour + SelectedWindowHour));
            return new DateTimeContext(canvasInterval, viewInterval);
        }
    }
}
