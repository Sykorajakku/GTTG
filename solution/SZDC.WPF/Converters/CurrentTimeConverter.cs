using System;
using System.Globalization;
using System.Windows.Data;

namespace SZDC.Wpf.Converters {

    /// <summary>
    /// Converts <see cref="DateTime"/> to string representation.
    /// </summary>
    public class CurrentTimeConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            if (value is null) {
                return "--:--:-";
            }

            if (value is DateTime currentTimeValue) {

                var hours = currentTimeValue.Hour;
                var minutes = currentTimeValue.Minute;
                var halfMinute = currentTimeValue.Second >= 30 ? 30 : 0; // time in timetable truncated to 30 seconds

                return $"{hours}:{minutes}:{halfMinute}";
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException(); // Not impl.
        }
    }
}
