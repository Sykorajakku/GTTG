using System.Windows;
using System.Windows.Controls;
using SZDC.Editor.Tools;
using SZDC.Editor.TrainTimetables;

namespace SZDC.Wpf.Modifiers {

    /// <summary>
    /// Interaction logic for TrainModifier.xaml
    /// </summary>
    public partial class TrainModifier : Button {

        private TrainModificationTool _trainModificationTool;

        public TrainModifier() {
            InitializeComponent();
            Loaded += OnLoaded;
            Click += OnClick;
        }

        private void OnClick(object sender, RoutedEventArgs e) {
            _trainModificationTool.TryChangeState();
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {

            if (DataContext is DynamicTrainTimetable trainGraph) {
                _trainModificationTool = trainGraph.Tools.TrainModificationTool;
            }
        }
    }
}
