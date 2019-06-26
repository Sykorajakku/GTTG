namespace SZDC.Editor.MouseInput {

    public interface IMouseInputTarget {

        void RightUp(MouseInputArgs args);
        void RightDown(MouseInputArgs args);
        void LeftUp(MouseInputArgs args);
        void LeftDown(MouseInputArgs args);
        void Move(MouseInputArgs args);
        void ScrollUp(MouseZoomArgs args);
        void ScrollDown(MouseZoomArgs args);
        void Enter(MouseInputArgs args);
        void Leave(MouseInputArgs args);
    }
}
