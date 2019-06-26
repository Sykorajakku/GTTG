// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Drawing.Canvases;
using GTTG.Model.Model.Infrastructure;
using GTTG.Model.ViewModel.Infrastructure.Tracks;

namespace GTTG.Model.ViewModel.Infrastructure.Stations {

    /// <summary>
    /// Represents visualization of <see cref="StationView{TTrackView}"/>.
    /// </summary>
    /// <typeparam name="TTrackView">Concrete implementation of <see cref="TrackView"/> used by this instance.</typeparam>
    public class StationView<TTrackView> : InfrastructureViewElement
        where TTrackView : TrackView {

        /// <summary>
        /// Instance of <see cref="Station"/> being visualized.
        /// </summary>
        public Station Station { get; }

        /// <summary>
        /// Visualization of tracks in <see cref="Station"/>.
        /// </summary>
        public ImmutableArray<TTrackView> TrackViews { get; }

        /// <summary>
        /// Creates visualization of <see cref="Station"/>.
        /// </summary>
        /// <param name="station">Instance of <see cref="Station"/> to be visualized.</param>
        /// <param name="trackViewFactory">Interface with factory method to convert list of <see cref="Track"/> instances in <see cref="Station"/> to <see cref="TrackViews"/>.</param>
        public StationView(Station station,
                           ITrackViewFactory<TTrackView> trackViewFactory) {

            Station = station;
            TrackViews = ImmutableArray.CreateRange(station.Tracks.Select(trackViewFactory.CreateTrackView));
        }

        /// <inheritdoc/>
        protected override void OnDraw(DrawingCanvas drawingCanvas) {

            foreach (var trackView in TrackViews) {
                drawingCanvas.Draw(trackView);
            }
        }

        /// <inheritdoc/>
        protected override SKSize MeasureOverride(SKSize availableSize) {

            float desiredHeight = 0;
            int index = 0;
            for (int count = TrackViews.Length;  index < count; ++index) {

                var trackView = TrackViews[index];
                trackView.Measure(availableSize);
                desiredHeight += trackView.DesiredSize.Height;
            }

            return new SKSize(float.MaxValue, desiredHeight);
        }

        /// <summary>
        /// Arranges <see cref="TrackViews"/> proportionally in <paramref name="finalSize"/> height.
        /// If height returned from <see cref="MeasureOverride"/> is higher than <paramref name="finalSize"/>,
        /// tracks receives in <see cref="ArrangeOverride"/> scaled desired height.
        /// Otherwise remaining space is split equally between tracks.
        /// </summary>
        protected override SKSize ArrangeOverride(SKSize finalSize) {

            float y = 0;
            float scale = finalSize.Height / DesiredSize.Height;

            int index = 0;
            for (int count = TrackViews.Length; index < count; ++index) {

                var trackView = TrackViews[index];

                var x = 0;
                trackView.Arrange(new SKPoint(x, y), new SKSize(float.MaxValue, trackView.DesiredSize.Height * scale), this);
                y += trackView.ArrangedHeight;
            }

            return new SKSize(float.MaxValue, y);
        }

        /// <inheritdoc/>
        public override IEnumerable<IVisual> ProvideVisuals() {
            return TrackViews;
        }
    }
}
