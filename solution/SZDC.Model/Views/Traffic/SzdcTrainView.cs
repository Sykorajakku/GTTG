using System;
using System.Collections.Generic;
using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Drawing.Canvases;
using GTTG.Model.Lines;
using GTTG.Model.Model.Events;
using GTTG.Model.Strategies;
using GTTG.Model.Strategies.Types;
using GTTG.Model.ViewModel.Traffic;
using SZDC.Model.Components;
using SZDC.Model.Events;
using SZDC.Model.Factories;
using SZDC.Model.Infrastructure.Trains;

namespace SZDC.Model.Views.Traffic {

    public class SzdcTrainView : StrategyTrainView<SzdcStrategy, SzdcTrain> {

        private bool _isSelected;

        public bool IsSelected {
            get => _isSelected;
            set => Update(ref _isSelected, value);
        }

        public SKColor PathColor => TrainPath.PathColor;

        public ITrainPath TrainMovementPoints { get; }

        public SzdcTrainView(SzdcTrain train,
                             ITrainPath trainPath,
                             SzdcStrategy strategy) 
            
            : base(train, trainPath, strategy) {

            _isSelected = false;
            TrainMovementPoints = trainPath;
            Train.PropertyChanged += TrainOnPropertyChanged;
            OnLoaded();
        }

        private void OnLoaded() {
            UpdateTrainViewContent();
        }

        private void TrainOnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            UpdateTrainViewContent();
        }

        protected override void OnDraw(DrawingCanvas drawingCanvas) {

            var defaultColor = TrainPath.PathColor;
            if (IsSelected) {
                TrainPath.PathColor = ViewConstants.SelectedTrainPathColor;
            }
            base.OnDraw(drawingCanvas);

            TrainPath.PathColor = defaultColor;
        }
        
        public override void UpdateTrainViewContent() {

            base.UpdateTrainViewContent();
            UpdateEventFlags();

            foreach (var trainEvent in Train.Schedule) {

                if (!(trainEvent is SzdcTrainEvent trainMovementEvent)) continue;

                var movementEventType = new TrainEventPlacement(trainMovementEvent, AnglePlacement.Acute);
                var container = new Container(ElementsOrder.FirstFromRight);

                foreach (var component in ProviderComponents(trainMovementEvent)) {
                    container.AddComponent(component);
                }

                Strategy.TrackStrategyManager.Add(movementEventType, container);
            }

            Strategy.TrainNumberPlacementSelector.AddTrainNumberComponentsTo(this);
            RescaleContainers(IsSelected ? 1f : 0.75f);
            Strategy.Dock();
            
            // As new elements provided to strategy, push layers to them
            Strategy.PopDrawingLayer();
            Strategy.PushDrawingLayer(CurrentDrawingLayer);
        }

        private void UpdateEventFlags() {

            for (var i = 0; i < Train.Schedule.Length; ++i) {

                if (!(Train.Schedule[i] is SzdcTrainEvent trainEvent)) {
                    continue;
                }

                if (trainEvent.TrainEventType == TrainEventType.Arrival && HasNextDepartureUnder30Seconds(trainEvent, i)) {

                    trainEvent.TrainEventFlags = TrainEventFlags.LessThanHalfMinute;
                    continue;
                }

                trainEvent.TrainEventFlags = TrainEventFlags.None;
            }
        }

        private bool HasNextDepartureUnder30Seconds(SzdcTrainEvent trainEvent, int index) {

            var nextTrainEvent = GetNextMovementEvent(index);
            if (nextTrainEvent == null) return false;

            if (nextTrainEvent.TrainEventType == TrainEventType.Departure) {
                return IsLessThan30Seconds(trainEvent.DateTime, nextTrainEvent.DateTime);
            }
            return false;
        }

        private bool IsLessThan30Seconds(DateTime firstDateTime, DateTime secondDateTime) {

            if (firstDateTime.Date != secondDateTime.Date) {
                return false;
            }

            if (firstDateTime.Hour != secondDateTime.Hour) {
                return false;
            }

            if (firstDateTime.Minute != secondDateTime.Minute) {
                return false;
            }

            return Math.Abs(firstDateTime.Second - secondDateTime.Second) < 30;
        }

        private SzdcTrainEvent GetNextMovementEvent(int start) {

            if (Train.Schedule.Length == start + 1) return null;

            for (int index = start + 1; index < Train.Schedule.Length; ++index) {

                if (Train.Schedule[index] is SzdcTrainEvent trainEvent) {
                    return trainEvent;
                }
            }
            return null;
        }
        
        public override void Arrange() {
            
            var scale = IsSelected ? 1f : 0.75f;
            RescaleContainers(scale);
            base.Arrange();
        }

        public void Select() {

            IsSelected = true;
            Arrange();
        }

        public void DeselectTrain() {

            IsSelected = false;
            Arrange();
        }

        public void OnTrainViewRemove() {

            TrainMovementPoints.Clear();
            Strategy.StationStrategyManager.Clear();
            Strategy.TrackStrategyManager.Clear();
        }

        private void RescaleContainers(float scale) {

            foreach (var component in Strategy.TrackStrategyManager.Values) {

                component.Measure(new SKSize(float.PositiveInfinity, float.PositiveInfinity));
                component.Arrange(SKPoint.Empty, component.DesiredSize);
                component.Scale(scale);
            }

            foreach (var containers in Strategy.StationStrategyManager.Values) {

                containers.Measure(new SKSize(float.PositiveInfinity, float.PositiveInfinity));
                containers.Arrange(SKPoint.Empty, containers.DesiredSize);
                containers.Scale(scale);
            }
        }
        
        private IEnumerable<ViewElement> ProviderComponents(SzdcTrainEvent trainEvent) {
            
            if (trainEvent.TrainEventFlags == TrainEventFlags.None) {
                yield return new TimeComponent(this, trainEvent);
            }

            if (trainEvent.TrainEventFlags == TrainEventFlags.LessThanHalfMinute) {
                yield return new TriangleComponent(this);
            }

            if (trainEvent.TrainEventFlags == TrainEventFlags.IsCancelled) {
                yield return new CircumscribedTimeComponent(this, trainEvent);
            }
        }
    }
}
