using System;
using System.Threading;
using System.Windows;
using Autofac;

using SZDC.Editor;
using SZDC.Wpf.Editor;
using SZDC.Wpf.Modules;

namespace SZDC.Wpf {

    public partial class App {

        protected override void OnStartup(StartupEventArgs e) {

            base.OnStartup(e);

            var builder = new ContainerBuilder();
            builder.RegisterModule<CoreModule>();
            builder.RegisterModule<ViewModule>();
            builder.RegisterModule<LocatorModule>();
            builder.RegisterModule<DataModule>();
            builder.RegisterInstance(SynchronizationContext.Current).As<SynchronizationContext>();

            using (var container = builder.Build()) {

                ProjectEditorCommands.Initialize(container.Resolve<IServiceProvider>());
                Start(container.Resolve<IServiceProvider>());
            }
        }

        private static void Start(IServiceProvider serviceProvider) {

            var editor = serviceProvider.GetService<ApplicationEditor>();
            var window = serviceProvider.GetService<MainWindow>();

            window.DataContext = editor;
            window.ShowDialog();
        }
    }
}
