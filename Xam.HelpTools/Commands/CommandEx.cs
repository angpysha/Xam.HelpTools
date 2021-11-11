using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Input;

namespace Xam.HelpTools.Commands
{
    public class CommandEx<TViewModelType> : CommandEx<object, TViewModelType> where TViewModelType : class
    {
        public CommandEx(Action action, bool continueOnTheSameContext) : base(action, continueOnTheSameContext)
        {
        }

        public CommandEx(Action<object> action, bool continueOnTheSameContext) : base(action, continueOnTheSameContext)
        {
        }

        public CommandEx(Action<object> action, Expression<Func<TViewModelType, bool>> canExecuteExpression, WeakReference<TViewModelType> target, Action<Exception> onException = null, bool continueInTheSameContext = true, bool allowMultipleExecutions = true) : base(action, canExecuteExpression, target, onException, continueInTheSameContext, allowMultipleExecutions)
        {
        }

        public CommandEx(Action action, Expression<Func<TViewModelType, bool>> canExecuteExpression, WeakReference<TViewModelType> target, Action<Exception> onException = null, bool continueInTheSameContext = true, bool allowMultipleExecutions = true) : base(action, canExecuteExpression, target, onException, continueInTheSameContext, allowMultipleExecutions)
        {
        }
    }

    public class CommandEx<TParameterType, TViewModelType> : BaseCommandEx<TParameterType, TViewModelType>, ICommand where TViewModelType:class 
    {
        protected Action<TParameterType> execute;
        public CommandEx(Action action, bool continueOnTheSameContext) : this((d) => action(), continueOnTheSameContext)
        {
            
        }

        public CommandEx(Action<TParameterType> action, bool continueOnTheSameContext)
        {
            execute = action;
            _continueOnTheSameContext = continueOnTheSameContext;
        }

        public CommandEx(Action<TParameterType> action, Expression<Func<TViewModelType, bool>> canExecuteExpression, WeakReference<TViewModelType> target,
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

        public CommandEx(Action action, Expression<Func<TViewModelType, bool>> canExecuteExpression, WeakReference<TViewModelType> target,
            Action<Exception> onException = null,
            bool continueInTheSameContext = true, bool allowMultipleExecutions = true) : this(
            (d) => action(), canExecuteExpression, target, onException, continueInTheSameContext, allowMultipleExecutions)
        {

        }

        public void Execute(object parameter)
        {
            ExecutionCount++;
            try
            {
                execute((TParameterType)parameter);
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
    }
}
