using System;

namespace SZDC.Editor.MouseInput {

    public static class MouseInputConnector {

        public static void Connect(IMouseInputSource source, IMouseInputTarget target) {

            source.Move.Subscribe(
                target.Move
            );

            source.RightUp.Subscribe(
                target.RightUp
            );

            source.RightDown.Subscribe(
                target.RightDown
            );

            source.LeftUp.Subscribe(
                target.LeftUp
            );

            source.LeftDown.Subscribe(
                target.LeftDown
            );

            source.Scroll.Subscribe(
                args => {
                    if (args.Delta > 0) {
                        target.ScrollUp(args);
                    } else {
                        target.ScrollDown(args);
                    }
                });

            source.Enter.Subscribe(
                target.Enter
            );

            source.Leave.Subscribe(
                target.Leave
            );
        }
    }
}
