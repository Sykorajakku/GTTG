using System;
using System.Globalization;
using System.Windows.Data;

namespace SZDC.Wpf.Converters {

    /// <summary>
    /// True if both values non-null
    /// </summary>
    public class IsSelectedConverter : IMultiValueConverter {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            // if selected railway and it's segment --> both not null
            return values[0] != null && values[1] != null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException(); // Not impl.
        }
    }
}
