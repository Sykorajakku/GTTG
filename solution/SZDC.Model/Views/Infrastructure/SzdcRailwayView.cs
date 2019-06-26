using System;
using System.Collections.Generic;
using System.Linq;
using SkiaSharp;

using GTTG.Core.Component;
using GTTG.Core.Strategies.Implementations;
using GTTG.Core.Strategies.Interfaces;
using GTTG.Model.Model.Infrastructure;
using GTTG.Model.Strategies.Types;
using GTTG.Model.ViewModel.Infrastructure.Railways;
using GTTG.Model.ViewModel.Infrastructure.Stations;
using SZDC.Model.Infrastructure;

namespace SZDC.Model.Views.Infrastructure {

    public class SzdcRailwayView : StrategyRailwayView<SzdcStationView, SzdcTrackView> {

        private readonly Dictionary<(SzdcStationView, SzdcStationView), double> _stationsAvailableHeightMultiples;
        private readonly IViewProvider _viewProvider;

        private float SegmentsAvailableSpace => 
            ViewConstants.AdjacentStationsVerticalSpace * _viewProvider.DpiScale * (StationViews.Length - 1);

        private float FirstOrLastSegmentAvailableSpace =>
            ViewConstants.StartOrEndStationSegmentSpace * _viewProvider.DpiScale;

        public SzdcRailwayView(Railway railway,
                               IViewProvider viewProvider,
                               IStationViewFactory<SzdcStationView, SzdcTrackView> stationViewFactory,
                               ISegmentRegistry<SegmentType<Station>, MeasureableSegment> stationSegments) 

            : base(railway, stationViewFactory, stationSegments) {

            _stationsAvailableHeightMultiples = new Dictionary<(SzdcStationView, SzdcStationView), double>();
            _viewProvider = viewProvider;
            ComputeHeightMultiples();
        }

        // creates distribution in percentages from distances between their position on railway in kilometers
        private void ComputeHeightMultiples() {

            var szdcRailway = (SzdcRailway) Railway;

            var adjacentStationViews = StationViews
                .Skip(1)
                .Zip(StationViews, (s1, s2) => (s1, s2))
                .ToList();

            foreach (var (s2, s1) in adjacentStationViews) {

                _stationsAvailableHeightMultiples[(s1, s2)] = 
                    szdcRailway.StationInfo[s2.Station].RailwayDistance
                    - szdcRailway.StationInfo[s1.Station].RailwayDistance;
            }

            var distanceSum = Math.Abs(szdcRailway.StationInfo[StationViews[StationViews.Length - 1].Station].RailwayDistance
                                     - szdcRailway.StationInfo[StationViews[0].Station].RailwayDistance);
            foreach (var (s2, s1) in adjacentStationViews) {
                _stationsAvailableHeightMultiples[(s1, s2)] /= distanceSum;
            }
        }

        protected override SKSize MeasureOverride(SKSize availableSize) {

            var desiredHeight = 0f;

            foreach (var stationView in StationViews) {

                stationView.Measure(new SKSize(float.MaxValue, float.MaxValue));
                desiredHeight += stationView.DesiredSize.Height;
            }

            desiredHeight += SegmentsAvailableSpace;
            desiredHeight += FirstOrLastSegmentAvailableSpace * 2;

            return new SKSize(float.MaxValue, desiredHeight);
        }
       
        protected override SKSize ArrangeOverride(SKSize finalSize) {

            var totalSegmentsAvailableSpace = SegmentsAvailableSpace;
            const float x = 0f;
            var y = 0f;

            if (finalSize.Height > DesiredSize.Height) {
                totalSegmentsAvailableSpace += finalSize.Height - DesiredSize.Height;
            }

            var firstSegment = StationSegments.Resolve(new SegmentType<Station>(StationViews[0].Station, SegmentPlacement.Upper));
            firstSegment.SetBounds(this, y, y + FirstOrLastSegmentAvailableSpace);
            y += firstSegment.SegmentLocalHeight;

            var stationViewsCount = StationViews.Length;

            for (var i = 0; i < stationViewsCount - 1; ++i) {

                var firstStationView = StationViews[i];
                var secondStationView = StationViews[i + 1];

                firstStationView.Arrange(new SKPoint(x, y), firstStationView.DesiredSize, this);
                y += firstStationView.ArrangedHeight;

                var lowerFirstSegment = StationSegments.Resolve(new SegmentType<Station>(firstStationView.Station, SegmentPlacement.Lower));
                var upperSecondSegment = StationSegments.Resolve(new SegmentType<Station>(secondStationView.Station, SegmentPlacement.Upper));

                var segmentAvailableSpace = totalSegmentsAvailableSpace * _stationsAvailableHeightMultiples[(firstStationView, secondStationView)];
                lowerFirstSegment.SetBounds(this, y, y + (float) segmentAvailableSpace);
                upperSecondSegment.SetBounds(this, y, y + (float) segmentAvailableSpace);

                y += (float) segmentAvailableSpace;
            }

            var lastStationView = StationViews[stationViewsCount - 1];
            lastStationView.Arrange(new SKPoint(x, y), lastStationView.DesiredSize, this);
            y += lastStationView.ArrangedHeight;

            var lastStationSegment = StationSegments.Resolve(new SegmentType<Station>(lastStationView.Station, SegmentPlacement.Lower));
            lastStationSegment.SetBounds(this, y, y + FirstOrLastSegmentAvailableSpace);
            y += lastStationSegment.SegmentLocalHeight;

            return new SKSize(finalSize.Width, y);
        }
    }
}
