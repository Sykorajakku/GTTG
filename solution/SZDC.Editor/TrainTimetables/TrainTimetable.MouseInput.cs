using System;
using System.Collections.Generic;
using System.ComponentModel;

using Autofac;
using SkiaSharp;

using GTTG.Core.Component;
using SZDC.Editor.MouseInput;

namespace SZDC.Editor.TrainTimetables {

    public abstract partial class TrainTimetable {

        protected readonly List<IDisposable> Registrations = new List<IDisposable>();
        protected readonly DragProcessor DragProcessor = new DragProcessor();
        protected IMouseInputSourceProvider MouseInputSourceProvider { get; set; }

        protected void InitializeMouseInput() {

            MouseInputSourceProvider = ComponentContext.Resolve<IMouseInputSourceProvider>();
            MouseInputSourceProvider.PropertyChanged += OnMouseInputChanged;
        }

        private void OnMouseInputChanged(object sender, PropertyChangedEventArgs e) {

            if (e.PropertyName != nameof(IMouseInputSourceProvider.MouseInputSource)) {
                return;
            }

            foreach (var registration in Registrations) {
                registration.Dispose();
            }

            OnEnableMouseInput();
        }

        protected virtual void OnEnableMouseInput() {

            var mouseInputSource = MouseInputSourceProvider.MouseInputSource;

            Registrations.Add(mouseInputSource?.LeftUp.Subscribe(LeftUp));
            Registrations.Add(mouseInputSource?.LeftDown.Subscribe(LeftDown));
            Registrations.Add(mouseInputSource?.Move.Subscribe(Move));
            Registrations.Add(mouseInputSource?.Scroll.Subscribe(Scroll));
            Registrations.Add(mouseInputSource?.Leave.Subscribe(Leave));
        }

        public void LeftUp(MouseInputArgs args) {
            DragProcessor.TryExitDrag(args);
        }

        public void LeftDown(MouseInputArgs args) {
            DragProcessor.TryInitializeDrag(args);
        }

        public void Move(MouseInputArgs args) {

            
            if (!DragProcessor.IsEnabled) {
                return;
            }

            var translation = DragProcessor.GetTranslation(args);
            if (ViewProvider.ContentMatrix.ScaleY.Equals(1.0f) && ViewProvider.ContentHeight.Equals(ViewProvider.ViewHeight)) {
                translation.Y = 0;
            }
            if (ViewProvider.ContentMatrix.ScaleX.Equals(1.0f) && ViewProvider.ContentWidth.Equals(ViewProvider.ViewWidth)) {
                translation.X = 0;
            }

            var result = GraphicalComponent.TryTranslate(translation);

            if (result == TranslationTransformationResult.ViewModified) {
                ViewModifiedNotifier.NotifyViewChange();
            }
        }

        public void Scroll(MouseZoomArgs args) {

            var result = GraphicalComponent.TryScale(new SKPoint(args.X, args.Y), args.Delta);

            if (result != ScaleTransformationResult.ViewUnmodified) {
                ViewModifiedNotifier.NotifyViewChange();
            }
        }

        public void Leave(MouseInputArgs args) {
            DragProcessor.TryExitDrag(args);
        }
    }
}
