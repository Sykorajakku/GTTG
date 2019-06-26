using Autofac;

namespace SZDC.Wpf.Modules {

    public class ViewModule : Module {

        protected override void Load(ContainerBuilder builder) {

            builder.RegisterType<MainWindow>()
                .As<MainWindow>()
                .SingleInstance();
        }
    }
}
