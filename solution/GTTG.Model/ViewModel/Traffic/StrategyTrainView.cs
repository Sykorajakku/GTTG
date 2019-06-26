// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Drawing.Canvases;
using GTTG.Model.Lines;
using GTTG.Model.Model.Traffic;
using GTTG.Model.Strategies;

namespace GTTG.Model.ViewModel.Traffic {


    /// <summary>
    /// Represents visualization of <see cref="Train"/> with strategies which positions added elements nearby train path. 
    /// </summary>
    /// <typeparam name="TStrategy">Concrete implementation of <see cref="IStrategy"/> used by this instance.</typeparam>
    /// <typeparam name="TTrain">Concrete implementation of <see cref="Train"/> used by this instance.</typeparam>
    public class StrategyTrainView<TStrategy, TTrain> : TrainView<TTrain>
        where TTrain : Train
        where TStrategy : IStrategy {

        /// <summary>
        /// Instance of <typerefparam name="TStrategy"/> being used to add elements nearby train path.
        /// </summary>
        public TStrategy Strategy { get; }

        /// <summary>
        /// Creates visualization of <see cref="Train"/> with provided <paramref name="strategy"/>.
        /// </summary>
        /// <param name="train">Instance of <see cref="Train"/> to be visualized.</param>
        /// <param name="trainPath">Train path representing schedule of train.</param>
        /// <param name="strategy">Implementation of <see cref="Strategy"/>.</param>
        public StrategyTrainView(TTrain train, ITrainPath trainPath, TStrategy strategy)
            : base(train, trainPath) {

            Strategy = strategy;
            Strategy.Clear();
        }

        /// <inheritdoc/>
        public override void Arrange() {
            base.Arrange();
            Strategy.Dock();
        }

        /// <inheritdoc/>
        public override IEnumerable<IVisual> ProvideVisuals() {
            yield return Strategy;
        }

        /// <summary>
        /// Draws <see cref="Strategy"/>.
        /// </summary>
        public virtual void DrawContainers(DrawingCanvas drawingCanvas) {
            drawingCanvas.Draw(Strategy);
        }

        /// <inheritdoc/>
        protected override void OnDraw(DrawingCanvas drawingCanvas) {
            DrawContainers(drawingCanvas);
            base.OnDraw(drawingCanvas);
        }

        /// <inheritdoc/>
        public override void UpdateTrainViewContent() {
            base.UpdateTrainViewContent();
            Strategy.Clear();
        }

        /// <inheritdoc/>
        public override bool HasHit(SKPoint contentPoint) {
            return true; // true as it passes testing to strategy managers
        }
    }
}
