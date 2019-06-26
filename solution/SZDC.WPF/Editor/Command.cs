using System;
using System.Windows.Input;

namespace SZDC.Wpf.Editor {

    public class Command : ICommand {

        private readonly Func<object, bool> _canExecute;
        private readonly Action<object> _execute;

#pragma warning disable CS0067 // Not used in this interface implementation as event
        public event EventHandler CanExecuteChanged;

        public Command(Func<object, bool> canExecute, Action<object> execute) {
            _canExecute = canExecute;
            _execute = execute;
        }

        public bool CanExecute(object parameter) {
            return _canExecute?.Invoke(parameter) == true;
        }

        public void Execute(object parameter) {
            _execute?.Invoke(parameter);
        }
    }
}
