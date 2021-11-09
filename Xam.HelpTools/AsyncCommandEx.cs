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

namespace Xam.HelpTools
{

    public class AsyncCommandEx<TViewModelType> : AsyncCommandEx<object, TViewModelType> where TViewModelType : class
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

    public class AsyncCommandEx<TParameterType, TViewModelType> : ICommand where TViewModelType : class
    {
        private readonly Expression<Func<TViewModelType, bool>> _canExecute;
        private bool _expressionValue;
        private readonly WeakReference<TViewModelType> _target;
        private WeakEventManager _weakEventManager = new WeakEventManager();
        private bool _subscribed;
        private bool _continueOnTheSameContext = true;
        private Func<TViewModelType, bool> _getValueFunc;
        private bool _allowMultipleExecutions;
        private Action<Exception> _onException;

        private readonly Func<TParameterType, Task> execute;


        private int _executionCount;

        protected int ExecutionCount
        {
            get => _executionCount;
            set
            {
                var shouldRaiseCanExecuteChanged = (_allowMultipleExecutions, _executionCount, value) switch
                {
                    (true, _, _) => false,
                    (false, 0, > 0) => true,
                    (false, > 0, 0) => true,
                    (false, _, _) => false
                };

                _executionCount = value;

                if (shouldRaiseCanExecuteChanged)
                    _weakEventManager.HandleEvent(this, EventArgs.Empty, nameof(CanExecuteChanged));
            }
        }


        public bool IsExecuting => ExecutionCount > 0;

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

        public bool CanExecute(object parameter)
        {

            return (_allowMultipleExecutions, IsExecuting) switch
            {
                (true, _) => CanExecuteInternal(),
                (false, true) => false,
                (false, false) => CanExecuteInternal()
            };
            //  return true;
        }

        private bool CanExecuteInternal()
        {
            if (_canExecute != null && _target.TryGetTarget(out var vm))
            {
                var propertyInfo = GetCanExecutePropertyInfo();
                var propertyDescriptor = propertyInfo.ToPropertyDescriptor();
                if (_subscribed == false)
                {
                    //propertyDescriptor.AddValueChanged(vm, OnCanExecuteChanged);
                    ((INotifyPropertyChanged)vm).PropertyChanged += OnPropertyChanged;
                    _subscribed = true;
                }

                if (_getValueFunc == null)
                {
                    GetGetMethod(propertyInfo);
                }

                _expressionValue = (bool)_getValueFunc(vm);
                return _expressionValue;
            }

            return true;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _weakEventManager.HandleEvent(this, EventArgs.Empty, nameof(CanExecuteChanged));
        }

        private void GetGetMethod(PropertyInfo propertyInfo)
        {
            var tp = typeof(TViewModelType);
            var props = tp.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var propInfoEx = props.FirstOrDefault(x => x.Name == propertyInfo.Name);
            if (propInfoEx != null)
            {
                var getMethod = propInfoEx.GetGetMethod();
                var expressionParameter = Expression.Parameter(tp, "x");

                var getterExpression =
                    Expression.Lambda<Func<TViewModelType, bool>>(Expression.Call(expressionParameter, getMethod),
                        expressionParameter);

                var compiled = getterExpression.Compile();
                _getValueFunc = compiled;
            }
        }

        private void OnCanExecuteChanged(object sender, EventArgs e)
        {
            var propertyInfo = GetCanExecutePropertyInfo();
            _weakEventManager.HandleEvent(this, EventArgs.Empty, nameof(CanExecuteChanged));
        }

        private PropertyInfo GetCanExecutePropertyInfo()
        {
            if (_canExecute != null)
            {
                if (_canExecute is LambdaExpression member)
                {
                    if (member.Body is MemberExpression member1)
                    {
                        //  UpdateData(member1);
                        return GetPropertyInfoInternal(member1);
                    }
                    else if (member.Body is UnaryExpression expression)
                    {
                        var operand = expression.Operand;
                        if (operand is MemberExpression memberExpression)
                        {
                            return GetPropertyInfoInternal(memberExpression);
                            // UpdateData(memberExpression);
                        }
                        // var expr = (member as UnaryExpression).Operand as MemberExpression;
                        // 
                    }
                }


                PropertyInfo GetPropertyInfoInternal(MemberExpression memberExpression)
                {
                    return memberExpression.Member as PropertyInfo;
                }
            }
            return null;
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
            //AsyncContext.Run(() => ExecuteAsync((TParameterType)parameter));
            //   var tsk = Task.Run(() => ExecuteAsync((TParameterType)parameter));
            //  tsk.WaitAndUnwrapException();
            ExecuteAsync((TParameterType)parameter).RunAsync(_continueOnTheSameContext, _onException);

        }



        public event EventHandler CanExecuteChanged
        {
            add => _weakEventManager.AddEventHandler(value);
            remove => _weakEventManager.RemoveEventHandler(value);
        }
    }
}
