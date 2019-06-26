using System.ComponentModel;

using SZDC.Model.Views.Infrastructure;
using SZDC.Model.Views.Traffic;

namespace SZDC.Model.Views {

    public interface IViewModel : INotifyPropertyChanged {

        SzdcRailwayView RailwayView { get; set; }
        SzdcTrafficView TrafficView { get; set; }
    }
}
