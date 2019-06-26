using System;
using System.Globalization;
using System.Windows.Data;

using SZDC.Model.Components;
using SZDC.Model.Infrastructure;
using SZDC.WPF.Converters;

namespace SZDC.Wpf.Converters {

    public class HitTestComponentConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            if (value is TimeComponent timeComponent) {
                var trainEventType = timeComponent.TrainEvent.TrainEventType;
                var stationName = ((SzdcStation) timeComponent.TrainEvent.Station).StationName;
                var time = ToRailwayTime(timeComponent.TrainEvent.DateTime);
                var trainNumber = timeComponent.TrainView.Train.TrainNumber;

                return new ConvertedTimeComponent {
                    StationName = stationName,
                    Time = time,
                    TrainNumber = trainNumber,
                    TrainEventType = trainEventType
                };
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException(); // not impl.
        }

        private string ToRailwayTime(DateTime dateTime) {
            var moreThan30Seconds = dateTime.Second >= 30 ? '3' : '0';
            return $"{dateTime.Hour}:{dateTime.Minute}:{moreThan30Seconds}";
        }
    }
}
