// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Drawing.Canvases;
using GTTG.Core.Strategies.Implementations;
using GTTG.Core.Strategies.Interfaces;
using GTTG.Model.Lines;
using GTTG.Model.Model.Infrastructure;
using GTTG.Model.Strategies.Converters;
using GTTG.Model.Strategies.Dockers;
using GTTG.Model.Strategies.Types;
using GTTG.Model.ViewModel.Traffic;

namespace GTTG.Model.Strategies  {

    /// <summary>
    /// Represents strategies applicable to <see cref="TrainView{TTrain}"/>.
    /// </summary>
    public class Strategy : Visual, IStrategy  {

        /// <summary>
        /// Docker for strategy to place elements in segments above or below horizontal line of tracks.
        /// </summary>
        protected IStrategyDocker TrackStrategyDocker { get; }

        /// <summary>
        /// Docker for strategy to place elements in segments between stations.
        /// </summary>
        protected IStrategyDocker StationStrategyDocker { get; }

        /// <summary>
        /// Manager to which elements are added to be placed below or above horizontal line of tracks in angles intersecting train's path. 
        /// </summary>
        public StrategyManager<TrainEventPlacement, ViewElement, SegmentType<Track>, MeasureableSegment> TrackStrategyManager { get; }

        /// <summary>
        /// Manager to which elements are added to be placed in segments between stations on train's path. 
        /// </summary>
        public StrategyManager<TrainEventPlacement, ViewElement, SegmentType<Station>, MeasureableSegment> StationStrategyManager { get; }

        /// <summary>
        /// Creates instance with strategies applicable to particular train by using it's <see cref="ITrainPath"/>.
        /// </summary>
        /// <param name="trainPath">Path of train to which strategies are applied.</param>
        /// <param name="trackSegmentRegistry">TrackSegments above or below horizontal line of tracks.</param>
        /// <param name="stationSegmentRegistry">TrackSegments between stations.</param>
        public Strategy(ITrainPath trainPath,
                        ISegmentRegistry<SegmentType<Track>, MeasureableSegment> trackSegmentRegistry,
                        ISegmentRegistry<SegmentType<Station>, MeasureableSegment> stationSegmentRegistry) {

            var typeConverter = new TrainEventPlacementConverter(trainPath);
            TrackStrategyManager
                = new StrategyManager<TrainEventPlacement, ViewElement, SegmentType<Track>, MeasureableSegment>(trackSegmentRegistry, typeConverter);
            StationStrategyManager
                = new StrategyManager<TrainEventPlacement, ViewElement, SegmentType<Station>, MeasureableSegment>(stationSegmentRegistry, typeConverter);

            StationStrategyDocker = new StationStrategyDocker<ViewElement>(trainPath, typeConverter, StationStrategyManager);
            TrackStrategyDocker = new TracksStrategyDocker<ViewElement>(trainPath, typeConverter, TrackStrategyManager);
        }

        /// <summary>
        /// Creates instance with strategies applicable to particular train by using it's <see cref="ITrainPath"/>.
        /// </summary>
        /// <param name="trackStrategyDocker">Docker for strategy to place elements in segments above or below horizontal line of tracks.</param>
        /// <param name="stationStrategyDocker">Docker for strategy to place elements in segments between stations.</param>
        /// <param name="trackStrategyManager">Manager to which elements are added to be placed below or above horizontal line of tracks in angles intersecting train's path.</param>
        /// <param name="stationStrategyManager">Manager to which elements are added to be placed in segments between stations on train's path.</param>
        public Strategy(IStrategyDocker trackStrategyDocker,
                        IStrategyDocker stationStrategyDocker,
                        StrategyManager<TrainEventPlacement, ViewElement, SegmentType<Track>, MeasureableSegment> trackStrategyManager,
                        StrategyManager<TrainEventPlacement, ViewElement, SegmentType<Station>, MeasureableSegment> stationStrategyManager) {

            TrackStrategyDocker = trackStrategyDocker;
            StationStrategyDocker = stationStrategyDocker;
            TrackStrategyManager = trackStrategyManager;
            StationStrategyManager = stationStrategyManager;
        }

        /// <inheritdoc/>
        public override bool HasHit(SKPoint contentPoint) {
            return true;
        }

        /// <inheritdoc/>
        public override IEnumerable<IVisual> ProvideVisuals() {
            yield return TrackStrategyManager;
            yield return StationStrategyManager;
        }

        /// <inheritdoc/>
        public virtual void Dock() {
            TrackStrategyDocker.Dock();
            StationStrategyDocker.Dock();
        }

        /// <inheritdoc/>
        public virtual void Clear() {
            TrackStrategyManager.Clear();
            StationStrategyManager.Clear();
        }

        /// <inheritdoc/>
        protected override void OnDraw(DrawingCanvas drawingCanvas) {
            TrackStrategyManager.Draw(drawingCanvas);
            StationStrategyManager.Draw(drawingCanvas);
        }
    }
}
