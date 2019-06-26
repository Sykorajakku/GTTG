using Autofac;

using SZDC.Editor.Components;

namespace SZDC.Editor.Implementations {

    /// <summary>
    /// Collects all components which are connectable to some standalone UI element.
    /// </summary>
    public class ComponentsCollector {

        public StationsSidebarComponent StationsSidebarComponent { get; }

        public TimeSidebarComponent TimeSidebarComponent { get; }

        public TrainTimetableComponent TrainTimetableComponent { get; }

        public RailwayDistanceSidebarComponent RailwayDistanceComponent { get; }

        public ComponentsCollector(IComponentContext componentContext) {

            TrainTimetableComponent = componentContext.Resolve<TrainTimetableComponent>();
            StationsSidebarComponent = componentContext.Resolve<StationsSidebarComponent>();
            TimeSidebarComponent = componentContext.Resolve<TimeSidebarComponent>();
            RailwayDistanceComponent = componentContext.Resolve<RailwayDistanceSidebarComponent>();
        }
    }
}
