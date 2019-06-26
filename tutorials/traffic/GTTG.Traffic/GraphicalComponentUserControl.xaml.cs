using System;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

using GTTG.Core.Component;
using GTTG.Core.Drawing.Canvases;
using GTTG.Core.Drawing.Layers;
using GTTG.Core.Strategies.Implementations;
using GTTG.Core.Utils;
using GTTG.Model.Model.Infrastructure;
using GTTG.Model.Model.Traffic;
using GTTG.Model.Strategies.Types;
using GTTG.Model.ViewModel.Infrastructure.Railways;
using GTTG.Model.ViewModel.Traffic;
using GTTG.Traffic.Layers;
using GTTG.Traffic.MouseInput;
using GTTG.Traffic.ViewModel;
using GTTG.Traffic.ViewModel.Traffic;

namespace GTTG.Traffic {

    /// <summary>
    /// Interaction logic for GraphicalComponentUserControl.xaml
    /// </summary>
    public partial class GraphicalComponentUserControl : SKElement {

        private StrategyRailwayView<TutorialStationView, TutorialTrackView> _railwayView;
        private TrafficView<TutorialTrainView, Train> _trafficView;
        private GraphicalComponent _graphicalComponent;
        private DrawingManager _drawingManager;
        private DragProcessor _dragProcessor;
        private TimeAxisLayer _timeAxisLayer;

        public GraphicalComponentUserControl() {
            InitializeComponent();
            Loaded += (_,__) => OnLoaded();
            SizeChanged += (_,__) => OnSizeChanged();
        }

        private void OnLoaded() {

            _graphicalComponent = new GraphicalComponent();
            _graphicalComponent.TryChangeDateTimeContext(TrainTimetableData.DateTimeContext);
            _drawingManager = new DrawingManager(new CanvasFactory(_graphicalComponent), new DrawingLayersOrder());
            _dragProcessor = new DragProcessor();

            var lineSegments = new SegmentRegistry<LineType, MeasureableSegment>();
            var tracksSegments = new SegmentRegistry<SegmentType<Track>, MeasureableSegment>();
            var stationSegments = new SegmentRegistry<SegmentType<Station>, MeasureableSegment>();

            var trackViewFactory = new TutorialTrackViewFactory(lineSegments);
            var stationViewFactory = new TutorialStationViewFactory(trackViewFactory, tracksSegments);
            var trainViewFactory = new TutorialTrainViewFactory(_graphicalComponent, lineSegments, tracksSegments, stationSegments);

            _railwayView = new StrategyRailwayView<TutorialStationView, TutorialTrackView>(TrainTimetableData.Railway, stationViewFactory, stationSegments);
            _trafficView =  new TrafficView<TutorialTrainView, Train>(TrainTimetableData.Traffic, trainViewFactory);

            _timeAxisLayer = new TimeAxisLayer(_graphicalComponent);
            _drawingManager.ReplaceRegisteredDrawingLayer(_timeAxisLayer);
            _drawingManager.ReplaceRegisteredDrawingLayer(new InfrastructureLayer(_railwayView));
            _drawingManager.ReplaceRegisteredDrawingLayer(new TimeLinesLayer(_graphicalComponent));
            _drawingManager.ReplaceRegisteredDrawingLayer(new TrafficLayer(_trafficView));

            var source = new WpfMouseInputSource(this);
            source.LeftUp.Subscribe(LeftUp);
            source.LeftDown.Subscribe(LeftDown);
            source.Move.Subscribe(Move);
            source.Scroll.Subscribe(Scroll);
            source.Leave.Subscribe(Leave);

            OnSizeChanged();
            _trafficView.Update();
            OnSizeChanged();
            InvalidateVisual();
        }

        private void OnSizeChanged() {

            _graphicalComponent?.TryResizeView(CanvasSize.Width, CanvasSize.Height);
            _railwayView?.Measure(LayoutConstants.InfiniteSize);
            _railwayView?.Arrange(new SKPoint(0, _timeAxisLayer.Height), new SKSize(float.MaxValue, CanvasSize.Height - _timeAxisLayer.Height));
            _trafficView?.Arrange();
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e) {
            _drawingManager?.Draw(e.Surface);
        }

        public void LeftUp(MouseInputArgs args) {
            _dragProcessor.TryExitDrag(args);
        }

        public void LeftDown(MouseInputArgs args) {
            _dragProcessor.TryInitializeDrag(args);
        }

        public void Move(MouseInputArgs args) {

            if (!_dragProcessor.IsEnabled) {
                return;
            }

            var translation = _dragProcessor.GetTranslation(args);
            var viewProvider = _graphicalComponent;

            /*
               prevents tearing while content height is equal to view height and translation vector is not 0 in Y,
               which would results in unmodified state (change set Y to 0), same for X and width in following condition
            */
            
            if (viewProvider.ContentMatrix.ScaleY.Equals(1.0f) && viewProvider.ContentWidth.Equals(viewProvider.ViewHeight)) {
                translation.Y = 0;
            }
            if (viewProvider.ContentMatrix.ScaleX.Equals(1.0f) && viewProvider.ContentHeight.Equals(viewProvider.ViewWidth)) {
                translation.X = 0;
            }

            var result = _graphicalComponent.TryTranslate(translation);

            if (result == TranslationTransformationResult.ViewModified) {
                InvalidateVisual();
            }
        }

        public void Scroll(MouseZoomArgs args) {

            var result = _graphicalComponent.TryScale(new SKPoint(args.X, args.Y), args.Delta);
            if (result != ScaleTransformationResult.ViewUnmodified) {
                InvalidateVisual();
            }
        }

        public void Leave(MouseInputArgs args) {
            _dragProcessor.TryExitDrag(args);
        }
    }
}
