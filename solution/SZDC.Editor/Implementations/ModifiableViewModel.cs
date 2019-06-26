using GTTG.Core.Base;
using SZDC.Model.Views;
using SZDC.Model.Views.Infrastructure;
using SZDC.Model.Views.Traffic;

namespace SZDC.Editor.Implementations {

    /// <summary>
    /// View model of railway and traffic passed down to application logic parts.
    /// </summary>
    public class ModifiableViewModel : ObservableObject, IViewModel {

        private SzdcTrafficView _trafficView;
        private SzdcRailwayView _railwayView;

        public SzdcTrafficView TrafficView {
            get => _trafficView;
            set => Update(ref _trafficView, value);
        }

        public SzdcRailwayView RailwayView {
            get => _railwayView;
            set => Update(ref _railwayView, value);
        }
    }
}
