using Autofac;

using GTTG.Core.Component;
using GTTG.Core.Drawing.Canvases;
using GTTG.Core.Drawing.Layers;
using GTTG.Core.HitTest;
using SZDC.Editor.Implementations;
using SZDC.Editor.Interfaces;
using SZDC.Editor.Layers;
using SZDC.Editor.TrainTimetables;
using SZDC.Model.Views;

namespace SZDC.Editor.Modules.Core {

    public class CoreModule : Module {

        protected override void Load(ContainerBuilder builder) {

            builder
                .RegisterType<StaticTrainTimetable>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<GraphicalComponent>()
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<DrawingManager>()
                .AsSelf()
                .As<DrawingManager>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<HitTestManager>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<ModifiableViewModel>()
                .AsSelf()
                .As<IViewModel>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<CanvasFactory>()
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<ViewModifiedNotifier>()
                .AsSelf()
                .As<IViewModifiedNotifier>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<StaticLayersOrder>()
                .As<IRegisteredLayersOrder>()
                .SingleInstance();
        }
    }

    public class DynamicCoreModule : Module {

        protected override void Load(ContainerBuilder builder) {

            builder
                .RegisterType<DynamicTrainTimetable>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<GraphicalComponent>()
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<DrawingManager>()
                .AsSelf()
                .As<DrawingManager>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<HitTestManager>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<ModifiableViewModel>()
                .AsSelf()
                .As<IViewModel>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<ViewModifiedNotifier>()
                .AsSelf()
                .As<IViewModifiedNotifier>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<CanvasFactory>()
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<DynamicLayersOrder>()
                .As<IRegisteredLayersOrder>()
                .SingleInstance();
        }
    }
}
