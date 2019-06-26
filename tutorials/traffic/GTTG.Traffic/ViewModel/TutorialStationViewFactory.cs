using GTTG.Core.Strategies.Implementations;
using GTTG.Core.Strategies.Interfaces;
using GTTG.Model.Model.Infrastructure;
using GTTG.Model.Strategies.Types;
using GTTG.Model.ViewModel.Infrastructure.Stations;
using GTTG.Model.ViewModel.Infrastructure.Tracks;

namespace GTTG.Traffic.ViewModel {

    public class TutorialStationViewFactory : IStationViewFactory<TutorialStationView, TutorialTrackView> {

        private readonly ITrackViewFactory<TutorialTrackView> _trackViewFactory;
        private readonly ISegmentRegistry<SegmentType<Track>, MeasureableSegment> _segmentRegistry;

        public TutorialStationViewFactory(ITrackViewFactory<TutorialTrackView> trackViewFactory,
            ISegmentRegistry<SegmentType<Track>, MeasureableSegment> segmentRegistry) {

            _trackViewFactory = trackViewFactory;
            _segmentRegistry = segmentRegistry;
        }

        public TutorialStationView CreateStationView(Station station) {
            return new TutorialStationView(station, _trackViewFactory, _segmentRegistry);
        }
    }
}
