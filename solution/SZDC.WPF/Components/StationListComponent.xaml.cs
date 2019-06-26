using System.ComponentModel;
using System.Windows;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

using SZDC.Editor.Components;
using SZDC.Editor.MouseInput;
using SZDC.Editor.TrainTimetables;
using SZDC.Wpf.Input;

namespace SZDC.Wpf.Components {
    
    /// <summary>
    /// Interaction logic for StationListComponent.xaml
    /// </summary>
    public partial class StationListComponent : SKElement {

        private StationsSidebarComponent _stationsSidebar;
        
        /* To send toggle / collapse commands to UI */
        private TrainTimetable _trainTimetable;

        public StationListComponent() {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs eventArgs) {

            eventArgs.Surface.Canvas.Clear();
            _stationsSidebar?.Draw(eventArgs.Surface);            
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {

            if (DataContext is TrainTimetable trainTimetable &&
                trainTimetable.Components?.StationsSidebarComponent != null) {

                _trainTimetable = trainTimetable;
                _stationsSidebar = trainTimetable.Components.StationsSidebarComponent;

                // receive changes of view modification of (also when station is toggled / collapsed)
                _stationsSidebar.ViewModified += InvalidateVisual;
                // receive toggle changes
                _stationsSidebar.PropertyChanged += OnStationViewChanged;
                // connect this UI mouse input to receive collapse / toggle clicks
                MouseInputConnector.Connect(new WpfMouseInputSource(this), _stationsSidebar);
            }
        }

        private void OnStationViewChanged(object sender, PropertyChangedEventArgs e) {

            if (e.PropertyName == nameof(StationsSidebarComponent.ToggledStation)) {
                // command to application logic, moved here as stations sidebar does not have access to app. logic
                _trainTimetable.ChangeStationViewToggleState(_stationsSidebar.ToggledStation);
            }
        }
    }
}
