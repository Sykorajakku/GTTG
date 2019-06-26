using System;
using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Component;
using GTTG.Core.HitTest;
using SZDC.Editor.Modules.Tools;
using SZDC.Editor.MouseInput;
using SZDC.Model.Components;
using SZDC.Model.Views;

namespace SZDC.Editor.Tools {

    [TrainTimetableToolAttribute]
    public class ComponentsTool : MouseInputTool {

        private TimeComponent _hitTestedTimeComponent;
        private readonly IViewModel _viewModel;
        private readonly HitTestManager _hitTestManager;
        private readonly IMouseInputSourceProvider _mouseInputSourceProvider;
        private readonly IViewProvider _viewProvider;

        public TimeComponent HitTestedTimeComponent {
            get => _hitTestedTimeComponent;
            set => Update(ref _hitTestedTimeComponent, value);
        }

        public ComponentsTool(IViewModel viewModel,
                              HitTestManager hitTestManager,
                              IMouseInputSourceProvider mouseInputSourceProvider,
                              IViewProvider viewProvider) 
            
            : base(mouseInputSourceProvider) {

            _viewModel = viewModel;
            _hitTestManager = hitTestManager;
            _viewProvider = viewProvider;
            _mouseInputSourceProvider = mouseInputSourceProvider;
        }

        protected override void OnEnableMouseInput() {

            var mouseInputSource = _mouseInputSourceProvider.MouseInputSource;
            Observers.Add(mouseInputSource?.LeftDown.Subscribe(ProcessLeftDownClick));
        }

        private void ProcessLeftDownClick(MouseInputArgs mouseInputArgs) {

            if (_viewModel.TrafficView == null) return;

            var viewLocation = new SKPoint(mouseInputArgs.X, mouseInputArgs.Y);
            var contentLocation = _viewProvider.ConvertViewToContentLocation(viewLocation);

            TimeComponent timeComponent = null;

            HitTestResultBehavior ResultCallback(IVisual element) {
                if (element is TimeComponent component) {
                    timeComponent = component;
                }
                return HitTestResultBehavior.Continue;
            }

            _hitTestManager.HitTest((_,__) => HitTestFilterBehavior.Continue, ResultCallback, contentLocation);
            HitTestedTimeComponent = timeComponent;
        }
    }
}
