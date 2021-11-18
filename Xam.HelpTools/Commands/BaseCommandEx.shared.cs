using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Xam.HelpTools.Commands
{
    public abstract class BaseCommandEx<TParameterType, TViewModelType>  where TViewModelType : class
    {
        protected Expression<Func<TViewModelType, bool>> _canExecute;
        protected bool _expressionValue;
        protected WeakReference<TViewModelType> _target;
        protected WeakEventManager _weakEventManager = new WeakEventManager();
        protected bool _subscribed;
        protected bool _continueOnTheSameContext = true;
        protected Func<TViewModelType, bool> _getValueFunc;
        protected Action<TViewModelType, bool> _setValueFunc;
        protected bool _allowMultipleExecutions;
        protected Action<Exception> _onException;
        private SynchronizationContext _syncContext;

        private bool? _canExecuteOld;



        private volatile int _executionCount;

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
                    RaiseCanExecuteChanged();
            }
        }


        public void RaiseCanExecuteChanged()
        {
            if (MainThread.IsMainThread)
            {
                _weakEventManager.HandleEvent(this, EventArgs.Empty, nameof(CanExecuteChanged));
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() => _weakEventManager.HandleEvent(this, EventArgs.Empty, nameof(CanExecuteChanged)));
            }
        }


        public bool IsExecuting => ExecutionCount > 0;
        private bool _handleCanExecuteChanged = true;

        public virtual bool CanExecute(object parameter)
        {
            return (_allowMultipleExecutions, IsExecuting) switch
            {
                (true, _) => CanExecuteInternal(),
                (false, true) => DisableCanExecute(),
                (false, false) => CanExecuteInternal()
            };
        }

        private bool DisableCanExecute()
        {
            _handleCanExecuteChanged = false;

            if (_target !=null && _target.TryGetTarget(out var itm))
            {
                _canExecuteOld = _getValueFunc(itm);
                _setValueFunc(itm, false);
            }

            return false;
        }

        protected bool CanExecuteInternal()
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

                if (_setValueFunc == null)
                {
                    GetSetMethod(propertyInfo);
                }

                if (_canExecuteOld != null)
                {
                    _handleCanExecuteChanged = false;
                    _setValueFunc(vm, _canExecuteOld.Value);
                    _canExecuteOld = null;
                }

                _expressionValue = (bool)_getValueFunc(vm);
                return _expressionValue;
            }

            return true;
        }

        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
           // _weakEventManager.HandleEvent(this, EventArgs.Empty, nameof(CanExecuteChanged))
           //if (e.PropertyName == )
           var propertyInfo = GetCanExecutePropertyInfo();
           if (e.PropertyName == propertyInfo.Name)
           {
               if (_handleCanExecuteChanged == false)
               {
                   _handleCanExecuteChanged = true;
                   return;
               }
               RaiseCanExecuteChanged();
           }
        }

        protected void GetGetMethod(PropertyInfo propertyInfo)
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

        protected void GetSetMethod(PropertyInfo propertyInfo)
        {
            var tp = typeof(TViewModelType);
            var props = tp.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var propInfoEx = props.FirstOrDefault(x => x.Name == propertyInfo.Name);
            if (propInfoEx != null)
            {
                var setMethod = propInfoEx.GetSetMethod();
                var instanceParameter = Expression.Parameter(tp, "ins");
                var paramParameter = Expression.Parameter(typeof(bool), "val");

                var setterExpression = Expression.Lambda<Action<TViewModelType, bool>>(
                    Expression.Call(instanceParameter, setMethod, paramParameter), instanceParameter, paramParameter);

                var compiled = setterExpression.Compile();
                _setValueFunc = compiled;

            }
        }

        protected PropertyInfo GetCanExecutePropertyInfo()
        {
            if (_canExecute != null)
            {
                if (_canExecute is LambdaExpression member)
                {
                    if (member.Body is MemberExpression member1)
                    {
                         return GetPropertyInfoInternal(member1);
                         return GetPropertyInfoInternal(member1);
                    }
                    else if (member.Body is UnaryExpression expression)
                    {
                        var operand = expression.Operand;
                        if (operand is MemberExpression memberExpression)
                        {
                            return GetPropertyInfoInternal(memberExpression);
                        }
                    }
                }


                PropertyInfo GetPropertyInfoInternal(MemberExpression memberExpression)
                {
                    return memberExpression.Member as PropertyInfo;
                }
            }
            return null;
        }

        public event EventHandler CanExecuteChanged
        {
            add => _weakEventManager.AddEventHandler(value);
            remove => _weakEventManager.RemoveEventHandler(value);
        }
    }
}
