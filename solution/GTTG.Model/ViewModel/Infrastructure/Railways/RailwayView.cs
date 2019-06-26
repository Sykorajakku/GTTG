// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Drawing.Canvases;
using GTTG.Model.Model.Infrastructure;
using GTTG.Model.ViewModel.Infrastructure.Stations;
using GTTG.Model.ViewModel.Infrastructure.Tracks;

namespace GTTG.Model.ViewModel.Infrastructure.Railways {

    /// <summary>
    /// Represents visualization of <see cref="Railway"/>.
    /// </summary>
    /// <typeparam name="TStationView">Concrete implementation of <see cref="StationView{TTrackView}"/> used by this instance.</typeparam>
    /// <typeparam name="TTrackView">Concrete implementation of <see cref="TrackView"/> used by this instance.</typeparam>
    public class RailwayView<TStationView, TTrackView> : InfrastructureViewElement
        where TStationView : StationView<TTrackView>
        where TTrackView : TrackView {

        /// <summary>
        /// Instance of <see cref="Railway"/> being visualized.
        /// </summary>
        public Railway Railway { get; }

        private ImmutableArray<TStationView> _stationViews;

        /// <summary>
        /// Visualization of stations in <see cref="Railway"/>.
        /// </summary>
        public ImmutableArray<TStationView> StationViews {
            get => _stationViews;
            set => Update(ref _stationViews, value);
        }

        /// <summary>
        /// Creates visualization of <see cref="Railway"/>.
        /// </summary>
        /// <param name="railway">Instance of <see cref="Railway"/> to be visualized.</param>
        /// <param name="stationViewFactory">Interface with factory method to convert list of <see cref="Station"/> instances in <see cref="Railway"/> to <see cref="StationViews"/>.</param>
        public RailwayView(Railway railway,
                           IStationViewFactory<TStationView, TTrackView> stationViewFactory) {

            Railway = railway;
            StationViews = ImmutableArray.CreateRange(railway.Stations.Select(stationViewFactory.CreateStationView));
        }

        /// <inheritdoc/>
        protected override void OnDraw(DrawingCanvas drawingCanvas) {

            foreach (var stationView in StationViews) {
                drawingCanvas.Draw(stationView);
            }
        }

        /// <inheritdoc/>
        public override IEnumerable<IVisual> ProvideVisuals() {
            return StationViews;
        }

        /// <inheritdoc/>
        protected override SKSize MeasureOverride(SKSize availableSize) {

            float desiredHeight = 0;

            int index = 0;
            for (int count = StationViews.Length; index < count; ++index) {

                var stationView = StationViews[index];
                stationView.Measure(availableSize);
                desiredHeight += stationView.DesiredSize.Height;
            }
            return new SKSize(float.MaxValue, desiredHeight);
        }

        /// <summary>
        /// Arranges <see cref="StationViews"/> proportionally in <paramref name="finalSize"/> height.
        /// If height returned from <see cref="MeasureOverride"/> is higher than <paramref name="finalSize"/>,
        /// stations receives in <see cref="ArrangeOverride"/> scaled desired height.
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
                stationView.Arrange(new SKPoint(x, y), new SKSize(float.MaxValue, stationView.DesiredSize.Height * scale), this);

                var space = count - 1 == index ? 0 : remainingSpace;
                y += stationView.UnscaledHeight + space;
            }
            return new SKSize(float.MaxValue, y);
        }
    }
}
