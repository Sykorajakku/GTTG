using Autofac;

namespace SZDC.Editor.Modules.Services {

    public class ServicesModule : Module {

        protected override void Load(ContainerBuilder builder) {

            builder
                .RegisterAssemblyTypes(ThisAssembly)
                .Where(t => t.Name.EndsWith("Service"))
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}

