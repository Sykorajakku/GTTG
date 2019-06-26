using System;
using SkiaSharp;

using GTTG.Core.Component;
using SZDC.Editor.Modules.Tools;
using SZDC.Editor.MouseInput;

namespace SZDC.Editor.Tools {

    [TrainTimetableToolAttribute]
    public class CurrentDateTimeTool : MouseInputTool {

        private DateTime? _currentDateTime;
        private bool _isEnabled;

        private readonly DragProcessor _dragProcessor;
        private readonly IViewProvider _viewTimeConverter;
        private readonly IMouseInputSourceProvider _mouseInputSourceProvider;

        public DateTime? CurrentDateTime {
            get => _isEnabled ? _currentDateTime : null;
            set => Update(ref _currentDateTime, value);
        }

        public CurrentDateTimeTool(IMouseInputSourceProvider mouseInputSourceProvider,
                                   IViewProvider viewTimeConverter)
            : base(mouseInputSourceProvider) {

            _mouseInputSourceProvider = mouseInputSourceProvider;
            _viewTimeConverter = viewTimeConverter;
            _dragProcessor = new DragProcessor();
        }

        protected override void OnEnableMouseInput() {

            var currentInputSource = _mouseInputSourceProvider.MouseInputSource;

            Observers.Add(currentInputSource?.LeftUp.Subscribe(LeftUp));
            Observers.Add(currentInputSource?.LeftDown.Subscribe(LeftDown));
            Observers.Add(currentInputSource?.Move.Subscribe(Move));
            Observers.Add(currentInputSource?.Leave.Subscribe(Leave));
            Observers.Add(currentInputSource?.Enter.Subscribe(Enter));

            _isEnabled = true;
        }

        public override void DisableMouseInput() {

            _dragProcessor.Reset();
            base.DisableMouseInput();

            _isEnabled = false;
        }

        public void LeftUp(MouseInputArgs args) {
            _dragProcessor.TryExitDrag(args);
        }

        public void LeftDown(MouseInputArgs args) {
            _dragProcessor.TryInitializeDrag(args);
        }

        public void Enter(MouseInputArgs args) {
            _isEnabled = true;
        }

        public void Move(MouseInputArgs args) {

            if (!_dragProcessor.IsEnabled) {

                var canvasHorizontalPosition = _viewTimeConverter.ConvertViewToContentLocation(new SKPoint(args.X, args.Y)).X;
                CurrentDateTime = _viewTimeConverter.GetDateTimeFromContent(canvasHorizontalPosition);
            }
        }

        public void Leave(MouseInputArgs args) {
            _dragProcessor.TryExitDrag(args);
            _isEnabled = false;
            CurrentDateTime = null;
        }
    }
}
