// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GTTG.Core.Base {

    /// <summary>Observable object, using <see cref="INotifyPropertyChanged"/>.</summary>
    public abstract class ObservableObject : INotifyPropertyChanged {

        /// <summary>Add handler to receive notification if property of derived class is updated.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Notify observers about property changes.</summary>
        /// <param name="propertyName">Name of property that is changed.</param>
        public void Notify(string propertyName = default(string)) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>Update property backing field and notify observers about property change.</summary>
        /// <typeparam name="T">The type of field.</typeparam>
        /// <param name="field">The field to update.</param>
        /// <param name="value">New value assigned to the field.</param>
        /// <param name="propertyName">Name of property that is changed.</param>
        /// <param name="notifyIfEqual">If true, notify is triggered even if <paramref name="field"/> and <paramref name="value"/> are equal.</param>
        /// <returns>True if backing field value changed; otherwise false.</returns>
        public bool Update<T>(ref T field, T value, bool notifyIfEqual = false, [CallerMemberName] string propertyName = default(string)) {

            if (!Equals(field, value) || notifyIfEqual) {
                field = value;
                Notify(propertyName);
                return true;
            }
            
            return false;
        }
    }
}
