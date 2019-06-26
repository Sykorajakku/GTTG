using System;
using System.Globalization;
using System.Windows.Data;
using SZDC.Editor.Interfaces;

namespace SZDC.Wpf.Converters {

    /// <summary>
    /// Returns string "first station name - last station name" of railway segment
    /// </summary>
    public class RailwaySegmentConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            if (!(value is RailwaySegmentBriefDescription railwaySegmentDescription)) {
                return string.Empty;
            }

            var stationsCount = railwaySegmentDescription.StationsInSegment.Count;

            if (stationsCount == 1) {
                return railwaySegmentDescription.StationsInSegment[0];
            }

            var firstStation = railwaySegmentDescription.StationsInSegment[0];
            var lastStation = railwaySegmentDescription.StationsInSegment[stationsCount - 1];
            return $"{firstStation} - {lastStation}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException(); // Not impl.
        }
    }
}
