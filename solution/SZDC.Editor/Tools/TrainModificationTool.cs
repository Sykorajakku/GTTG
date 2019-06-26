using System;
using System.Linq;
using SkiaSharp;

using GTTG.Core.Component;
using GTTG.Core.Utils;

using SZDC.Editor.Modules.Tools;
using SZDC.Editor.MouseInput;
using SZDC.Editor.Services;
using SZDC.Model.Events;

namespace SZDC.Editor.Tools {

    [TrainTimetableToolAttribute]
    public class TrainModificationTool : MouseInputTool {

        private bool _isModifying;
        private readonly EventManagementService _eventManagementService;
        private readonly TrainViewSelectionTool _trainViewSelectionTool;
        private readonly IMouseInputSourceProvider _mouseInputSourceProvider;
        private readonly IViewProvider _viewTimeConverter;

        public bool IsModifying {
            get => _isModifying;
            set => Update(ref _isModifying, value);
        }

        public TrainModificationTool(IMouseInputSourceProvider mouseInputSourceProvider,
                                     IViewProvider viewTimeConverter,
                                     TrainViewSelectionTool trainViewSelectionTool,
                                     EventManagementService eventManagementService) 
            
            : base(mouseInputSourceProvider) {

            IsModifying = false;
            _eventManagementService = eventManagementService;
            _viewTimeConverter = viewTimeConverter;
            _trainViewSelectionTool = trainViewSelectionTool;
            _mouseInputSourceProvider = mouseInputSourceProvider;
        }

        protected override void OnEnableMouseInput() {

            Observers.Add(_mouseInputSourceProvider.MouseInputSource?.RightDown.Subscribe(TryModifyTrainEventDateTime));
        }

        public void TryChangeState() {

            if (!IsModifying) {
                IsModifying = ModifySelectedTrain();
            }
            else {
                SaveSelectedTrain();
                IsModifying = false;
            }
        }

        private bool ModifySelectedTrain() {

            if (!_trainViewSelectionTool.IsEnabled || _trainViewSelectionTool.SelectedTrainView == null) {
                return false;
            }

            _trainViewSelectionTool.Disable();
            EnableMouseInput();
            return true;
        }

        private void SaveSelectedTrain() {

            DisableMouseInput();
            _trainViewSelectionTool.Enable();
        }

        private void TryModifyTrainEventDateTime(MouseInputArgs inputArgs) {

            var globalHorizontalPosition = _viewTimeConverter.ConvertViewToContentLocation(new SKPoint(inputArgs.X, inputArgs.Y));
            var inputArgsDateTime = _viewTimeConverter.GetDateTimeFromContent(globalHorizontalPosition.X);

            if (inputArgsDateTime <= DateTime.Now) {
                return; // only allow future modifications 
            }
 
            var trainView = _trainViewSelectionTool.SelectedTrainView;
            var train = trainView.Train;
            var trainMovementEvents = train.Schedule;
            
            SzdcTrainEvent latestBeforeClickDate = null;
            SzdcTrainEvent firstAfterClickDate = null;

            foreach (var trainMovementEvent in trainMovementEvents.Cast<SzdcTrainEvent>()) {

                if (trainMovementEvent.DateTime <= inputArgsDateTime) {
                    latestBeforeClickDate = trainMovementEvent;
                }

                if (firstAfterClickDate == null && trainMovementEvent.DateTime > inputArgsDateTime) {
                    firstAfterClickDate = trainMovementEvent;
                }
            }

            if (latestBeforeClickDate == null && firstAfterClickDate == null) {
                return;
            }

            SzdcTrainEvent modifiedEvent = null;

            if (latestBeforeClickDate == null) {
                modifiedEvent = firstAfterClickDate;
            }
            else if (firstAfterClickDate == null) {
                modifiedEvent = latestBeforeClickDate;
            }
            else {

                var firstAfterEventPoint =
                    trainView.TrainMovementPoints.PointsByTrainPathEvents[firstAfterClickDate].PathPoint;
                var firstAfterDistance =
                    PlacementUtils.ComputesVectorLength(firstAfterEventPoint - globalHorizontalPosition);

                var latestBeforeEventPoint =
                    trainView.TrainMovementPoints.PointsByTrainPathEvents[latestBeforeClickDate].PathPoint;
                var latestBeforeDistance =
                    PlacementUtils.ComputesVectorLength(latestBeforeEventPoint - globalHorizontalPosition);

                modifiedEvent = firstAfterDistance < latestBeforeDistance ? firstAfterClickDate : latestBeforeClickDate;
            }

            _eventManagementService.ModifyTrainScheduleEventDateTime(train, modifiedEvent, inputArgsDateTime);
        }
    }
}
