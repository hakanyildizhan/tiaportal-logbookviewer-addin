using Sirius.LogbookViewer.UI.Service;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Sirius.LogbookViewer.UI.Model
{
    /// <summary>
    /// A base view model that provides basic functionality (IoC, Property Changed handlers, etc.) to other view models.
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        protected object _propertyValueCheckLock = new object();
        public static IServiceContainer ServiceProvider;

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        /// <summary>
        /// Call this to fire a <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="name"></param>
        public void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Runs a command if the updating flag is not set.
        /// If the flag is true (indicating the function is already running) then the action is not run.
        /// If the flag is false (indicating no running function) then the action is run.
        /// Once the action is finished if it was run, then the flag is reset to false.
        /// </summary>
        /// <param name="updatingFlag">The boolean property flag defining if the command is already running.</param>
        /// <param name="checkFlag">True if the <see cref="updatingFlag"/> should be checked before running the command.</param>
        /// <param name="action">The action to run if the command is not already running.</param>
        /// <returns></returns>
        protected async Task RunCommandAsync(Expression<Func<bool>> updatingFlag, bool checkFlag, Func<Task> action)
        {
            // ensure single access to check
            lock (_propertyValueCheckLock)
            {
                // do nothing if the function is already running
                if (checkFlag && updatingFlag.GetPropertyValue())
                {
                    return;
                }
                    
                // Set the property flag to indicate we are running
                updatingFlag.SetPropertyValue(true);
                OnPropertyChanged(((updatingFlag.Body as MemberExpression).Member as System.Reflection.PropertyInfo).Name);
            }

            try
            {
                await action();
            }
            finally
            {
                // Set the property flag back to false
                updatingFlag.SetPropertyValue(false);
                OnPropertyChanged(((updatingFlag.Body as MemberExpression).Member as System.Reflection.PropertyInfo).Name);
            }
        }
    }
}
