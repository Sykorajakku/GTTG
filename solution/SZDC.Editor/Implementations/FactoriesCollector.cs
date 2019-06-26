using Autofac;

using GTTG.Model.ViewModel.Infrastructure.Railways;
using GTTG.Model.ViewModel.Traffic;
using SZDC.Model.Factories;
using SZDC.Model.Infrastructure.Trains;
using SZDC.Model.Views.Infrastructure;
using SZDC.Model.Views.Traffic;

namespace SZDC.Editor.Implementations {

    /// <summary>
    /// Collects factories of view model.
    /// </summary>
    public class FactoriesCollector {

        public IRailwayViewFactory<SzdcRailwayView, SzdcStationView, SzdcTrackView> RailwayViewFactory { get; }
        public ITrafficViewFactory<SzdcTrafficView, SzdcTrainView, SzdcTrain> TrafficViewFactory { get; }

        public FactoriesCollector(IComponentContext componentContext) {

            RailwayViewFactory = componentContext.Resolve<SzdcRailwayViewFactory>();
            TrafficViewFactory = componentContext.Resolve<SzdcTrafficViewFactory>();
        }
    }
}
