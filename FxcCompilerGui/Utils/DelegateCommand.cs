using System;
using System.Windows.Input;

namespace FxcCompilerGui.Utils
{
    public class DelegateCommand : ICommand
    {
        public DelegateCommand(Action<object> method) : this(method, null)
        {
        }

        public DelegateCommand(Action<object> method, Predicate<object> canExecute)
        {
            _method = method;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _method.Invoke(parameter);
        }

        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            var handler = CanExecuteChanged;
            handler?.Invoke(this, e);
        }

        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged(EventArgs.Empty);
        }

        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _method;

        public event EventHandler CanExecuteChanged;
    }
}