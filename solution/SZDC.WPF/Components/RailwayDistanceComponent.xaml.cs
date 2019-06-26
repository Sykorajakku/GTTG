using System.Windows;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

using SZDC.Editor.Components;
using SZDC.Editor.TrainTimetables;

namespace SZDC.Wpf.Components {

    /// <summary>
    /// Interaction logic for RailwayDistanceComponent.xaml
    /// </summary>
    public partial class RailwayDistanceComponent : SKElement {

        /// <summary>
        /// Column with railway distance values to be drawn
        /// </summary>
        private RailwayDistanceSidebarComponent _railwayDistanceComponent;

        public RailwayDistanceComponent() {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs eventArgs) {

            eventArgs.Surface.Canvas.Clear();
            _railwayDistanceComponent?.Draw(eventArgs.Surface);
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {

            if (DataContext is TrainTimetable trainTimetable &&
                
                trainTimetable.Components?.TrainTimetableComponent != null) {

                _railwayDistanceComponent = trainTimetable.Components.RailwayDistanceComponent;
                // make this component redrawn when view modified
                _railwayDistanceComponent.ViewModified += InvalidateVisual;
            }
        }
    }
}
