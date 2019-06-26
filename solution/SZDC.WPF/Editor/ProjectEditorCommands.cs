using System;
using System.Windows;
using System.Windows.Input;

using SZDC.Editor;
using SZDC.Editor.TrainTimetables;
using SZDC.Wpf.TrainGraph;

namespace SZDC.Wpf.Editor {

    public static class ViewConnector {

        public static void Connect<TWindow, TDisposableContext>(IServiceProvider serviceProvider)
            where TWindow : Window, new() {

            var context = serviceProvider.GetService<TDisposableContext>();
            var window = new TWindow {DataContext = context};
            window.Show();
        }

        public static void ConnectDynamic(IServiceProvider serviceProvider, ApplicationEditor applicationEditor) {

            var context = serviceProvider.GetService<DynamicTrainTimetable>();
            var window = new DynamicTrainGraphWindow { DataContext = context, ApplicationEditor = applicationEditor };
            window.Show();
        }
    }

    public static class ProjectEditorCommands {

        public static ICommand OpenStaticTimetable { get; set; }

        public static ICommand OpenDynamicTimetable { get; set; }

        public static void Initialize(IServiceProvider serviceProvider) {

            OpenStaticTimetable = new Command(_ => true, _ => CreateStaticTimetable(serviceProvider));
            OpenDynamicTimetable = new Command(_ => true, _ => CreateDynamicTimetable(serviceProvider));
        }

        private static void CreateDynamicTimetable(IServiceProvider serviceProvider) {

            var projectEditor = serviceProvider.GetService<ApplicationEditor>();
            var scopedTimetable = projectEditor.OpenDynamicTrainTimetable();
            ViewConnector.ConnectDynamic(scopedTimetable, projectEditor);
        }

        private static void CreateStaticTimetable(IServiceProvider serviceProvider) {

            var projectEditor = serviceProvider.GetService<ApplicationEditor>();
            var scopedTimetable = projectEditor.OpenStaticTrainTimetable();
            ViewConnector.Connect<StaticTrainGraphWindow, StaticTrainTimetable>(scopedTimetable);
        }
    }
}
