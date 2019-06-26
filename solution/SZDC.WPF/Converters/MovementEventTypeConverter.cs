using System;
using System.Globalization;
using System.Windows.Data;

using SZDC.Model.Events;

namespace SZDC.Wpf.Converters {

    public class MovementEventTypeConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            if (value is SzdcTrainEvent trainMovementEvent) {
                return trainMovementEvent.TrainEventType;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException(); // Not impl.
        }
    }
}
