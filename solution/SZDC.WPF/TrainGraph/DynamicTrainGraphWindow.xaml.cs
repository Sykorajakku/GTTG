using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using GTTG.Core.Time;
using GTTG.Model.Model.Events;
using SZDC.Editor;
using SZDC.Editor.Interfaces;
using SZDC.Editor.TrainTimetables;

namespace SZDC.Wpf.TrainGraph {

    /// <summary>
    /// Interaction logic for DynamicTrainGraphWindow.xaml
    /// </summary>
    public partial class DynamicTrainGraphWindow : Window, IDynamicDataReceiver {

        private CancellationTokenSource _cancellationTokenSource;
        private DynamicTrainTimetable _dynamicTrainTimetable;

        public ApplicationEditor ApplicationEditor { get; set; }

        public DynamicTrainGraphWindow() {

            InitializeComponent();
            Loaded += OnLoaded;
            Closed += OnClosed;
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {

            if (DataContext is DynamicTrainTimetable dynamicTrainTimetable && ApplicationEditor != null) {

                _dynamicTrainTimetable = dynamicTrainTimetable;
                var interval = ApplicationEditor.DynamicDataProvider.CurrentTimeInterval;
                _dynamicTrainTimetable.UpdateDateTimeContext(new DateTimeContext(interval, interval));
                _dynamicTrainTimetable.EventManagementService.ScheduleChanged += EventManagementServiceOnScheduleChanged;
                ReceiverDescription = dynamicTrainTimetable.TimetableInfo.RailwaySegmentDetailedDescription;
                ApplicationEditor.DynamicDataProvider.Register(this);
                
                _cancellationTokenSource = new CancellationTokenSource();

                var synchronizationContext = SynchronizationContext.Current;
                Task.Factory.StartNew(() => {

                        while (!_cancellationTokenSource.IsCancellationRequested) {

                            Thread.Sleep(60 * 1000); // 1 minute
                            synchronizationContext.Send((state) => {
                                _dynamicTrainTimetable.UpdateDynamicContext();
                            }, null);
                        }

                    }, _cancellationTokenSource.Token,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default);
            }
        }

        private void EventManagementServiceOnScheduleChanged(int arg1, ImmutableList<TrainEvent> arg2) {
            ApplicationEditor.DynamicDataProvider.Modify(arg1, arg2.ToImmutableArray());
        }

        private void OnClosed(object sender, EventArgs e) {
            ApplicationEditor.DynamicDataProvider.RemoveReceiver(this);
            _cancellationTokenSource?.Cancel();
        }

        public RailwaySegmentDetailedDescription ReceiverDescription { get; set; }

        public void Update(DateTimeInterval contentInterval, Dictionary<int, (StaticTrainDescription, ImmutableArray<TrainEvent>)> schedules) {
            _dynamicTrainTimetable.ChangeContentTimeInterval(contentInterval);
            _dynamicTrainTimetable.ChangeTrafficView(schedules); // add / removes train views to match current schedules
        }

        public void Modify(int trainNumber, ImmutableArray<TrainEvent> schedule) {
            _dynamicTrainTimetable.Modify(trainNumber, schedule);
        }
    }
}
