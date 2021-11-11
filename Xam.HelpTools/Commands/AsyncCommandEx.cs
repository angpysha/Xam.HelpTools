using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Nito.AsyncEx;
using Nito.AsyncEx.Synchronous;
using Xam.HelpTools.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Xam.HelpTools.Commands
{

    public class AsyncCommandEx<TViewModelType> : AsyncCommandEx<object, TViewModelType>, ICommand where TViewModelType : class
    {
        public AsyncCommandEx(Func<Task> action, bool continueOnTheSameContext = true) : base(action, continueOnTheSameContext)
        {
        }

        public AsyncCommandEx(Func<object, Task> action, bool continueOnTheSameContext = true) : base(action, continueOnTheSameContext)
        {
        }

        public AsyncCommandEx(Func<object, Task> action, Expression<Func<TViewModelType, bool>> canExecuteExpression,
            WeakReference<TViewModelType> target,
            Action<Exception> onException = null,
            bool continueInTheSameContext = true, bool allowMultipleExecutions = true) : base(action,
            canExecuteExpression, target, onException, continueInTheSameContext, allowMultipleExecutions)
        {

        }

        public AsyncCommandEx(Func<Task> action, Expression<Func<TViewModelType, bool>> canExecuteExpression,
            WeakReference<TViewModelType> target,
            Action<Exception> onException = null,
            bool continueInTheSameContext = true, bool allowMultipleExecutions = true) : base(action,
            canExecuteExpression, target, onException, continueInTheSameContext, allowMultipleExecutions)
        {

        }
    }

    public class AsyncCommandEx<TParameterType, TViewModelType> : BaseCommandEx<TParameterType,TViewModelType>, ICommand where TViewModelType : class
    {
        protected Func<TParameterType, Task> execute;

        public AsyncCommandEx(Func<Task> action, bool continueOnTheSameContext) : this((d) => action(), continueOnTheSameContext)
        {

        }

        public AsyncCommandEx(Func<TParameterType, Task> action, bool continueOnTheSameContext)
        {
            execute = action;
            _continueOnTheSameContext = continueOnTheSameContext;
        }

        public AsyncCommandEx(Func<TParameterType, Task> action, Expression<Func<TViewModelType,bool>> canExecuteExpression, WeakReference<TViewModelType> target,
            Action<Exception> onException = null,
            bool continueInTheSameContext = true, bool allowMultipleExecutions = true)
        {
            execute = action;
            _canExecute = canExecuteExpression;
            _target = target;
            _continueOnTheSameContext = continueInTheSameContext;
            _allowMultipleExecutions = allowMultipleExecutions;
            _onException = onException;
        }

        public AsyncCommandEx(Func<Task> action, Expression<Func<TViewModelType, bool>> canExecuteExpression, WeakReference<TViewModelType> target,
            Action<Exception> onException = null,
            bool continueInTheSameContext = true, bool allowMultipleExecutions = true) : this(
            (d) => action(), canExecuteExpression, target, onException,continueInTheSameContext, allowMultipleExecutions)
        {

        }


        public async Task ExecuteAsync(TParameterType parameter)
        {
            ExecutionCount++;
            try
            {
                await execute(parameter).ConfigureAwait(_continueOnTheSameContext);
            }
            catch (Exception ex)
            {
                _onException?.Invoke(ex);
            }
            finally
            {
                if (--ExecutionCount <= 0)
                {
                    ExecutionCount = 0;
                }
            }
        }
        void ICommand.Execute(object parameter)
        {
            ExecuteAsync((TParameterType)parameter).RunAsync(_continueOnTheSameContext, _onException);

        }

    }
}
