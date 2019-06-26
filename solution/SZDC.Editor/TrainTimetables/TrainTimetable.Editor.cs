using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using SkiaSharp;

using GTTG.Core.Component;
using GTTG.Core.Time;
using GTTG.Core.Utils;
using GTTG.Model.Model.Events;
using SZDC.Editor.Interfaces;
using SZDC.Editor.Layers;
using SZDC.Editor.ModelProviders;
using SZDC.Editor.Tools;
using SZDC.Model.Infrastructure;
using SZDC.Model.Infrastructure.Traffic;
using SZDC.Model.Infrastructure.Trains;
using SZDC.Model.Layers;
using SZDC.Model.Views.Infrastructure;
using SZDC.Model.Views.Traffic;

namespace SZDC.Editor.TrainTimetables {

    public abstract partial class TrainTimetable {
        
        protected void LayoutAndDisplay() {
            Layout();
            ViewModifiedNotifier.NotifyViewChange();
        }

        public void ChangeStationViewToggleState(SzdcStationView stationView) {
            stationView.ChangeState();
            LayoutAndDisplay();
        }

        public void SetDpiScale(float dpi) {
            GraphicalComponent.DpiScale = dpi;
            LayoutAndDisplay();
        }

        public void ResizeView(float width, float height) {

            Height = height;
            var result = GraphicalComponent.TryResizeView(width, height);

            if (result == ResizeTransformationResult.ViewUnmodified) {
                return;
            }
            LayoutAndDisplay();
        }


        public void ChangeContentTimeInterval(DateTimeInterval newContentTimeInterval) {

            var currentViewTimeInterval = GraphicalComponent.DateTimeContext.ContentDateTimeInterval;
            if (currentViewTimeInterval.Start < newContentTimeInterval.Start) {
                var currentViewTimeSpan = currentViewTimeInterval.TimeSpan;
                currentViewTimeInterval = new DateTimeInterval(newContentTimeInterval.Start, newContentTimeInterval.Start.Add(currentViewTimeSpan));
            }

            var newDateTimeContext = new DateTimeContext(newContentTimeInterval, currentViewTimeInterval);
            UpdateDateTimeContext(newDateTimeContext);
        }

        public void UpdateDateTimeContext(DateTimeContext dateTimeContext) {

            var result = GraphicalComponent.TryChangeDateTimeContext(dateTimeContext);
            if (result == TimeModificationResult.TimeModified) {
                LayoutAndDisplay();
            }
        }

        public bool ChangeViewTimeInterval(DateTimeInterval dateTimeInterval) {

            var result = GraphicalComponent.TryChangeViewTime(dateTimeInterval);

            if (result == TimeModificationResult.TimeModified) {
                LayoutAndDisplay();
                return true;
            }
            return false;
        }

        public bool ChangeViewTimeInterval(TimeInterval timeInterval) {

            if (timeInterval.TimeSpan > ViewProvider.DateTimeContext.ContentDateTimeInterval.TimeSpan) {
                return false;
            }

            var newViewStart = ViewProvider.DateTimeContext.ViewDateTimeInterval.Start;

            if (newViewStart + timeInterval.TimeSpan > ViewProvider.DateTimeContext.ContentDateTimeInterval.End) {
                newViewStart = ViewProvider.DateTimeContext.ContentDateTimeInterval.End - timeInterval.TimeSpan;
            }

            var newViewTimeInterval = new DateTimeInterval(newViewStart, newViewStart + timeInterval.TimeSpan);
            return ChangeViewTimeInterval(newViewTimeInterval);
        }


        protected virtual void Layout() {
            LayoutRailwayView();
            LayoutTrafficView();
        }

        protected virtual void LayoutRailwayView() {

            var railwayView = ViewModel.RailwayView;

            if (railwayView == null) {
                return;
            }

            ViewModel.RailwayView.Measure(LayoutConstants.InfiniteSize);
            // ContentHeight is equal to DesiredSize of railway unless lower than Height
            var contentHeight = Math.Max(Height, ViewModel.RailwayView.DesiredSize.Height);

            // Call resize only if ContentHeight would be changed
            if (Math.Abs(GraphicalComponent.ContentHeight - contentHeight) > 0.001) {
                GraphicalComponent.TryResizeContentArea(ViewProvider.ContentWidth, contentHeight);
            }

            railwayView.Arrange(SKPoint.Empty, new SKSize(float.MaxValue, contentHeight));
        }

        protected virtual void LayoutTrafficView() {

            if (ViewModel.TrafficView == null) {
                return;
            }

            foreach (var trainView in ViewModel.TrafficView.TrainViews) {
                trainView.Arrange();
            }
        }

        protected virtual void DisposeTrafficView() {

            if (ViewModel.TrafficView != null) {

                foreach (var trainView in ViewModel.TrafficView.TrainViews) {
                    RemoveTrainView(trainView, false);
                }
            }

            ViewModifiedNotifier.NotifyViewChange();
        }

