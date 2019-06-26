using SkiaSharp;

using SZDC.Editor.Interfaces;
using SZDC.Editor.MouseInput;
using GTTG.Core.Base;
using GTTG.Core.Drawing.Layers;

namespace SZDC.Editor.Components {

    /// <summary>
    /// Connects <see cref="TrainTimetables.StaticTrainTimetable"/> with some UI element that displays <see cref="SKSurface"/> and receives mouse input on the <see cref="SKSurface"/>.
    /// <inheritdoc cref="ObservableObject" />
    /// </summary>
    public class TrainTimetableComponent : ObservableObject, IMouseInputSourceProvider {

        private readonly DrawingManager _drawingManager;

        private IMouseInputSource _mouseInputSource;

        /// <summary>
        /// Mouse input from backing UI element onto <see cref="T:SkiaSharp.SKSurface" />.
        /// <inheritdoc />
        /// </summary>
        public IMouseInputSource MouseInputSource {
            get => _mouseInputSource;
            set => Update(ref _mouseInputSource, value);
        }

        /// <summary>
        /// Handler for redraw requests to backing UI element.
        /// </summary>
        public ViewModifiedHandler ViewModified;

        public TrainTimetableComponent(DrawingManager drawingManager, 
                                       IViewModifiedNotifier viewModifiedNotifier) {
            _drawingManager = drawingManager;
            viewModifiedNotifier.ViewModified += ViewModifiedHandler;
        }
 
        /// <summary>
        /// Draws train graph content.
        /// </summary>
        /// <param name="surface">Backing UI element's surface onto which is train graph content drawn.</param>
        public void Draw(SKSurface surface) {
            _drawingManager.Draw(surface);
        }

        private void ViewModifiedHandler() {
            ViewModified?.Invoke();
        }
    }
}
