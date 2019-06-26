using System;

using SZDC.Editor.Modules.Tools;
using SZDC.Editor.MouseInput;

namespace SZDC.Editor.Tools {

    [TrainTimetableToolAttribute]
    public class CanvasTranslationCacheTool: MouseInputTool {

        private readonly IMouseInputSourceProvider _mouseInputSourceProvider;
        private readonly DragProcessor _dragProcessor;

        public CanvasTranslationCacheTool(IMouseInputSourceProvider mouseInputSourceProvider)

            : base(mouseInputSourceProvider) {

            _mouseInputSourceProvider = mouseInputSourceProvider;
            _dragProcessor = new DragProcessor();
        }

        protected override void OnEnableMouseInput() {

            var mouseInputSource = _mouseInputSourceProvider.MouseInputSource;

            Observers.Add(mouseInputSource?.LeftUp.Subscribe(OnExitDrag));
            Observers.Add(mouseInputSource?.LeftDown.Subscribe(OnInitDrag));
            Observers.Add(mouseInputSource?.Leave.Subscribe(OnExitDrag));
        }

        public void OnExitDrag(MouseInputArgs args) {
            _dragProcessor.TryExitDrag(args);
        }

        public void OnInitDrag(MouseInputArgs args) {
            _dragProcessor.TryInitializeDrag(args);
        }
    }
}
