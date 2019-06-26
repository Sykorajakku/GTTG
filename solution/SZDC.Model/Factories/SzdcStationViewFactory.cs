using GTTG.Core.Component;
using GTTG.Core.Strategies.Implementations;
using GTTG.Core.Strategies.Interfaces;
using GTTG.Model.Model.Infrastructure;
using GTTG.Model.Strategies.Types;
using GTTG.Model.ViewModel.Infrastructure.Stations;
using SZDC.Model.Views.Infrastructure;

namespace SZDC.Model.Factories {

    /// <inheritdoc />
    public class SzdcStationViewFactory : IStationViewFactory<SzdcStationView, SzdcTrackView> {

        private readonly SzdcTrackViewFactory _trackViewFactory;
        private readonly ISegmentRegistry<SegmentType<Track>, MeasureableSegment> _segmentRegistry;
        private readonly IViewProvider _viewProvider;

        public SzdcStationViewFactory(IViewProvider viewProvider,
                                      SzdcTrackViewFactory trackViewFactory,
                                      ISegmentRegistry<SegmentType<Track>, MeasureableSegment> segmentRegistry) {

            _viewProvider = viewProvider;
            _trackViewFactory = trackViewFactory;
            _segmentRegistry = segmentRegistry;
        }

        /// <inheritdoc />
        public SzdcStationView CreateStationView(Station station) {
            return new SzdcStationView(station, _viewProvider, _segmentRegistry, _trackViewFactory);
        }
    }
}
