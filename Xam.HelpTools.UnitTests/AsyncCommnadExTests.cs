using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Xam.HelpTools.Commands;

namespace Xam.HelpTools.UnitTests
{
    public class AsyncCommandExTests : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _CanExecute;

        public bool CanExecute
        {
            get => _CanExecute;
            set
            {
                _CanExecute = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanExecute)));
            }
        }

        private Expression<Func<AsyncCommandExTests, bool>> CanExecExpr = x => x.CanExecute;

        [Test]
        public void TestIsExecuting()
        {
            var asyncCommand = new AsyncCommandEx<AsyncCommandExTests>(() =>Task.Run(async () =>
            {
                await Task.Delay(2000);
            }));
            asyncCommand.ExecuteAsync(null);
            Assert.AreEqual(asyncCommand.IsExecuting, true);
        }

        [Test]
        public async void TestAsyncCommandIsExecuteChanged()
        {
            var asyncCommand = new AsyncCommandEx<object,AsyncCommandExTests>(() => Task.Run(async () =>
            {
                await Task.Delay(2000);
                //CanExecute = !CanExecute;
            }), CanExecExpr, new WeakReference<AsyncCommandExTests>(this), allowMultipleExecutions:false);
            asyncCommand.ExecuteAsync(null);
            Assert.Equals(asyncCommand.CanExecute(null), false);
        }


    }
}
