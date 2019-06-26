using Autofac;

using SZDC.Data;
using SZDC.Editor.Interfaces;

namespace SZDC.Wpf.Modules {

    public class DataModule : Module {

        protected override void Load(ContainerBuilder builder) {

            builder
                .RegisterType<DbDataProvider>()
                .As<IStaticDataProvider>()
                .InstancePerLifetimeScope();
        }
    }
}
