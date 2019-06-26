using System;
using System.Windows;
using SZDC.Editor.TrainTimetables;

namespace SZDC.Wpf.TrainGraph {

    /// <summary>
    /// Interaction logic for StaticTrainGraphWindow.xaml
    /// </summary>
    public partial class StaticTrainGraphWindow : Window {

        public StaticTrainGraphWindow() {
            InitializeComponent();
            Closed += OnClosed;
        }
        
        private void OnClosed(object sender, EventArgs e) {

            var trainGraph = (StaticTrainTimetable) DataContext;
            trainGraph?.Dispose();
        }
    }
}
