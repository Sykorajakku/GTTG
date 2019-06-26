using GTTG.Infrastructure.MouseInput;

using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using SkiaSharp.Views.WPF;

namespace GTTG.Infrastructure {

    public class WpfMouseInputSource : IMouseInputSource {

        public IObservable<MouseInputArgs> RightUp { get; }
        public IObservable<MouseInputArgs> RightDown { get; }
        public IObservable<MouseInputArgs> LeftUp { get; }
        public IObservable<MouseInputArgs> LeftDown { get; }
        public IObservable<MouseInputArgs> Move { get; }
        public IObservable<MouseZoomArgs> Scroll { get; }
        public IObservable<MouseInputArgs> Enter { get; }
        public IObservable<MouseInputArgs> Leave { get; }

        public WpfMouseInputSource(SKElement inputElement) {

            RightUp = RegisterEvent<MouseButtonEventArgs>
                (inputElement, nameof(inputElement.MouseRightButtonUp)).ToMouseInputArgs(inputElement);
            RightDown = RegisterEvent<MouseButtonEventArgs>
                (inputElement, nameof(inputElement.MouseRightButtonDown)).ToMouseInputArgs(inputElement);
            LeftUp = RegisterEvent<MouseButtonEventArgs>
                (inputElement, nameof(inputElement.MouseLeftButtonUp)).ToMouseInputArgs(inputElement);
            LeftDown = RegisterEvent<MouseButtonEventArgs>
                (inputElement, nameof(inputElement.MouseLeftButtonDown)).ToMouseInputArgs(inputElement);

            Move = RegisterEvent<MouseEventArgs>
                (inputElement, nameof(inputElement.MouseMove)).ToMouseInputArgs(inputElement);

            Enter = RegisterEvent<MouseEventArgs>
                (inputElement, nameof(inputElement.MouseEnter)).ToMouseInputArgs(inputElement);
            Leave = RegisterEvent<MouseEventArgs>
                (inputElement, nameof(inputElement.MouseLeave)).ToMouseInputArgs(inputElement);

            Scroll = RegisterEvent<MouseWheelEventArgs>
                (inputElement, nameof(inputElement.MouseWheel)).ToMouseZoomInputArgs(inputElement);
        }

        private static IObservable<EventPattern<T>> RegisterEvent<T>(IInputElement source, string name) {
            return Observable.FromEventPattern<T>(source, name);
        }
    }

    public static class ObservableExtensions {

        public static IObservable<MouseInputArgs> ToMouseInputArgs<T>(this IObservable<EventPattern<T>> t, SKElement control)
            where T : MouseEventArgs {

            return t.Select(e => {

                var position = e.EventArgs.GetPosition(control);
                return new MouseInputArgs(
                    (float)(position.X * control.CanvasSize.Width / control.ActualWidth),
                    (float)(position.Y * control.CanvasSize.Height / control.ActualHeight)
                );
            });
        }

        public static IObservable<MouseZoomArgs> ToMouseZoomInputArgs<T>(this IObservable<EventPattern<T>> t, SKElement control)
            where T : MouseWheelEventArgs {

            return t.Select(e => {

                var position = e.EventArgs.GetPosition(control);
                var mouseInputArgs = new MouseInputArgs(
                    (float)(position.X * control.CanvasSize.Width / control.ActualWidth),
                    (float)(position.Y * control.CanvasSize.Height / control.ActualHeight)
                );
                return new MouseZoomArgs(mouseInputArgs, e.EventArgs.Delta / 120.0f);
            });
        }
    }
}
