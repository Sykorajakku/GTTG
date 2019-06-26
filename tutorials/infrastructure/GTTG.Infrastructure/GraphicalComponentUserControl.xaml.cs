using System;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

using GTTG.Core.Component;
using GTTG.Core.Drawing.Canvases;
using GTTG.Core.Drawing.Layers;
using GTTG.Core.Utils;
using GTTG.Infrastructure.Layers;
using GTTG.Infrastructure.MouseInput;
using GTTG.Infrastructure.ViewModel;
using GTTG.Infrastructure.ViewModelFactories;
using GTTG.Model.ViewModel.Infrastructure.Railways;

namespace GTTG.Infrastructure {

    /// <summary>
    /// Interaction logic for GraphicalComponentUserControl.xaml
    /// </summary>
    public partial class GraphicalComponentUserControl : SKElement {

        /*
         * Graphical engine managing view state in application.
         */
        private readonly GraphicalComponent _graphicalComponent;

        /*
         * Draws added layers of content in defined order.
         */
        private readonly DrawingManager _drawingManager;

        /*
         *  Manages mouse state and cursor deltas in drag implementation.
         */
        private readonly DragProcessor _dragProcessor;

        /*
         * We need to keep reference on axis layer is it's height is measured
         */
        private TimeAxisLayer _timeAxisLayer;
        
        /*
         * View model of railway
         */
        private RailwayView<TutorialStationView, TutorialTrackView> _railwayView;

        public GraphicalComponentUserControl() {

            _graphicalComponent = new GraphicalComponent();
            _dragProcessor = new DragProcessor();
            _drawingManager = new DrawingManager(new CanvasFactory(_graphicalComponent), new DrawingLayersOrder());

            InitializeComponent();
            /* Handlers for application state modifications from UI changes */
            Loaded += (_,__) => OnLoaded();
            SizeChanged += (_,__) => OnSizeChanged();
        }

        /*
         *  UI sizes already determined. We can initialize view.
         */
        private void OnLoaded() {

            _graphicalComponent.TryChangeDateTimeContext(TrainTimetableData.DateTimeContext);

            /* Initialize view model */
            var trackViewFactory = new TutorialTrackViewFactory();
            var stationViewFactory = new TutorialStationViewFactory(trackViewFactory);
            _railwayView = new RailwayView<TutorialStationView,TutorialTrackView>(TrainTimetableData.Railway, stationViewFactory);

            /* Order of calls to register has no effect on layer order as it is defined by DrawingLayerOrder in constructor */
            _timeAxisLayer = new TimeAxisLayer(_graphicalComponent);
            _drawingManager.ReplaceRegisteredDrawingLayer(new InfrastructureLayer(_railwayView));
            _drawingManager.ReplaceRegisteredDrawingLayer(new TimeLinesLayer(_graphicalComponent));
            _drawingManager.ReplaceRegisteredDrawingLayer(_timeAxisLayer);

            /*
             * Add handlers to mouse input. Input source is created to transform UI device independent pixels
             * to device pixels used by library (also supports device independent - see IgnorePixelsScaling property,
             * but we use device pixels here).
             */
            var source = new WpfMouseInputSource(this);
            source.LeftUp.Subscribe(LeftUp);
            source.LeftDown.Subscribe(LeftDown);
            source.Move.Subscribe(Move);
            source.Scroll.Subscribe(Scroll);
            source.Leave.Subscribe(Leave);

            /* Arrange the application content with current UI size */
            OnSizeChanged();
            /* Trigger redraw, OnPaintSurface() called */
            InvalidateVisual();
        }

        private void OnSizeChanged() {

            /* CanvasSize in device pixels */
            _graphicalComponent?.TryResizeView(CanvasSize.Width, CanvasSize.Height);
            _railwayView?.Measure(LayoutConstants.InfiniteSize);
            /* Move railway view to avoid content being hidden by _timeAxisLayer drawn atop */
            _railwayView?.Arrange(new SKPoint(0, _timeAxisLayer.Height), new SKSize(float.MaxValue, CanvasSize.Height - _timeAxisLayer.Height));

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
               which would results in unmodified state (change set Y to 0), same for X and width in second if statement
            */
            if (viewProvider.ContentMatrix.ScaleY.Equals(1.0f) && viewProvider.ContentHeight.Equals(viewProvider.ViewHeight)) {
                translation.Y = 0;
            }
            if (viewProvider.ContentMatrix.ScaleX.Equals(1.0f) && viewProvider.ContentWidth.Equals(viewProvider.ViewWidth)) {
                translation.X = 0;
            }

            var result = _graphicalComponent.TryTranslate(translation);

            if (result == TranslationTransformationResult.ViewModified) {

                /* Trigger redraw as different content subset is visible by modification */
                InvalidateVisual();
            }
        }

        public void Scroll(MouseZoomArgs args) {

            var result = _graphicalComponent.TryScale(new SKPoint(args.X, args.Y), args.Delta);
            if (result != ScaleTransformationResult.ViewUnmodified) {

                /* Trigger redraw as different content subset is visible by modification */
                InvalidateVisual();
            }
        }

        public void Leave(MouseInputArgs args) {
            _dragProcessor.TryExitDrag(args);
        }
    }
}
