using System;
using System.ComponentModel;
using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Component;
using SZDC.Editor.Interfaces;
using SZDC.Model.Infrastructure;
using SZDC.Model.Views;

namespace SZDC.Editor.Components {

    /// <summary>
    /// Column with railway distance values of stations. The values are mapped vertically to the same horizontal line position of stations.
    /// </summary>
    public class RailwayDistanceSidebarComponent : ObservableObject {

        private const float KilometersTextSize = 12;
        private const string ExpectedDisplayedTextFormatReference = "123.4=123.0"; // default reference string width
        private const float KilometersTextCanvasWidthOffset = 4; // pixels count between left border and kilometers string              

        private float _railwayDistanceColumnRequiredWidth;
        private readonly IViewProvider _viewProvider;
        private readonly IViewModel _viewModel;
        private readonly SKPaint _stationPaint = new SKPaint {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Black, IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Times New Roman")
        };

        /// <summary>
        /// Requests <see cref="Draw"/> call.
        /// </summary>
        public event ViewModifiedHandler ViewModified;

        /// <summary>
        /// Width of this component as column in pixels units provided to the graphical component.
        /// UI element binds width to this value.
        /// </summary>
        public float RailwayDistanceColumnRequiredWidth {
            get => _railwayDistanceColumnRequiredWidth;
            private set => Update(ref _railwayDistanceColumnRequiredWidth, value);
        }
        
        public RailwayDistanceSidebarComponent(IViewProvider viewProvider,
                                               IViewModifiedNotifier viewModifiedNotifier,
                                               IViewModel viewModel) {

            _viewProvider = viewProvider;
            _viewModel = viewModel;
            _viewModel.PropertyChanged += OnRailwayViewChanged;

            viewModifiedNotifier.ViewModified += () => ViewModified?.Invoke();

            if (_viewModel.RailwayView != null) {
                MeasureRailwayDistanceMaxWidth();
            }
            else {
                RailwayDistanceColumnRequiredWidth =
                    KilometersTextCanvasWidthOffset + _stationPaint.MeasureText(ExpectedDisplayedTextFormatReference);
            }
        }

        private void OnRailwayViewChanged(object sender, PropertyChangedEventArgs e) {

            if (e.PropertyName == nameof(_viewModel.RailwayView)) {
                MeasureRailwayDistanceMaxWidth();
            }
        }

        private void MeasureRailwayDistanceMaxWidth() {

            var maxLengthRailwayDistanceString = ExpectedDisplayedTextFormatReference;
            var railway = (SzdcRailway) _viewModel.RailwayView.Railway;

            foreach (var stationView in _viewModel.RailwayView.StationViews) {


                var stationKilometersString = railway.StationInfo[stationView.Station].RailwayDistanceStringValue;
                if (maxLengthRailwayDistanceString.Length < stationKilometersString.Length) {
                    maxLengthRailwayDistanceString = stationKilometersString;
                }
            }

            RailwayDistanceColumnRequiredWidth =
                KilometersTextCanvasWidthOffset + _stationPaint.MeasureText(maxLengthRailwayDistanceString);
        }

        public void Draw(SKSurface skSurface) {

            // use content matrix to show correct subarea, do not account horizontal translation
            var viewMatrix = _viewProvider.ContentMatrix;
            viewMatrix.TransX = 0;
            skSurface.Canvas.SetMatrix(viewMatrix);

            // values to make text size same with different DPI or scale values
            var dpiScale = _viewProvider.DpiScale;
            var scale = _viewProvider.Scale;

            if (_viewModel.RailwayView == null) return;

            _stationPaint.TextSize = KilometersTextSize * dpiScale / scale;
            var textHeight = Math.Abs(_stationPaint.FontMetrics.CapHeight);
            var railway = (SzdcRailway) _viewModel.RailwayView.Railway;

            foreach (var stationView in _viewModel.RailwayView.StationViews) {

                // place kilometers distance in the middle of it's station view height
                var stationCanvasY = stationView.ContentLeftTop.Y + stationView.ContentHeight / 2 + textHeight / 2;
                skSurface.Canvas.DrawText(
                    railway.StationInfo[stationView.Station].RailwayDistanceStringValue,
                    new SKPoint(KilometersTextCanvasWidthOffset * dpiScale / scale, stationCanvasY), _stationPaint);
            }
        }
    }
}
