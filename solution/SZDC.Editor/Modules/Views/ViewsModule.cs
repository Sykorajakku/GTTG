using Autofac;

using SZDC.Editor.Components;
using SZDC.Editor.MouseInput;

namespace SZDC.Editor.Modules.Views {

    public class ViewsModule : Module {

        protected override void Load(ContainerBuilder builder) {

            builder
                .RegisterType<TrainTimetableComponent>()
                .As<IMouseInputSourceProvider>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<TimeSidebarComponent>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<StationsSidebarComponent>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<RailwayDistanceSidebarComponent>()
                .AsSelf()
                .InstancePerLifetimeScope();
        }
    }
}
