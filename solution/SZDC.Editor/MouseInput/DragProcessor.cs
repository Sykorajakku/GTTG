using SkiaSharp;

namespace SZDC.Editor.MouseInput {

    public enum DragState { On, Off }
    public enum DragStateTransition { Changed, Unchanged }

    /// <summary>
    /// Manages state of mouse drag and tracks mouse position. If in drag, deltas of previous positions are stored and reported.
    /// </summary>
    public class DragProcessor {

        public DragState DragState { get; private set; }
        public bool IsEnabled => DragState == DragState.On;

        private (float x, float y) _previous;

        public DragProcessor() {
            DragState = DragState.Off;
        }
        
        /// <summary>
        /// Starts drag. Stores this <paramref name="mouseInputArgs"/> position.
        /// </summary>
        public DragStateTransition TryInitializeDrag(MouseInputArgs mouseInputArgs) {

            if (DragState != DragState.Off) {
                return DragStateTransition.Unchanged;
            }

            DragState = DragState.On;
            _previous = (mouseInputArgs.X, mouseInputArgs.Y);
            return DragStateTransition.Changed;
        }

        /// <summary>
        /// Exits drag.
        /// </summary>
        public DragStateTransition TryExitDrag(MouseInputArgs mouseInputArgs) {

            if (DragState != DragState.On) {
                return DragStateTransition.Unchanged;
            }
            
            DragState = DragState.Off;
            return DragStateTransition.Changed;
        }

        /// <summary>
        /// Moves mouse cursor and returns delta to previous position.
        /// </summary>
        /// <param name="mouseInputArgs">New position.</param>
        /// <returns>Vector as delta from previous event to this <paramref name="mouseInputArgs"/>.</returns>
        public SKPoint GetTranslation(MouseInputArgs mouseInputArgs) {

            var (x, y) = (mouseInputArgs.X - _previous.x, mouseInputArgs.Y - _previous.y);
            _previous = (mouseInputArgs.X, mouseInputArgs.Y);
            return new SKPoint(-x, -y);
        }

        public void Reset() {
            DragState = DragState.Off;
            _previous = (0, 0);
        }
    }
}
