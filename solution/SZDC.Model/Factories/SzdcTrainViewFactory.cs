using GTTG.Core.Base;
using GTTG.Core.Component;
using GTTG.Core.Strategies.Implementations;
using GTTG.Core.Strategies.Interfaces;
using GTTG.Model.Lines;
using GTTG.Model.Model.Infrastructure;
using GTTG.Model.Strategies;
using GTTG.Model.Strategies.Converters;
using GTTG.Model.Strategies.Dockers;
using GTTG.Model.Strategies.Types;
using GTTG.Model.ViewModel.Traffic;
using SZDC.Model.Factories.Exceptions;
using SZDC.Model.Infrastructure.Trains;
using SZDC.Model.Views.Traffic;
using SZDC.Model.Views.TrainPaths;

namespace SZDC.Model.Factories {

    public class SzdcStrategy : Strategy {

        public TrainNumberPlacementSelector TrainNumberPlacementSelector { get; }

        public SzdcStrategy(TrainNumberPlacementSelector trainNumberPlacementSelector, 
                            IStrategyDocker trackStrategyDocker,
                            IStrategyDocker stationStrategyDocker,
                            StrategyManager<TrainEventPlacement, ViewElement, SegmentType<Track>, MeasureableSegment> trackContainers,
                            StrategyManager<TrainEventPlacement, ViewElement, SegmentType<Station>, MeasureableSegment> stationContainers)
            
            : base(trackStrategyDocker, stationStrategyDocker, trackContainers, stationContainers) {

            TrainNumberPlacementSelector = trainNumberPlacementSelector;
        }
    }

    public class SzdcTrainViewFactory : ITrainViewFactory<SzdcTrainView, SzdcTrain> {

        private readonly IViewProvider _viewProvider;
        private readonly ISegmentRegistry<LineType, MeasureableSegment> _trackLineSegmentsRegistry;
        private readonly ISegmentRegistry<SegmentType<Track>, MeasureableSegment> _trackSegmentsRegistry;
        private readonly ISegmentRegistry<SegmentType<Station>, MeasureableSegment> _stationSegmentsRegistry;

        public SzdcTrainViewFactory(IViewProvider viewProvider, 
                                    ISegmentRegistry<LineType, MeasureableSegment> trackLineSegmentsRegistry,
                                    ISegmentRegistry<SegmentType<Track>, MeasureableSegment> trackSegmentsRegistry,
                                    ISegmentRegistry<SegmentType<Station>, MeasureableSegment> stationSegmentsRegistry) {

            _viewProvider = viewProvider;
            _trackLineSegmentsRegistry = trackLineSegmentsRegistry;
            _trackSegmentsRegistry = trackSegmentsRegistry;
            _stationSegmentsRegistry = stationSegmentsRegistry;
        }

        public SzdcTrainView CreateTrainView(SzdcTrain train) {

            var linePaint = TrainLinePaintFactory.CreateLinePaint(train);

            ITrainPath trainPath = new TrainPath(_viewProvider, _trackLineSegmentsRegistry, linePaint);
            var converter = new TrainEventPlacementConverter(trainPath);
            trainPath = ArrangeTrainPath(train, trainPath, converter);
            var component = CreateTrainComponentsManager(trainPath, converter);

            return new SzdcTrainView(train, trainPath, component);
        }

        private SzdcStrategy CreateTrainComponentsManager(ITrainPath trainPath, TrainEventPlacementConverter converter) {

            var tracksManager = new StrategyManager<TrainEventPlacement, ViewElement, SegmentType<Track>, MeasureableSegment>(_trackSegmentsRegistry, converter);
            var tracksDocker = new TracksStrategyDocker<ViewElement>(trainPath, converter, tracksManager);

            var stationsManager = new StrategyManager<TrainEventPlacement, ViewElement, SegmentType<Station>, MeasureableSegment>(_stationSegmentsRegistry, converter);
            var stationsDocker = new StationStrategyDocker<ViewElement>(trainPath, converter, stationsManager);

            var trainNumberSelector = new TrainNumberPlacementSelector(converter);

            return new SzdcStrategy(trainNumberSelector, tracksDocker, stationsDocker, tracksManager, stationsManager);
        }

        // chain multiple train path implementations in decorator pattern
        private ITrainPath ArrangeTrainPath(SzdcTrain train, ITrainPath trainPath, TrainEventPlacementConverter converter) {

            trainPath = ArrangeTrainPathWithTrainDirectionSymbols(trainPath, train);

            if (train.IsDepartingToOtherRailway) {
                trainPath = new DepartureArrowTrainPath(trainPath, converter, _trackSegmentsRegistry, _trackLineSegmentsRegistry, converter);
            }

            if (train.IsArrivingFromOtherRailway) {
                trainPath = new ArrivalArrowTrainPath(trainPath, converter, _trackSegmentsRegistry, _trackLineSegmentsRegistry, converter);
            }

            if (train.TrainType.IsCargoType()) {
                trainPath = new DottedEndPointsTrainPath(trainPath, train);
            }

            return trainPath;
        }

        private static ITrainPath ArrangeTrainPathWithTrainDirectionSymbols(ITrainPath trainPath, SzdcTrain train) {

            switch (train.TrainTypeDecoration) {
                case TrainDecorationType.AgainstValidDirection:
                    return new DottedTrainPath(trainPath);
                case TrainDecorationType.FollowsValidDirection:
                    return trainPath;
                case TrainDecorationType.IrregularTrain:
                    return new PerpendicularIntersectionsTrainPath(trainPath);
                default:
                    throw new ViewFactoryException($"Train type direction {train.TrainTypeDecoration} not recognized.");
            }
        }
    }
}
