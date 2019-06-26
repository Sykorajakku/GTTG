using System;
using Autofac;

using SZDC.Editor.Locator;
using SZDC.Editor.Services;

namespace SZDC.Wpf.Modules {

    public class LocatorModule : Module {

        protected override void Load(ContainerBuilder builder) {
            builder.RegisterType<AutofacServiceProvider>().As<IServiceProvider>().InstancePerLifetimeScope();
            builder.RegisterType<StaticTimetableServiceLocator>().As<StaticTimetableServiceLocator>().InstancePerLifetimeScope();
            builder.RegisterType<DynamicTimetableServiceLocator>().As<DynamicTimetableServiceLocator>().InstancePerLifetimeScope();
        }
    }
}
