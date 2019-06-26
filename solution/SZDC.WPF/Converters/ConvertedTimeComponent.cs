using GTTG.Model.Model.Events;

namespace SZDC.WPF.Converters {
    
    /// <summary>
    /// Visualized UI model to make binding to, returned by converter
    /// </summary>
    public class ConvertedTimeComponent {

        public string StationName { get; set; }
        public string Time { get; set; }
        public TrainEventType TrainEventType { get; set; }
        public int TrainNumber { get; set; }
    }
}
