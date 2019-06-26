using System;
using System.ComponentModel;
using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Component;
using SZDC.Editor.Interfaces;
using SZDC.Editor.MouseInput;
using SZDC.Model.Infrastructure;
using SZDC.Model.Views;
using SZDC.Model.Views.Infrastructure;

namespace SZDC.Editor.Components {

    /// <summary>
    /// Column with names of stations. The names are mapped vertically to the same horizontal line position of stations.
    /// </summary>
    public class StationsSidebarComponent : ObservableObject, IMouseInputTarget {

        /// <summary>
        /// Requests <see cref="Draw"/> call.
        /// </summary>
        public event ViewModifiedHandler ViewModified;

        private float _requiredComponentWidth;
        private SzdcStationView _selectedStation;
        private readonly IViewModel _viewModel;
        private readonly IViewProvider _viewProvider;
        private readonly SKPaint _stationPaint = new SKPaint { TextSize = TextSize, Style = SKPaintStyle.Fill, Color = SKColors.Black, IsAntialias = true, Typeface = SKTypeface.FromFamilyName("Times New Roman") };
        private readonly SKPaint _stationBackgroundPaint = new SKPaint { Style = SKPaintStyle.Fill, Color = SKColors.AliceBlue };

        private const float TextSize = 14;
        private const string ReferenceStationTextWidth = "Pardubice hl.n.";
        private const float StationTextColumnPrefixOffset = 4;
        private const float StationTextColumnPostfixOffset = 4;

        /// <summary>
        /// Currently toggled or collapsed station in the component.
        /// </summary>
        public SzdcStationView ToggledStation {
            get => _selectedStation;
            set => Update(ref _selectedStation, value, true);
        }

        /// <summary>
        /// Width of this component as column in pixels units provided to the graphical component.
        /// UI element binds width to this value.
        /// </summary>
        public float RequiredComponentWidth {
            get => _requiredComponentWidth;
            set => Update(ref _requiredComponentWidth, value);
        }
        
        public StationsSidebarComponent(IViewProvider viewProvider,
                                        IViewModifiedNotifier viewModifiedNotifier,
                                        IViewModel viewModel) {

            _viewModel = viewModel;
            _viewProvider = viewProvider;
            viewModifiedNotifier.ViewModified += () => ViewModified?.Invoke();

            _viewModel.PropertyChanged += OnRailwayViewChanged;
        }

        private void OnRailwayViewChanged(object sender, PropertyChangedEventArgs e) {

            if (e.PropertyName == nameof(_viewModel.RailwayView)) {
                MeasureStationNameTextFromViewModelData();
            }
        }

        private void MeasureStationNameTextFromViewModelData() {

            var maxLengthStationNameString = ReferenceStationTextWidth;
            foreach (var stationView in _viewModel.RailwayView.StationViews) {

                var stationName = ((SzdcStation) stationView.Station).StationName;

                if (maxLengthStationNameString.Length < stationName.Length) {
                    maxLengthStationNameString = stationName;
                }
            }
            RequiredComponentWidth = GetColumnWidth(maxLengthStationNameString);
        }

        private float GetColumnWidth(string stationName) {

            var bounds = SKRect.Empty;
            var textWidth = _stationPaint.MeasureText(stationName, ref bounds);

            return StationTextColumnPrefixOffset
                + textWidth
                + StationTextColumnPostfixOffset;
        }

        public void Draw(SKSurface skSurface) {

            var viewMatrix = _viewProvider.ContentMatrix;
            viewMatrix.TransX = 0;
            skSurface.Canvas.SetMatrix(viewMatrix);

            var dpiScale = _viewProvider.DpiScale;
            var scale = _viewProvider.Scale;

            if (_viewModel.RailwayView == null) return;

            _stationPaint.TextSize = TextSize * dpiScale / scale;

            foreach (var stationView in _viewModel.RailwayView.StationViews) {

                if (stationView.TrackViews.Length != 1) {
                    DrawStationBackground(stationView, skSurface);
                }

                var textCanvasY = stationView.ContentLeftTop.Y + stationView.ContentHeight / 2 + Math.Abs(_stationPaint.FontMetrics.CapHeight / 2);
                var textCanvasX = StationTextColumnPrefixOffset * dpiScale / scale;
                var stationName = ((SzdcStation) stationView.Station).StationName;

                skSurface.Canvas.DrawText(stationName, new SKPoint(textCanvasX, textCanvasY), _stationPaint);
            }
        }

        public void DrawStationBackground(SzdcStationView stationView, SKSurface surface) {

            if (stationView.IsToggled) {

                var trackViewsCount = stationView.TrackViews.Length;
                var firstTrackPosition = stationView.TrackViews[0].ContentLeftTop;
                var lastTrackPosition = stationView.TrackViews[trackViewsCount - 1].ContentLeftBottom;
                var tracksHeight = lastTrackPosition.Y - firstTrackPosition.Y;

                var backgroundRect = SKRect.Create(0, firstTrackPosition.Y, _requiredComponentWidth, tracksHeight);
                surface.Canvas.DrawRect(backgroundRect, _stationBackgroundPaint);
            }
            else {

                var capHeight = _stationPaint.FontMetrics.CapHeight;
                var horizontalPosition = stationView.ContentLeftTop.Y + stationView.ContentHeight / 2 - capHeight / 2;
                var backgroundRect = SKRect.Create(0, horizontalPosition, _requiredComponentWidth, _stationPaint.FontMetrics.CapHeight);
                surface.Canvas.DrawRect(backgroundRect, _stationBackgroundPaint);
            }
        }

        public void LeftDown(MouseInputArgs args) {

            if (_viewModel.RailwayView == null) return;

            var clickCanvasY = _viewProvider.ConvertViewToContentLocation(args.X, args.Y).Y;

            foreach (var stationView in _viewModel.RailwayView.StationViews) {

                if (clickCanvasY >= stationView.ContentLeftTop.Y && clickCanvasY <= stationView.ContentLeftBottom.Y) {
                    ToggledStation = stationView;
                    break;
                }
            }
        }

        public void RightUp(MouseInputArgs args) {
            // Do nothing.
        }

        public void RightDown(MouseInputArgs args) {
            // Do nothing.
        }

        public void LeftUp(MouseInputArgs args) {
            // Do nothing.
        }

        public void Move(MouseInputArgs args) {
            // Do nothing.
        }

        public void ScrollUp(MouseZoomArgs args) {
            // Do nothing.
        }

        public void ScrollDown(MouseZoomArgs args) {
            // Do nothing.
        }

        public void Enter(MouseInputArgs args) {
            // Do nothing.
        }

        public void Leave(MouseInputArgs args) {
            // Do nothing.
        }
    }
}
