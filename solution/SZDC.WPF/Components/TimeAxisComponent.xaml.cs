using System.Windows;

using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using SZDC.Editor.Components;
using SZDC.Editor.TrainTimetables;

namespace SZDC.Wpf.Components {

    public partial class TimeAxisComponent : SKElement {

        private TimeSidebarComponent _timeSidebar;

        public TimeAxisComponent() {

            InitializeComponent();
            Loaded += OnLoaded;
        }
        
        protected override void OnPaintSurface(SKPaintSurfaceEventArgs eventArgs) {

            using (var surface = eventArgs.Surface) {
                _timeSidebar?.SetHeight(CanvasSize.Height); // Set height to application logic
                _timeSidebar?.Draw(surface);                
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {

            if (DataContext is TrainTimetable trainGraph &&
                trainGraph.Components?.TimeSidebarComponent != null) {

                _timeSidebar = trainGraph.Components.TimeSidebarComponent;
                _timeSidebar.ViewModified += InvalidateVisual; // redraw request from application logic
            }
        }
    }
}
