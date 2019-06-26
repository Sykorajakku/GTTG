using Autofac;

using SZDC.Editor;

namespace SZDC.Wpf.Modules {

    public class CoreModule : Module {

        protected override void Load(ContainerBuilder builder) {

            builder.RegisterType<ApplicationEditor>().As<ApplicationEditor>().SingleInstance();
        }
    }
}
