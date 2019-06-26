// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Drawing.Canvases;
using GTTG.Model.Lines;
using GTTG.Model.Model.Traffic;

namespace GTTG.Model.ViewModel.Traffic {

    /// <summary>
    /// Represents visualization of train.
    /// </summary>
    public abstract class TrainView<TTrain> : Visual where TTrain : Train {

        /// <summary>
        /// Instance of train being visualized.
        /// </summary>
        public TTrain Train { get; }

        /// <summary>
        /// Line representing schedule of train. 
        /// </summary>
        protected ITrainPath TrainPath { get; }

        /// <summary>
        /// Creates visualization of train.
        /// </summary>
        /// <param name="train">Instance of train to visualize.</param>
        /// <param name="trainPath">Train path representing schedule of train.</param>
        protected TrainView(TTrain train, ITrainPath trainPath) {

            Train = train;
            TrainPath = trainPath;
        }

        /// <summary>
        /// Arranges content of train on canvas to reflect changes in arrangement of other view models.
        /// Re-arranges train path as line of points in railway.
        /// </summary>
        public virtual void Arrange() {
            TrainPath.Arrange();
        }

        /// <summary>
        /// Determines closest distance from point to train path.
        /// </summary>
        /// <param name="point">Point to which find the distance.</param>
        /// <returns>Closest distance to <paramref name="point"/>.</returns>
        public virtual float DistanceFromPoint(SKPoint point) {
            return TrainPath.DistanceFromPoint(point);
        }

        /// <summary>
        /// Updates content of train visualization to match new schedule of train.
        /// Re-validates point in train path as schedule creating train path can be changed.
        /// </summary>
        public virtual void UpdateTrainViewContent() {
            TrainPath.Update(Train.Schedule);
        }
        
        /// <inheritdoc/>
        protected override void OnDraw(DrawingCanvas drawingCanvas) {
            TrainPath.Draw(drawingCanvas);
        }
    }
}
