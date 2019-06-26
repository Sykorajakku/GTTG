using System;
using System.Collections.Generic;
using System.ComponentModel;

using GTTG.Core.Base;
using SZDC.Editor.MouseInput;

namespace SZDC.Editor.Tools {

    /// <summary>
    /// Tool that receives mouse input. Provides option to unsubscribe tool from receiving input.
    /// </summary>
    /// <inheritdoc />
    public abstract class MouseInputTool : ObservableObject {

        private readonly IMouseInputSourceProvider _mouseInputSourceProvider;
        private bool _isEnabled;
        protected List<IDisposable> Observers;

        protected MouseInputTool(IMouseInputSourceProvider mouseInputSourceProvider) {

            Observers = new List<IDisposable>();
            _isEnabled = false;
            _mouseInputSourceProvider = mouseInputSourceProvider;
            _mouseInputSourceProvider.PropertyChanged += ChangeInputSource;
        }

        public void EnableMouseInput() {
            _isEnabled = true;
            OnEnableMouseInput();
        }

        protected abstract void OnEnableMouseInput();

        public virtual void DisableMouseInput() {

            foreach (var observer in Observers) {
                observer?.Dispose();
            }

            Observers.Clear();
        }

        private void ChangeInputSource(object sender, PropertyChangedEventArgs e) {

            if (e.PropertyName == nameof(_mouseInputSourceProvider.MouseInputSource)) {

                DisableMouseInput();
                if (_isEnabled) EnableMouseInput();
            }
        }
    }
}
