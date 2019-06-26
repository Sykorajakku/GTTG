using System;
using System.Globalization;
using System.Windows.Data;

namespace SZDC.Wpf.Converters {

    /// <summary>
    /// True if view is scaled
    /// </summary>
    public class ScaleToIsScaledConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is float f) {
                return f <= 1;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException(); // not impl.
        }
    }
}
