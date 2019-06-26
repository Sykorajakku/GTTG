using System.Windows;
using System.Windows.Controls;

using SZDC.Editor.Tools;
using SZDC.Editor.TrainTimetables;

namespace SZDC.Wpf.Modifiers {

    /// <summary>
    /// Interaction logic for ViewTimeConverter.xaml
    /// </summary>
    public partial class ViewTimeModifier : ComboBox {

        private TrainTimetable _trainTimetable;

        public ViewTimeModifier() {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {

            if (DataContext is TrainTimetable trainTimetable) {

                _trainTimetable = trainTimetable;
                SelectionChanged += TimeIntervalsComboBox_OnSelectionChanged;
            }
        }

        private void TimeIntervalsComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
            _trainTimetable.ChangeViewTimeInterval((TimeInterval) e.AddedItems[0]);
        }
    }
}
