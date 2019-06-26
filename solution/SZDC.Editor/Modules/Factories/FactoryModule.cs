using Autofac;

using SZDC.Model.Factories;

namespace SZDC.Editor.Modules.Factories {

    public class FactoryModule : Module {

        protected override void Load(ContainerBuilder builder) {

            builder
                .RegisterType<SzdcRailwayViewFactory>()
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<SzdcStationViewFactory>()
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<SzdcTrackViewFactory>()
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<SzdcTrainViewFactory>()
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<SzdcTrafficViewFactory>()
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
