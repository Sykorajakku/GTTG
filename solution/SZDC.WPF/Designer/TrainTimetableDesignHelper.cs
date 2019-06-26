using System.Collections.Generic;
using Autofac;
using Autofac.Core;

using SZDC.Data;
using SZDC.Editor.Designer;
using SZDC.Editor.Locator;
using SZDC.Wpf.Modules;
using CoreModule = SZDC.Editor.Modules.Core.CoreModule;

namespace SZDC.Wpf.Designer {

    public class TrainTimetableDesignHelper
        : TrainTimetableDesignerContext {

        public TrainTimetableDesignHelper()
            : base(new TimetableContext()) {
        }
    }

    public class TimetableContext : IComponentContext {

        private readonly IComponentContext _trainTimetableComponentContext;

        public TimetableContext() {

            var builder = new ContainerBuilder();

            builder.RegisterModule<CoreModule>();
            builder.RegisterModule<LocatorModule>();

            builder
                .RegisterType<SzdcContext>()
                .WithParameter("options", TestContextOptionsProvider.Get())
                .InstancePerLifetimeScope();

            var container = builder.Build();
            _trainTimetableComponentContext =
                container.Resolve<StaticTimetableServiceLocator>().GetScopedServiceLocator();
        }

        public object ResolveComponent(IComponentRegistration registration, IEnumerable<Parameter> parameters) {
            return _trainTimetableComponentContext.ResolveComponent(registration, parameters);
        }

        public IComponentRegistry ComponentRegistry => _trainTimetableComponentContext.ComponentRegistry;
    }
}
