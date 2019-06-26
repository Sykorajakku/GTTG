using System;
using SkiaSharp;

using GTTG.Core.Component;
using SZDC.Editor.Modules.Tools;
using SZDC.Editor.MouseInput;
using SZDC.Model.Views;
using SZDC.Model.Views.Traffic;

namespace SZDC.Editor.Tools {

    /// <summary>
    /// When enabled, selects nearest train relative to input target.
    /// </summary>
    [TrainTimetableToolAttribute]
    public class TrainViewSelectionTool : MouseInputTool {

        private SzdcTrainView _selectedTrainView;
        private SzdcTrainView _previouslySelectedTrainView;
        private bool _isEnabled;

        private readonly IViewProvider _viewConverter;
        private readonly IMouseInputSourceProvider _mouseInputSourceProvider;
        private readonly IViewModel _viewModel; 

        /// <summary>
        /// Currently selected train.
        /// </summary>
        public SzdcTrainView SelectedTrainView {
            get => _selectedTrainView;
            set {
                PreviouslySelectedTrainView = _selectedTrainView;
                Update(ref _selectedTrainView, value);
            }
        }

        /// <summary>
        /// Train being replaced as selected by <see cref="SelectedTrainView"/>.
        /// </summary>
        public SzdcTrainView PreviouslySelectedTrainView {
            get => _previouslySelectedTrainView;
            private set => Update(ref _previouslySelectedTrainView, value);
        }

        public bool IsEnabled {
            get => _isEnabled;
            set => Update(ref _isEnabled, value);
        }

        public TrainViewSelectionTool(IMouseInputSourceProvider mouseInputSourceProvider,
                                      IViewProvider viewConverter,
                                      IViewModel viewModel)

            : base(mouseInputSourceProvider) {

            _mouseInputSourceProvider = mouseInputSourceProvider;
            _viewConverter = viewConverter;
            _viewModel = viewModel;
            _isEnabled = false;
        }

        /// <summary>
        /// Sets <see cref="SelectedTrainView"/> as nearest train to location of <param name="mouseInputArgs"></param>.
        /// If no train found, null is set.
        /// </summary>
        /// <param name="mouseInputArgs">Contains location in current view, which is converted to canvas view.</param>
        private void SelectNearestTrain(MouseInputArgs mouseInputArgs) {

            var canvasLocation = new SKPoint(mouseInputArgs.X, mouseInputArgs.Y);
            var globalLocation = _viewConverter.ConvertViewToContentLocation(canvasLocation);

            SelectedTrainView = _viewModel.TrafficView.SelectNearestTrainView(globalLocation);
        }

        protected override void OnEnableMouseInput() {
            Observers.Add(_mouseInputSourceProvider.MouseInputSource?.RightDown.Subscribe(SelectNearestTrain));
        }

        public void Enable() {
            EnableMouseInput();
            IsEnabled = true;
        }

        public void Disable() {
            DisableMouseInput();
            IsEnabled = false;
        }
    }
}
