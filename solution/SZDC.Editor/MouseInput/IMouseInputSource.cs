using System;

namespace SZDC.Editor.MouseInput {

    public interface IMouseInputSource {

        IObservable<MouseInputArgs> RightUp { get; }
        IObservable<MouseInputArgs> RightDown { get; }
        IObservable<MouseInputArgs> LeftUp { get; }
        IObservable<MouseInputArgs> LeftDown { get; }
        IObservable<MouseInputArgs> Move { get; }
        IObservable<MouseZoomArgs> Scroll { get; }
        IObservable<MouseInputArgs> Enter { get; }
        IObservable<MouseInputArgs> Leave { get; }
    }
}
