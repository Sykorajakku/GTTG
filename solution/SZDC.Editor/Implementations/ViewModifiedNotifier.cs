using SZDC.Editor.Interfaces;

namespace SZDC.Editor.Implementations {

    /// <summary>
    /// Shared structure with interface access to request redraw of timetable content.
    /// </summary>
    public class ViewModifiedNotifier : IViewModifiedNotifier {

        public event ViewModifiedHandler ViewModified;

        public void NotifyViewChange() {
            ViewModified?.Invoke();
        }
    }
}
