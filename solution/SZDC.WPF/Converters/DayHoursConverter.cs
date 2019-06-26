using System;
using System.Globalization;
using System.Windows.Data;

using GTTG.Core.Time;

namespace SZDC.Wpf.Converters {

    public class DayHoursConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            if (value is DayHoursInterval dayHoursInterval) {
                return $"{dayHoursInterval.StartHour} - {dayHoursInterval.EndHour}";
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException(); // Not impl.
        }
    }
}
