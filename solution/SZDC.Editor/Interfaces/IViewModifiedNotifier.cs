namespace SZDC.Editor.Interfaces {

    public delegate void ViewModifiedHandler();

    /// <summary>
    /// Contract for entity with <see cref="ViewModified"/> where handlers for redraw requests are added.
    /// </summary>
    public interface IViewModifiedNotifier {

        event ViewModifiedHandler ViewModified;
    }
}
