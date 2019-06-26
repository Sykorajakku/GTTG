using System;
using System.Globalization;
using System.Windows.Data;

using SZDC.Editor.TrainTimetables;
using SZDC.Wpf.Editor;

namespace SZDC.Wpf.Converters {

    public class OpenCommandConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            if (value is TrainTimetableType trainTimetableType) {

                if (trainTimetableType == TrainTimetableType.Static) {
                    return ProjectEditorCommands.OpenStaticTimetable;
                }
                else if (trainTimetableType == TrainTimetableType.Realtime) {
                    return ProjectEditorCommands.OpenDynamicTimetable;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException(); // Not impl.
        }
    }
}
