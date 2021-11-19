using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Xam.HelpTools.Commands
{
    class CommandEx<TViewModelType> : CommandEx<object, TViewModelType> where TViewModelType : class
    {
        public CommandEx(Action action) : base(action)
        {
        }

        public CommandEx(Action<object> action) : base(action)
        {
        }

        public CommandEx(Action<object> action, Expression<Func<TViewModelType, bool>> canExecuteExpression, WeakReference<TViewModelType> target, Action<Exception> onException = null) : 
            base(action, canExecuteExpression, target, onException)
        {
        }

        public CommandEx(Action action, Expression<Func<TViewModelType, bool>> canExecuteExpression, WeakReference<TViewModelType> target, Action<Exception> onException = null) :
            base(action, canExecuteExpression, target, onException)
        {
        }
    }

    class CommandEx<TParameterType, TViewModelType> : BaseCommandEx<TParameterType, TViewModelType>, ICommand where TViewModelType:class 
    {
        protected Action<TParameterType> execute;
        public CommandEx(Action action) : this((d) => action())
        {
            
        }

        public CommandEx(Action<TParameterType> action)
        {
            execute = action;
        }

        public CommandEx(Action<TParameterType> action, Expression<Func<TViewModelType, bool>> canExecuteExpression, WeakReference<TViewModelType> target,
            Action<Exception> onException = null)
        {
            execute = action;
            _canExecute = canExecuteExpression;
            _target = target;
            _onException = onException;
        }

        public CommandEx(Action action, Expression<Func<TViewModelType, bool>> canExecuteExpression, WeakReference<TViewModelType> target,
            Action<Exception> onException = null,
            bool continueInTheSameContext = true) : this(
            (d) => action(), canExecuteExpression, target, onException)
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
