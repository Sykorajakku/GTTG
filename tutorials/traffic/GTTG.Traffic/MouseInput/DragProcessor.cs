using SkiaSharp;

namespace GTTG.Traffic.MouseInput {

    public enum DragState { On, Off }
    public enum DragStateTransition { Changed, Unchanged }

    public class DragProcessor {

        public DragState DragState { get; private set; }
        public bool IsEnabled => DragState == DragState.On;

        private (float x, float y) _previous;

        public DragProcessor() {
            DragState = DragState.Off;
        }
        
        public DragStateTransition TryInitializeDrag(MouseInputArgs mouseInputArgs) {

            if (DragState != DragState.Off) {
                return DragStateTransition.Unchanged;
            }

            DragState = DragState.On;
            _previous = (mouseInputArgs.X, mouseInputArgs.Y);
            return DragStateTransition.Changed;
        }

        public DragStateTransition TryExitDrag(MouseInputArgs mouseInputArgs) {

            if (DragState != DragState.On) {
                return DragStateTransition.Unchanged;
            }
            
            DragState = DragState.Off;
            return DragStateTransition.Changed;
        }

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
