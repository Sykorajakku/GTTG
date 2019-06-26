// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using SkiaSharp;

using GTTG.Core.Strategies.Implementations;
using GTTG.Core.Strategies.Interfaces;
using GTTG.Model.Model.Infrastructure;
using GTTG.Model.Strategies.Types;
using GTTG.Model.ViewModel.Infrastructure.Tracks;

namespace GTTG.Model.ViewModel.Infrastructure.Stations {

    /// <summary>
    /// Represents visualization of <see cref="Station"/> with segments to which elements in strategies can be placed.
    /// </summary>
    /// <typeparam name="TTrackView">Concrete implementation of <see cref="TrackView"/> used by this instance.</typeparam>
    public class StrategyStationView<TTrackView> : StationView<TTrackView>
        where TTrackView : TrackView {

        /// <summary>
        /// Segment registry where created segments above and below tracks were registered.
        /// </summary>
        protected ISegmentRegistry<SegmentType<Track>, MeasureableSegment> TrackSegments { get; }

        /// <summary>
        /// Creates visualization of <see cref="Station"/> and segments placed to <paramref name="trackSegments"/>.
        /// </summary>
        /// <param name="station">Instance of <see cref="Railway"/> to be visualized.</param>
        /// <param name="trackViewFactory">Interface with factory method to convert list of <see cref="Track"/> instances in <see cref="Station"/> to <see cref="StationView{TTrackView}"/>.</param>
        /// <param name="trackSegments">Registry where segments are registered.</param>
        public StrategyStationView(Station station,
                                   ISegmentRegistry<SegmentType<Track>, MeasureableSegment> trackSegments,
                                   ITrackViewFactory<TTrackView> trackViewFactory)

            : base (station, trackViewFactory) {

            TrackSegments = trackSegments;
            RegisterStationSegments(trackSegments);
        }

        /// <summary>
        /// Registers segments to <see cref="TrackSegments"/>.
        /// </summary>
        protected void RegisterStationSegments(ISegmentRegistry<SegmentType<Track>, MeasureableSegment> segmentRegistry) {

            foreach (var trackView in TrackViews) {

                var upperSegmentKey = new SegmentType<Track>(trackView.Track, SegmentPlacement.Upper);
                var lowerSegmentKey = new SegmentType<Track>(trackView.Track, SegmentPlacement.Lower);

                var upperSegment = new MeasureableSegment();
                var lowerSegment = new MeasureableSegment();

                segmentRegistry.Register(upperSegment).As(upperSegmentKey);
                segmentRegistry.Register(lowerSegment).As(lowerSegmentKey);
            }
        }

        /// <inheritdoc/>
        protected override SKSize MeasureOverride(SKSize availableSize) {

            float desiredHeight = 0;
            var size = base.MeasureOverride(availableSize);
            desiredHeight += size.Height;

            foreach (var trackView in TrackViews) {

                var segment = TrackSegments.Resolve(new SegmentType<Track>(trackView.Track, SegmentPlacement.Upper));
                segment.MeasureHeight();
                desiredHeight += segment.DesiredHeight;

                segment = TrackSegments.Resolve(new SegmentType<Track>(trackView.Track, SegmentPlacement.Lower));
                segment.MeasureHeight();
                desiredHeight += segment.DesiredHeight;
            }

            return new SKSize(float.MaxValue, desiredHeight);
        }

        /// <summary>
        /// Arranges <see cref="StationView{TTrackView}"/> and segments proportionally in <paramref name="finalSize"/> height.
        /// If height returned from <see cref="MeasureOverride"/> is higher than <paramref name="finalSize"/>,
        /// tracks and segments receives in <see cref="ArrangeOverride"/> scaled desired height.
        /// Otherwise remaining space is split equally between tracks.
        /// </summary>
        protected override SKSize ArrangeOverride(SKSize finalSize) {

            float y = 0;
            float scale = finalSize.Height / DesiredSize.Height;

            int index = 0;
            for (int count = TrackViews.Length; index < count; ++index) {

                var trackView = TrackViews[index];

                var upperSegment = TrackSegments.Resolve(new SegmentType<Track>(trackView.Track, SegmentPlacement.Upper));
                var lowerSegment = TrackSegments.Resolve(new SegmentType<Track>(trackView.Track, SegmentPlacement.Lower));

                var x = 0;
                var segmentHeight = upperSegment.DesiredHeight * scale;
                upperSegment.SetBounds(this, y, y + segmentHeight);
                y += segmentHeight;

                trackView.Arrange(new SKPoint(x, y), new SKSize(float.MaxValue, trackView.DesiredSize.Height * scale), this);
                y += trackView.ArrangedHeight;

                segmentHeight = lowerSegment.DesiredHeight * scale;
                lowerSegment.SetBounds(this, y, y + segmentHeight);
                y += segmentHeight;
            }

            return new SKSize(float.MaxValue, y);
        }
    }
}
