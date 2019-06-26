using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

using GTTG.Core.Base;
using SZDC.Editor.Tools;
using SZDC.Model.Infrastructure.Trains;
using SZDC.Model.Views;

namespace SZDC.Wpf.Converters {

    public class SelectionModel : ObservableObject {

        private readonly TrainViewSelectionTool _trainViewSelectionTool;
        private readonly IViewModel _viewModel;

        private ImmutableArray<SzdcTrain> _trains;
        private SzdcTrain _selectedTrain;

        public SzdcTrain SelectedTrain {
            get => _selectedTrain;
            set => Update(ref _selectedTrain, value);
        }

        public ImmutableArray<SzdcTrain> Trains {
            get => _trains;
            protected set => Update(ref _trains, value);
        }

        public SelectionModel(TrainViewSelectionTool trainViewSelectionTool, IViewModel viewModel) {

            Trains = ImmutableArray<SzdcTrain>.Empty;

            if (trainViewSelectionTool == null || viewModel == null) {
                return;
            }

            _trainViewSelectionTool = trainViewSelectionTool;
            _viewModel = viewModel;

            SelectedTrain = _trainViewSelectionTool.SelectedTrainView?.Train;
            if (_viewModel.TrafficView != null) {

                Trains = _viewModel.TrafficView.TrainViews.Select(t => t.Train).ToImmutableArray();
                viewModel.TrafficView.PropertyChanged += (_, __) => {
                    Trains = viewModel.TrafficView.TrainViews.Select(t => t.Train).ToImmutableArray();
                };
            }

            PropertyChanged += (_, e) => SetSelection(e);

            trainViewSelectionTool.PropertyChanged += (_, __) => {
                SelectedTrain = _trainViewSelectionTool.SelectedTrainView.Train;
            };
        }

        private void SetSelection(PropertyChangedEventArgs e) {

            if (_trainViewSelectionTool == null || _viewModel == null) {
                return;
            }
            _trainViewSelectionTool.SelectedTrainView = _viewModel.TrafficView.TrainViews.FirstOrDefault(t => t.Train == SelectedTrain);
        }
    }

    public class TrainSelectionToolConverter : IMultiValueConverter {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            return new SelectionModel(values[0] as TrainViewSelectionTool, values[1] as IViewModel);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException(); // not impl.
        }
    }
}
