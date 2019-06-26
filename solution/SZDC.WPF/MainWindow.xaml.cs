using System.Windows;

namespace SZDC.Wpf {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow() {
            InitializeComponent();
            
            // close all windows
            Closed += (_, __) => Application.Current.Shutdown();
        }
    }
}
