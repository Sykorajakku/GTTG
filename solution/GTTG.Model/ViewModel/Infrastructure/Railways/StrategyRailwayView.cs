// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using SkiaSharp;

using GTTG.Core.Strategies.Implementations;
using GTTG.Core.Strategies.Interfaces;
using GTTG.Model.Model.Infrastructure;
using GTTG.Model.Strategies.Types;
using GTTG.Model.ViewModel.Infrastructure.Stations;
using GTTG.Model.ViewModel.Infrastructure.Tracks;

namespace GTTG.Model.ViewModel.Infrastructure.Railways {

    /// <summary>
    /// Represents visualization of <see cref="Railway"/> with segments to which elements in strategies can be placed.
    /// </summary>
    /// <typeparam name="TStationView">Concrete implementation of <see cref="StationView{TTrackView}"/> used by this instance.</typeparam>
    /// <typeparam name="TTrackView">Concrete implementation of <see cref="TrackView"/> used by this instance.</typeparam>
    public class StrategyRailwayView<TStationView, TTrackView> : RailwayView<TStationView, TTrackView>
        where TTrackView : TrackView
        where TStationView : StationView<TTrackView> {

        /// <summary>
        /// Segment registry where created segments above and below station were registered. 
        /// </summary>
        protected ISegmentRegistry<SegmentType<Station>, MeasureableSegment> StationSegments;

        /// <summary>
        /// Creates visualization of <see cref="Railway"/> and segments placed to <paramref name="stationSegments"/>.
        /// </summary>
        /// <param name="railway">Instance of <see cref="Railway"/> to be visualized.</param>
        /// <param name="stationViewFactory">Interface with factory method to convert list of <see cref="Station"/> instances in <see cref="Railway"/> to <see cref="RailwayView{TStationView,TTrackView}.StationViews"/>.</param>
        /// <param name="stationSegments">Registry where segments are registered.</param>
        public StrategyRailwayView(Railway railway,
                                   IStationViewFactory<TStationView, TTrackView> stationViewFactory,
                                   ISegmentRegistry<SegmentType<Station>, MeasureableSegment> stationSegments)

            : base(railway, stationViewFactory) {

            StationSegments = stationSegments;
            RegisterSegments(stationSegments);
        }

        private void RegisterSegments(ISegmentRegistry<SegmentType<Station>, MeasureableSegment> stationSegments) {
            OnSegmentRegistration(stationSegments);
        }

        /// <summary>
        /// Registers segments to <see cref="StationSegments"/>.
        /// </summary>
        protected virtual void OnSegmentRegistration(ISegmentRegistry<SegmentType<Station>, MeasureableSegment> segmentRegistry) {

            foreach (var stationView in StationViews) {

                var station = stationView.Station;

                var upperKey = new SegmentType<Station>(station, SegmentPlacement.Upper);
                var upperSegment = new MeasureableSegment();

                var lowerKey = new SegmentType<Station>(station, SegmentPlacement.Lower);
                var lowerSegment = new MeasureableSegment();

                segmentRegistry.Register(upperSegment).As(upperKey);
                segmentRegistry.Register(lowerSegment).As(lowerKey);
            }
        }

        /// <inheritdoc/>
        protected override SKSize MeasureOverride(SKSize availableSize) {

            float desiredHeight = 0;

            int index = 0;
            for (int count = StationViews.Length; index < count; ++index) {

                var stationView = StationViews[index];
                stationView.Measure(availableSize);
                desiredHeight += stationView.DesiredSize.Height;

                var upperStationSegment =
                    StationSegments.Resolve(new SegmentType<Station>(stationView.Station, SegmentPlacement.Upper));
                upperStationSegment.MeasureHeight();
                desiredHeight += upperStationSegment.DesiredHeight;

                var lowerStationSegment =
                    StationSegments.Resolve(new SegmentType<Station>(stationView.Station, SegmentPlacement.Lower));
                lowerStationSegment.MeasureHeight();
                desiredHeight += lowerStationSegment.DesiredHeight;
            }
            return new SKSize(float.MaxValue, desiredHeight);
        }

        /// <summary>
        /// Arranges <see cref="RailwayView{TStationView,TTrackView}.StationViews"/> and segments proportionally in <paramref name="finalSize"/> height.
        /// If height returned from <see cref="MeasureOverride"/> is higher than <paramref name="finalSize"/>,
        /// stations and segments receives in <see cref="ArrangeOverride"/> scaled desired height.
        /// Otherwise remaining space is split equally between stations.
        /// </summary>
        protected override SKSize ArrangeOverride(SKSize finalSize) {

            float scale = Math.Min(finalSize.Height / DesiredSize.Height, 1);
            var remainingSpace = scale >= 1 ? (finalSize.Height - DesiredSize.Height) / (StationViews.Length - 1) : 0;

            float x = 0;
            float y = 0;
            
            int index = 0;
            for (int count = StationViews.Length; index < count; ++index) {

                var stationView = StationViews[index];
                var upperSegment = StationSegments.Resolve(new SegmentType<Station>(stationView.Station, SegmentPlacement.Upper));
                var lowerSegment = StationSegments.Resolve(new SegmentType<Station>(stationView.Station, SegmentPlacement.Lower));

                var scaledUpperSegmentHeight = upperSegment.DesiredHeight * scale;
                upperSegment.SetBounds(this, y, y + scaledUpperSegmentHeight);
                y += scaledUpperSegmentHeight;

                stationView.Arrange(new SKPoint(x, y), new SKSize(float.MaxValue, stationView.DesiredSize.Height * scale), this);
                y += stationView.UnscaledHeight;

                var scaledLowerSegmentHeight = lowerSegment.DesiredHeight * scale;
                lowerSegment.SetBounds(this, y, y + scaledLowerSegmentHeight);
                y += scaledLowerSegmentHeight;

                y += count - 1 == index ? 0 : remainingSpace; // omit last station
            }
            return new SKSize(float.MaxValue, y);
        }
    }
}
