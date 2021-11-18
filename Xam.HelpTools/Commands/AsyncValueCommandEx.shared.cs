using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xam.HelpTools.Commands;
using Xam.HelpTools.Helpers;

namespace Xam.HelpTools.Commands
{
    public class AsyncValueCommandEx<TParameterType> : AsyncCommandEx<TParameterType, object>
    {
        public AsyncValueCommandEx(Func<Task> action, bool continueOnTheSameContext) : base(action, continueOnTheSameContext)
        {
        }

        public AsyncValueCommandEx(Func<TParameterType, Task> action, bool continueOnTheSameContext) : base(action, continueOnTheSameContext)
        {
        }

      
    }
    public class AsyncValueCommandEx<TParameterType, TViewModelType> : BaseCommandEx<TParameterType, TViewModelType>, ICommand where TViewModelType: class
    {
        protected Func<TParameterType, ValueTask> execute;
        public AsyncValueCommandEx(Func<ValueTask> action, bool continueOnTheSameContext) : this((d) => action(), continueOnTheSameContext)
        {

        }

        public AsyncValueCommandEx(Func<TParameterType, ValueTask> action, bool continueOnTheSameContext)
        {
            execute = action;
            _continueOnTheSameContext = continueOnTheSameContext;
        }

        public AsyncValueCommandEx(Func<TParameterType, ValueTask> action, Expression<Func<TViewModelType, bool>> canExecuteExpression, WeakReference<TViewModelType> target,
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

        public AsyncValueCommandEx(Func<ValueTask> action, Expression<Func<TViewModelType, bool>> canExecuteExpression, WeakReference<TViewModelType> target,
            Action<Exception> onException = null,
            bool continueInTheSameContext = true, bool allowMultipleExecutions = true) : this(
            (d) => action(), canExecuteExpression, target, onException, continueInTheSameContext, allowMultipleExecutions)
        {

        }

        public async ValueTask ExecuteAsync(TParameterType parameter)
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
