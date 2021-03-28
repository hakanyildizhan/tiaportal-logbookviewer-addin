using System;
using System.Windows.Input;

namespace Sirius.LogbookViewer.UI
{
    /// <summary>
    /// Command wrapper to run an <see cref="Action"/>.
    /// </summary>
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// The action to run.
        /// </summary>
        private Action _action;

        /// <summary>
        /// Fired when the <see cref="CanExecute(object)"/> value has changed
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        public RelayCommand(Action action)
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
}
