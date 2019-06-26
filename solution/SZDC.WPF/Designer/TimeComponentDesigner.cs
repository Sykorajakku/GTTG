using GTTG.Model.Model.Events;
using SZDC.WPF.Converters;

namespace SZDC.Wpf.Designer {

    public class TimeComponentDesigner : ConvertedTimeComponent {

        public TimeComponentDesigner() {
            TrainEventType = TrainEventType.Arrival;
            StationName = "Pardubice hl.n";
            TrainNumber = 13523;
            Time = "12:13:0";
        }
    }
}
