using System;
using System.Globalization;
using System.Windows.Data;

using SZDC.Model.Views.Traffic;

namespace SZDC.Wpf.Converters {

    /// <summary>
    /// Returns true if has instance of some train view.
    /// </summary>
    public class HasSelectedTrainConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value is SzdcTrainView;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException(); // Not impl.
        }
    }
}
