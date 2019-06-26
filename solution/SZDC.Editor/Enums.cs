using System;

using SZDC.Editor.TrainTimetables;

namespace SZDC.Editor {

    public static class Enums {

        public static TrainTimetableType[] TimetableTypes { get; } = (TrainTimetableType[]) Enum.GetValues(typeof(TrainTimetableType));

    }
}
