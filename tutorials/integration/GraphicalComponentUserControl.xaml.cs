using System;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

using GTTG.Core.Component;
using GTTG.Integration.MouseInput;

namespace GTTG.Integration {

    /// <summary>
    /// Interaction logic for GraphicalComponentUserControl.xaml
    /// </summary>
    public partial class GraphicalComponentUserControl : SKElement {

        /*
         * Graphical engine managing view state in application.
         */
        private readonly GraphicalComponent _graphicalComponent;
        
        /*
         *  Manages mouse state and cursor deltas in drag implementation.
         */
        private readonly DragProcessor _dragProcessor;
        
        /*
         *  Paint with information how to draw text. Used to draw horizontal axis with time values.
         */
        private readonly SKPaint _timePaint = new SKPaint { Color = SKColors.Black, Style = SKPaintStyle.Fill, IsAntialias = true, TextSize = 14 };

        public GraphicalComponentUserControl() {

            _graphicalComponent = new GraphicalComponent();
            _dragProcessor = new DragProcessor();

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
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e) {

            e.Surface.Canvas.Clear();

            /* Set matrix equal from application state do display correct subset of drawn content */
            e.Surface.Canvas.SetMatrix(_graphicalComponent.ContentMatrix);
            
            /* Draw time values in the middle of view */
            var contentStart = _graphicalComponent.ContentDateTimeInterval.Start; 
            foreach (var dateTime in _graphicalComponent.ContentDateTimeInterval.GetDateTimesByPeriod(contentStart, TimeSpan.FromMinutes(10))) {

                var x = _graphicalComponent.GetContentHorizontalPosition(dateTime);
                e.Surface.Canvas.DrawText(dateTime.ToString("HH:mm"), new SKPoint(x, 30), _timePaint);
            }
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
