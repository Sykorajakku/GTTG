// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Drawing.Canvases;
using GTTG.Core.Strategies.Implementations;
using GTTG.Model.Lines;
using GTTG.Model.Model.Infrastructure;

namespace GTTG.Model.ViewModel.Infrastructure.Tracks {

    /// <summary>
    /// Represents visualization of <see cref="Track"/>.
    /// </summary>
    public class TrackView : InfrastructureViewElement {

        /// <summary>
        /// Instance of track being visualized.
        /// </summary>
        public Track Track { get; }

        /// <summary>
        /// Paint for <see cref="TrackPath"/>.
        /// </summary>
        public LinePaint LinePaint { get; }

        /// <summary>
        /// Horizontal line representing track.
        /// </summary>
        protected SKPath TrackPath { get; }
        
        /// <summary>
        /// Segment to determine height (<see cref="SKPaint.StrokeWidth"/> of <see cref="TrackPath"/>.
        /// </summary>
        protected MeasureableSegment TrackLineSegment { get; }

        /// <summary>
        /// Creates visualization of <paramref name="track"/>.
        /// </summary>
        /// <param name="track">Instance of track being visualized.</param>
        /// <param name="linePaint">Paint to use when drawing <see cref="SKPath"/> representing horizontal line of track.</param>
        /// <param name="trackLineSegment">Segment used to determine and position of horizontal line.</param>
        public TrackView(Track track, LinePaint linePaint, MeasureableSegment trackLineSegment) {

            Track = track;
            TrackPath = new SKPath();
            LinePaint = linePaint;
            TrackLineSegment = trackLineSegment;
            RegisterTrackLines();
        }

        private void RegisterTrackLines() {
            OnTrackLineRegistration();
        }

        private void OnTrackLineRegistration() {
            TrackLineSegment.HeightMeasureHelpers += () => LinePaint.Measure();
        }

        /// <inheritdoc/>
        protected override SKSize MeasureOverride(SKSize availableSize) {
            TrackLineSegment.MeasureHeight();
            return new SKSize(float.MaxValue, TrackLineSegment.DesiredHeight);
        }
        
        /// <summary>
        /// Set height of horizontal line.
        /// </summary>
        protected override SKSize ArrangeOverride(SKSize finalSize) {

            TrackLineSegment.SetBounds(this, 0, finalSize.Height);

            if (LinePaint.DesiredStrokeWidth > finalSize.Height) {
                LinePaint.Arrange(finalSize.Height);
            }

            return new SKSize(float.MaxValue, finalSize.Height);
        }

        /// <inheritdoc/>
        protected override void OnDraw(DrawingCanvas drawingCanvas) {

            TrackPath.Reset();

            var trackLineY = TrackLineSegment.SegmentLocalMiddle;
            TrackPath.MoveTo(0, trackLineY);
            TrackPath.LineTo(drawingCanvas.Width, trackLineY);
            drawingCanvas.Canvas.DrawPath(TrackPath, LinePaint.Paint);
        }

        /// <inheritdoc/>
        public override IEnumerable<IVisual> ProvideVisuals() {
            yield break;
        }
    }   
}
