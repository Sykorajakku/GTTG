using SkiaSharp;

using GTTG.Core.Component;
using GTTG.Core.Strategies.Implementations;
using GTTG.Core.Strategies.Interfaces;
using GTTG.Model.Lines;
using GTTG.Model.Model.Infrastructure;
using GTTG.Model.Model.Traffic;
using GTTG.Model.Strategies;
using GTTG.Model.Strategies.Types;
using GTTG.Model.ViewModel.Traffic;

namespace GTTG.Traffic.ViewModel.Traffic {

    public class TutorialTrainViewFactory : ITrainViewFactory<TutorialTrainView, Train> {

        private readonly ISegmentRegistry<LineType, MeasureableSegment> _linesSegmentRegistry;
        private readonly ISegmentRegistry<SegmentType<Track>, MeasureableSegment> _trackSegmentRegistry;
        private readonly ISegmentRegistry<SegmentType<Station>, MeasureableSegment> _stationSegmentRegistry;

        private readonly IViewProvider _viewProvider;

        public TutorialTrainViewFactory(IViewProvider viewProvider,
                                        ISegmentRegistry<LineType, MeasureableSegment> linesSegmentRegistry,
                                        ISegmentRegistry<SegmentType<Track>, MeasureableSegment> trackSegmentRegistry,
                                        ISegmentRegistry<SegmentType<Station>, MeasureableSegment> stationSegmentRegistry) {

            _viewProvider = viewProvider;
            _linesSegmentRegistry = linesSegmentRegistry;
            _trackSegmentRegistry = trackSegmentRegistry;
            _stationSegmentRegistry = stationSegmentRegistry;
        }

        public TutorialTrainView CreateTrainView(Train train) {

            var trackLine = new LinePaint(4, SKColors.Black);
            var trainPath = new TrainPath(_viewProvider, _linesSegmentRegistry, trackLine);
            var strategy = new Strategy(trainPath, _trackSegmentRegistry, _stationSegmentRegistry);
            return new TutorialTrainView(train, trainPath, strategy);
        }
    }
}
