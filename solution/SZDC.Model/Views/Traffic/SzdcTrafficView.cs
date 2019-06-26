using System.Collections.Generic;
using System.Linq;

using GTTG.Model.Model.Traffic;
using GTTG.Model.ViewModel.Traffic;
using SZDC.Model.Infrastructure.Trains;

namespace SZDC.Model.Views.Traffic {

    public class SzdcTrafficView : TrafficView<SzdcTrainView, SzdcTrain> {

        private readonly ITrainViewFactory<SzdcTrainView, SzdcTrain> _trainViewFactory;

        public SzdcTrafficView(Traffic<SzdcTrain> traffic, ITrainViewFactory<SzdcTrainView, SzdcTrain> trainViewFactory) :

            base(traffic, trainViewFactory) {
            _trainViewFactory = trainViewFactory;
        }

        public void Add(List<SzdcTrain> newTrains) {
            TrainViews = TrainViews.AddRange(newTrains.Select(t => _trainViewFactory.CreateTrainView(t)));
        }

        public void Remove(SzdcTrainView trainView) {
            TrainViews = TrainViews.Remove(trainView);
        }
    }
}
