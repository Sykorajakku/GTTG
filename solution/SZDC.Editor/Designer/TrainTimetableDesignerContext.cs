using System;
using System.Collections.Generic;
using Autofac;

using GTTG.Core.Time;
using SZDC.Editor.Implementations;
using SZDC.Editor.Tools;
using SZDC.Editor.TrainTimetables;

namespace SZDC.Editor.Designer {

    /// <summary>
    /// Used as instance in UI designer.
    /// </summary>
    public class TrainTimetableDesignerContext : StaticTrainTimetable {

        private readonly DateTimeContext _designerDateTimeContext = new DateTimeContext(
            new DateTimeInterval(DateTime.Today.AddHours(4), DateTime.Today.AddHours(10)),
            new DateTimeInterval(DateTime.Today.AddHours(6), DateTime.Today.AddHours(8))) {
        };

        public TrainTimetableDesignerContext(IComponentContext componentContext) : base(componentContext) {

            TimetableInfo = new TimetableInfo {
                RailwayNumber = "505",
                FirstStationName = "Pardubice hl.n",
                LastStationName = "Jaroměř",
                TimetableType = TrainTimetableType.Static
            };
            Tools = new ToolsCollector(componentContext);
            UpdateDateTimeContext(_designerDateTimeContext);
            Tools.CurrentDateTimeTool.CurrentDateTime = DateTime.Today.AddHours(5).AddMinutes(15);
            Tools.ViewTimeModifierTool.SelectedTimeInterval = new TimeInterval(1, 30);
            Tools.ViewTimeModifierTool.TimeIntervals = new List<TimeInterval>(new [] { new TimeInterval(1, 30) });
        }
    }
}
