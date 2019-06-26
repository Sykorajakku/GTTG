using GTTG.Infrastructure.ViewModel;
using GTTG.Model.Model.Infrastructure;
using GTTG.Model.ViewModel.Infrastructure.Stations;
using GTTG.Model.ViewModel.Infrastructure.Tracks;

namespace GTTG.Infrastructure.ViewModelFactories {

    public class TutorialStationViewFactory : IStationViewFactory<TutorialStationView, TutorialTrackView> {

        private readonly ITrackViewFactory<TutorialTrackView> _trackViewFactory;

        public TutorialStationViewFactory(ITrackViewFactory<TutorialTrackView> trackViewFactory) {
            _trackViewFactory = trackViewFactory;
        }

        public TutorialStationView CreateStationView(Station station) {
            return new TutorialStationView(station, _trackViewFactory);
        }
    }
}
