using System;

using GTTG.Core.Time;
using SZDC.Editor.Interfaces;

namespace SZDC.Editor.Selectors { 

    public class DynamicSelector : ITimeSelector {

        public DateTimeContext ToDateTimeContext() {
            throw new NotImplementedException(); // TODO: future versions, if we want to support selectable view intervals in dynamic mode
        }
    }
}
