using GTTG.Model.Model.Traffic;
using GTTG.Model.ViewModel.Traffic;
using SZDC.Model.Infrastructure.Trains;
using SZDC.Model.Views.Traffic;

namespace SZDC.Model.Factories {

    public class SzdcTrafficViewFactory : ITrafficViewFactory<SzdcTrafficView, SzdcTrainView, SzdcTrain> {

        private readonly SzdcTrainViewFactory _trainViewFactory;

        public SzdcTrafficViewFactory(SzdcTrainViewFactory trainViewFactory) {
            _trainViewFactory = trainViewFactory;
        }

        public SzdcTrafficView CreateTrafficView(Traffic<SzdcTrain> traffic) {
            return new SzdcTrafficView(traffic, _trainViewFactory);
        }
    }
}
