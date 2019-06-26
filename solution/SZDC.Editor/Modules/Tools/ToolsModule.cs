using Autofac;

namespace SZDC.Editor.Modules.Tools {

    public class ToolsModule : Module {

        protected override void Load(ContainerBuilder builder) {

            builder
                .RegisterAssemblyTypes(ThisAssembly)
                .Where(t => t.IsDefined(typeof(TrainTimetableToolAttributeAttribute), false))
                .InstancePerLifetimeScope();
        }
    }
}