        public void ChangeRailwayView(SzdcRailway railway) {

            ViewModel.RailwayView = Factories.RailwayViewFactory.CreateRailwayView(railway);
            DrawingManager.ReplaceRegisteredDrawingLayer(new InfrastructureLayer(ViewModel));
            LayoutAndDisplay();
        }

        public void ChangeTrafficView(Dictionary<int, (StaticTrainDescription, ImmutableArray<TrainEvent>)> schedules) {

            var trainSchedules = new Dictionary<int, SzdcTrainView>();

            foreach (var trainView in ViewModel.TrafficView.TrainViews) {

                var trainNumber = trainView.Train.TrainNumber;

                if (schedules.ContainsKey(trainNumber)) {

                    try {

                        var schedule = schedules[trainView.Train.TrainNumber].Item2;
                        var railway = (SzdcRailway) ViewModel.RailwayView.Railway;
                        trainSchedules.Add(trainView.Train.TrainNumber, trainView);

                        var (start, end) = ApplicationEditor.SanitizeSchedule(railway, schedule, trainNumber);
                        var scheduleSubset = ApplicationEditor.CreateScheduleSubset(railway, start, end, schedule);

                        trainView.Train.CompleteSchedule = schedule.ToImmutableList();
                        trainView.Train.Schedule = scheduleSubset.ToImmutableArray();
                    }
                    catch (ModelDefinitionException ex) {
                        Errors = Errors.Add(ex.Message);
                    }
                }
                else {
                    RemoveTrainView(trainView, false);
                }
            }

            var newTrains = new List<SzdcTrain>();

            foreach (var scheduleEntry in schedules) {
                if (!trainSchedules.ContainsKey(scheduleEntry.Key)) {

                    try {
                        var schedule = scheduleEntry.Value.Item2;
                        var staticTrain = scheduleEntry.Value.Item1;
                        var railway = (SzdcRailway) ViewModel.RailwayView.Railway;
                        var (start, end) = ApplicationEditor.SanitizeSchedule(railway, schedule, staticTrain.TrainNumber);
                        var scheduleSubset = ApplicationEditor.CreateScheduleSubset(railway, start, end, schedule);
                        var train = new SzdcTrain(scheduleEntry.Key, staticTrain.TrainType,
                            staticTrain.TrainDecorationType, start != 0, end != schedule.Length, scheduleSubset, schedule);
                        newTrains.Add(train);
                    }
                    catch (ModelDefinitionException ex) {
                        Errors = Errors.Add(ex.Message);
                    }
                }
            }

            ViewModel.TrafficView.Add(newTrains);

            foreach (var trainView in ViewModel.TrafficView.TrainViews) {
                trainView.Train.PropertyChanged += TrainOnPropertyChanged;
            }

            LayoutAndDisplay();
        }

        public void ChangeTrafficView(SzdcTraffic traffic) {

            if (ViewModel.TrafficView != null) {
                DisposeTrafficView();
            }

            ViewModel.TrafficView = Factories.TrafficViewFactory.CreateTrafficView(traffic);
            DrawingManager.ReplaceRegisteredDrawingLayer(new TrafficLayer(ViewModel));

            foreach (var trainView in ViewModel.TrafficView.TrainViews) {
                trainView.Train.PropertyChanged += TrainOnPropertyChanged;
            }

            LayoutAndDisplay();
        }

        protected virtual void RemoveTrainView(SzdcTrainView trainView, bool changeView) {

            ViewModel.TrafficView.TrainViews = ViewModel.TrafficView.TrainViews.Remove(trainView);
            trainView.OnTrainViewRemove(); // clear from strategies structures
            trainView.Train.PropertyChanged -= TrainOnPropertyChanged;

            if (changeView) {
                ViewModifiedNotifier.NotifyViewChange();
            }
        }

        protected void TrainOnPropertyChanged(object sender, PropertyChangedEventArgs e) {
            ViewModifiedNotifier.NotifyViewChange();
        }

        public void SelectTrainView(SzdcTrainView trainView) {

            var previouslySelectedTrain = Tools.TrainSelectionTool.PreviouslySelectedTrainView;
            if (previouslySelectedTrain != null) {

                previouslySelectedTrain.DeselectTrain();
                previouslySelectedTrain.PopDrawingLayer();
            }

            var newlySelectedTrain = Tools.TrainSelectionTool.SelectedTrainView;
            if (newlySelectedTrain != null) {

                newlySelectedTrain.Select();
                newlySelectedTrain.PushDrawingLayer(DrawingManager.GetDrawingLayer<SelectedTrainLayer>());
            }

            ViewModifiedNotifier.NotifyViewChange();
        }

        private void TrainSelectionToolOnPropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(Tools.TrainSelectionTool.SelectedTrainView)) {
                SelectTrainView(Tools.TrainSelectionTool.SelectedTrainView);
            }
        }
    }
}
