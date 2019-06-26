using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

using SZDC.Editor.Components;
using SZDC.Editor.TrainTimetables;
using SZDC.Wpf.Input;

namespace SZDC.Wpf.Components {

    public partial class TrainGraphComponent : SKElement {

        private TrainTimetable _trainTimetable;
        private TrainTimetableComponent _trainTimetableComponent;

        public TrainGraphComponent() {

            InitializeComponent();
            Loaded += (sender, args) => OnLoaded();
            SizeChanged += (sender, args) => OnSizeChanged();
        }

        private void OnSizeChanged() {

            if (_trainTimetableComponent == null) return;
            _trainTimetable.ResizeView(CanvasSize.Width, CanvasSize.Height);
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs eventArgs) {

            using (var surface = eventArgs.Surface) {
                _trainTimetableComponent?.Draw(surface);
            }
        }

        private void OnLoaded() {

            if (DataContext is TrainTimetable trainTimetable &&
                trainTimetable.Components?.TrainTimetableComponent != null) {

                _trainTimetable = trainTimetable;
                _trainTimetableComponent = trainTimetable.Components.TrainTimetableComponent;

                // redraw request from application logic
                _trainTimetableComponent.ViewModified += InvalidateVisual; 
                // connect this UI mouse events with application logic
                _trainTimetableComponent.MouseInputSource = new WpfMouseInputSource(this);

                // use device pixels for graphical component
                trainTimetable.SetDpiScale((float) (CanvasSize.Width / ActualWidth));
                trainTimetable.ResizeView(CanvasSize.Width, CanvasSize.Height);
            }
        }
    }
}
