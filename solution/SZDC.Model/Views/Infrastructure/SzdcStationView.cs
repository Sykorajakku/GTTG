using System;
using System.Linq;
using SkiaSharp;

using GTTG.Core.Component;
using GTTG.Core.Drawing.Canvases;
using GTTG.Core.Strategies.Implementations;
using GTTG.Core.Strategies.Interfaces;
using GTTG.Model.Model.Infrastructure;
using GTTG.Model.Strategies.Types;
using GTTG.Model.ViewModel.Infrastructure.Stations;
using GTTG.Model.ViewModel.Infrastructure.Tracks;

namespace SZDC.Model.Views.Infrastructure {

    public class SzdcStationView : StrategyStationView<SzdcTrackView> {

        public bool IsToggled { get; protected set; }

        protected float CollapsedLinePaintPosition { get; set; }

        private readonly IViewProvider _viewProvider;
        
        public SzdcStationView(Station station,
                               IViewProvider viewProvider,
                               ISegmentRegistry<SegmentType<Track>, MeasureableSegment> trackSegments,
                               ITrackViewFactory<SzdcTrackView> trackViewFactory)
           
            : base(station, trackSegments, trackViewFactory) {

            _viewProvider = viewProvider;

            IsToggled = false;
        }
        
        protected override void OnDraw(DrawingCanvas drawingCanvas) {

            if (!IsToggled) {

                var path = new SKPath();
                path.MoveTo(0, CollapsedLinePaintPosition);
                path.LineTo(drawingCanvas.Size.Width, CollapsedLinePaintPosition);

                var originalLineThickness = TrackViews[0].LinePaint.Paint.StrokeWidth;
                TrackViews[0].LinePaint.Paint.StrokeWidth /= _viewProvider.Scale;
                drawingCanvas.Canvas.DrawPath(path, TrackViews[0].LinePaint.Paint);
                TrackViews[0].LinePaint.Paint.StrokeWidth = originalLineThickness;
            }
            else {
                base.OnDraw(drawingCanvas);
            }
        }

        public void ChangeState() {
            if (TrackViews.Length == 1) return; // do not toggle only one track
            IsToggled = !IsToggled;
        }
        
        protected override SKSize MeasureOverride(SKSize availableSize) {
            return IsToggled ? MeasureToggled(availableSize) : MeasureCollapsed(availableSize);
        }

        protected override SKSize ArrangeOverride(SKSize finalSize) {
            return IsToggled ? ArrangeToggled(finalSize) : ArrangeCollapsed(finalSize);
        }

        /// <summary>Overlaps nearby upper and lower contents.</summary>
        private SKSize MeasureToggled(SKSize availableSize) {

            float desiredHeight = 0;
            foreach (var trackView in TrackViews) {

                trackView.Measure(availableSize);
                desiredHeight += trackView.DesiredSize.Height;
            }
            desiredHeight += ViewConstants.TrackSegmentVerticalSpace * (TrackViews.Length + 1);

            return new SKSize(float.MaxValue, desiredHeight);
        }

        /// <summary>Compute DesiredHeight of collapsed tracks into one. DesiredHeight is maximum from track required space.</summary>
        private SKSize MeasureCollapsed(SKSize availableSize) {

            float maxTrackHeight = 0;

            var dpiScaledSegmentSpace = ViewConstants.TrackSegmentVerticalSpace * _viewProvider.DpiScale;

            foreach (var trackView in TrackViews) {
                trackView.Measure(availableSize);
                maxTrackHeight = Math.Max(maxTrackHeight, trackView.DesiredSize.Height);
            }

            return new SKSize(float.MaxValue, dpiScaledSegmentSpace * 2 + maxTrackHeight);
        }

        private SKSize ArrangeToggled(SKSize finalSize) {

            Segment previousLowerSegment = null;
            float y = 0;

            foreach (var trackView in TrackViews) {
                
                var upperSegment = TrackSegments.Resolve(new SegmentType<Track>(trackView.Track, SegmentPlacement.Upper));
                upperSegment.SetBounds(this, y, y + ViewConstants.TrackSegmentVerticalSpace);
                y += ViewConstants.TrackSegmentVerticalSpace;

                trackView.Arrange(new SKPoint(0, y), trackView.DesiredSize, this);
                y += trackView.ArrangedHeight;

                var lowerSegment = previousLowerSegment = TrackSegments.Resolve(new SegmentType<Track>(trackView.Track, SegmentPlacement.Lower));
                lowerSegment.SetBounds(this, y, y + ViewConstants.TrackSegmentVerticalSpace);
            }

            if (previousLowerSegment != null) y += ViewConstants.TrackSegmentVerticalSpace;
            return new SKSize(float.MaxValue, y);
        }

        private SKSize ArrangeCollapsed(SKSize finalSize) {

            var segmentHeight = ViewConstants.TrackSegmentVerticalSpace * _viewProvider.DpiScale;
            var maxTrackHeight = TrackViews.Max(t => t.DesiredSize.Height);
            var trackY = segmentHeight;
            var lowerSegmentY = maxTrackHeight + trackY;

            foreach (var trackView in TrackViews) {

                var upperSegment = TrackSegments.Resolve(new SegmentType<Track>(trackView.Track, SegmentPlacement.Upper));
                upperSegment.SetBounds(this, 0, segmentHeight);
                
                trackView.Arrange(new SKPoint(0, trackY), trackView.DesiredSize, this);
                CollapsedLinePaintPosition = trackY + trackView.DesiredSize.Height / 2;
                if (trackView.ArrangedHeight < maxTrackHeight) {
                    var heightOffset = (maxTrackHeight - trackView.ArrangedHeight) / 2; 
                    trackView.Reposition(new SKPoint(0, trackY + heightOffset), this);
                }

                var lowerSegment = TrackSegments.Resolve(new SegmentType<Track>(trackView.Track, SegmentPlacement.Lower));
                lowerSegment.SetBounds(this, lowerSegmentY, lowerSegmentY + segmentHeight);
            }
            
            return new SKSize(float.PositiveInfinity, maxTrackHeight + segmentHeight * 2);
        }
    }
}
