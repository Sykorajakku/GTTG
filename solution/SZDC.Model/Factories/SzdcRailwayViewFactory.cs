using GTTG.Core.Component;
using GTTG.Core.Strategies.Implementations;
using GTTG.Core.Strategies.Interfaces;
using GTTG.Model.Model.Infrastructure;
using GTTG.Model.Strategies.Types;
using GTTG.Model.ViewModel.Infrastructure.Railways;
using SZDC.Model.Views.Infrastructure;

namespace SZDC.Model.Factories {

    /// <inheritdoc />
    public class SzdcRailwayViewFactory : IRailwayViewFactory<SzdcRailwayView, SzdcStationView, SzdcTrackView> {

        private readonly SzdcStationViewFactory _stationViewProvider;
        private readonly IViewProvider _viewProvider;
        private readonly ISegmentRegistry<SegmentType<Station>, MeasureableSegment> _segmentRegistry;

        public SzdcRailwayViewFactory(SzdcStationViewFactory stationViewFactory,
                                      IViewProvider viewProvider,
                                      ISegmentRegistry<SegmentType<Station>, MeasureableSegment> segmentRegistry) {

            _stationViewProvider = stationViewFactory;
            _viewProvider = viewProvider;
            _segmentRegistry = segmentRegistry;
        }

        /// <inheritdoc />
        public SzdcRailwayView CreateRailwayView(Railway railway) {
            return new SzdcRailwayView(railway, _viewProvider, _stationViewProvider, _segmentRegistry);
        }
    }
}
