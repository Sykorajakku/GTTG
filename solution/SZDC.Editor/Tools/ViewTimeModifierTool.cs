using System;
using System.Collections.Generic;
using System.ComponentModel;

using GTTG.Core.Base;
using GTTG.Core.Component;
using GTTG.Core.Time;
using SZDC.Editor.Modules.Tools;

namespace SZDC.Editor.Tools {

    public struct TimeInterval {

        public TimeInterval(int hours, int minutes) {
            Hours = hours;
            Minutes = minutes;
        }

        public int Hours { get; }
        public int Minutes { get; }
        
        public override string ToString() {
            if (Hours == 0) return $"{Minutes}m";
            return (Minutes != 0) ? $"{Hours}h {Minutes}m" : $"{Hours}h";
        }

        public TimeSpan TimeSpan => new TimeSpan(Hours, Minutes, 0);
    }

    [TrainTimetableToolAttribute]
    public class ViewTimeModifierTool : ObservableObject {

        private static readonly IReadOnlyCollection<TimeInterval> DefaultTimeIntervals =
            new List<TimeInterval> {
                new TimeInterval(hours: 0, minutes: 30),
                new TimeInterval(hours: 1, minutes: 0),
                new TimeInterval(hours: 2, minutes: 30),
                new TimeInterval(hours: 4, minutes: 0),
                new TimeInterval(hours: 6, minutes: 0),
                new TimeInterval(hours: 8, minutes: 0)
            };

        public List<TimeInterval> TimeIntervals {
            get => _timeIntervals;
            set => Update(ref _timeIntervals, value);
        }

        public TimeInterval SelectedTimeInterval {
            get => _selectedTimeInterval;
            set => Update(ref _selectedTimeInterval, value);
        }

        private TimeInterval _selectedTimeInterval;
        private List<TimeInterval> _timeIntervals;

        private readonly IViewProvider _viewTimeModifier;
        private DateTimeContext _currentDateTimeContext;

        public ViewTimeModifierTool(IViewProvider viewProvider) {

            _timeIntervals = new List<TimeInterval>();
            _viewTimeModifier = viewProvider;

            viewProvider.PropertyChanged += ViewTimeModifierOnPropertyChanged;
        }

        private TimeInterval DateTimeContextToInterval() {

            var hours = _viewTimeModifier.DateTimeContext.ViewDateTimeInterval.TimeSpan.Hours;
            var minutes = _viewTimeModifier.DateTimeContext.ViewDateTimeInterval.TimeSpan.Minutes;
            return new TimeInterval(hours, minutes);
        }

        
        private void ViewTimeModifierOnPropertyChanged(object sender, PropertyChangedEventArgs e) {

            if (e.PropertyName != nameof(_viewTimeModifier.DateTimeContext)) {
                return;
            }

            if (_currentDateTimeContext == null) {
                ProvideAvailableViewIntervals();
                _currentDateTimeContext = _viewTimeModifier.DateTimeContext;
                return;
            }

            var currentGlobal = _currentDateTimeContext.ContentDateTimeInterval;
            var newGlobal = _viewTimeModifier.DateTimeContext.ContentDateTimeInterval;

            if (currentGlobal.Equals(newGlobal)) {
                return;
            }

            ProvideAvailableViewIntervals();
            _currentDateTimeContext = _viewTimeModifier.DateTimeContext;
        }

        private void ProvideAvailableViewIntervals() {

            _timeIntervals.Clear();

            var borderDateTimeInterval = _viewTimeModifier.DateTimeContext.ContentDateTimeInterval;
            var borderDateTimeTimeSpan = borderDateTimeInterval.TimeSpan;

            foreach (var timeInterval in DefaultTimeIntervals) {

                if (borderDateTimeTimeSpan >= timeInterval.TimeSpan) {
                    _timeIntervals.Add(timeInterval);
                }
            }

            if (!_timeIntervals.Contains(DateTimeContextToInterval())) {
                _timeIntervals.Add(DateTimeContextToInterval());
            }

            _timeIntervals.Sort(CompareTimeIntervals);

            SelectedTimeInterval = DateTimeContextToInterval();
            TimeIntervals = _timeIntervals;
        }

        private static int CompareTimeIntervals(TimeInterval t1, TimeInterval t2) {

            if (t1.Hours == t2.Hours) {
                return (t1.Minutes < t2.Minutes) ? -1 : 1;
            }
            if (t1.Hours < t2.Hours) {
                return -1;
            }
            else {
                return 1;
            }
        }
    }
}
