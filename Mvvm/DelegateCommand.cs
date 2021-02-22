using System;
using System.Windows.Input;

namespace DiskSpaceAnalyzer.Mvvm
{
    public class DelegateCommand : ICommand
    {
        private readonly Action _action;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action();
        }
    }

    public class DelegateCommand<ParameterType> : ICommand
    {
        private readonly Action<ParameterType> _action;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<ParameterType> action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action((ParameterType)parameter);
        }
    }
}
