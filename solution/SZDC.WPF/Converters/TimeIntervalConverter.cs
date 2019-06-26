using System;
using System.Globalization;
using System.Windows.Data;

using GTTG.Core.Time;

namespace SZDC.Wpf.Converters {

    public class TimeIntervalConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            if (value is DateTimeInterval dateTimeInterval) {
                return dateTimeInterval.ToString("H:mm");
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException(); // not impl.
        }
    }
}
