// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Drawing.Canvases;
using GTTG.Model.Model.Traffic;

namespace GTTG.Model.ViewModel.Traffic {

    /// <summary>
    /// Represents visualization of <see cref="Traffic{TTrain}"/>.
    /// </summary>
    public class TrafficView<TTrainView, TTrain> : Visual
        where TTrainView : TrainView<TTrain>
        where TTrain : Train {

        private ImmutableList<TTrainView> _trainViews;

        /// <summary>
        /// Instance of traffic being visualized.
        /// </summary>
        public Traffic<TTrain> Traffic { get; }

        /// <summary>
        /// Visualization of <typerefparam name="TTrain"/> trains in <see cref="Traffic"/>.
        /// </summary>
        public ImmutableList<TTrainView> TrainViews {
            get => _trainViews;
            set => Update(ref _trainViews, value);
        }

        /// <summary>
        /// Creates visualization of <paramref name="traffic"/>.
        /// </summary>
        /// <param name="traffic">Instance of traffic to be visualized.</param>
        /// <param name="trainViewFactory">Interface with factory method to convert list of <typerefparam name="TTrain"/> instances in <paramref name="traffic"/> to <see cref="TrainViews"/>.</param>
        public TrafficView(Traffic<TTrain> traffic, ITrainViewFactory<TTrainView, TTrain> trainViewFactory) {
            TrainViews = ImmutableList.CreateRange(traffic.Trains.Select(trainViewFactory.CreateTrainView));
            Traffic = traffic;
        }

        /// <inheritdoc/>
        protected override void OnDraw(DrawingCanvas drawingCanvas) {

            foreach (var trainView in _trainViews) {
                drawingCanvas.Draw(trainView);
            }
        }

        /// <inheritdoc/>
        public override bool HasHit(SKPoint contentPoint) {
            return true;
        }

        /// <summary>
        /// Calls <see cref="TrainView{TTrain}.UpdateTrainViewContent"/> on all trains <see cref="TrainViews"/>.
        /// </summary>
        public void Update() => Parallel.ForEach(_trainViews, t => t.UpdateTrainViewContent());

        /// <summary>
        /// Calls <see cref="TrainView{TTrain}.Arrange"/> on all trains <see cref="TrainViews"/>.
        /// </summary>
        public void Arrange() => Parallel.ForEach(_trainViews, t => t.Arrange());

        /// <inheritdoc/>
        public override IEnumerable<IVisual> ProvideVisuals() {
            return TrainViews;
        }

        /// <summary>
        /// Selects nearest train in <see cref="TrainViews"/> with closest distance to provided point.
        /// </summary>
        /// <param name="canvasLocation">Position on <see cref="ContentDrawingCanvas"/> to which find the nearest train.</param>
        /// <returns>Instance of nearest <typerefparam name="TTrainView"/> in <see cref="TrainViews"/>.</returns>
        public TTrainView SelectNearestTrainView(SKPoint canvasLocation) {

            TTrainView nearestTrain = null;
            var minDistance = double.MaxValue;

            foreach (var trainView in TrainViews) {

                var distance = trainView.DistanceFromPoint(canvasLocation);
                if (distance < minDistance) {
                    nearestTrain = trainView;
                    minDistance = distance;
                }
            }

            return nearestTrain;
        }
    }
}
