using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Prism.Services;
using Xam.HelpTools.Commands;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Xam.HelpTools.Demo.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly IPageDialogService _pageDialogService;

        private AsyncCommandEx<object, MainPageViewModel> _pressbuttonCommand;
        public AsyncCommandEx<object, MainPageViewModel> PressButtonCommand => _pressbuttonCommand ?? (_pressbuttonCommand =
            new AsyncCommandEx<object, MainPageViewModel>(PressButtonCommandExecute, _canPressExpression, new WeakReference<MainPageViewModel>(this), allowMultipleExecutions: false));

        private Expression<Func<MainPageViewModel, bool>> _canPressExpression => x => x.CanPress;

        private bool _canPress;

        public bool CanPress
        {
            get => _canPress;
            set => SetProperty(ref _canPress, value);
        }

        private Command _buttonCOmmand2;
        public Command ButtonCOmmand2 => _buttonCOmmand2 ?? (_buttonCOmmand2 = new Command(ButtonCOmmand2Execute));


        private AsyncValueCommandEx<object, MainPageViewModel> _pressbuttonValueCommand;
        public AsyncValueCommandEx<object, MainPageViewModel> PressButtonValueCommand => _pressbuttonValueCommand ?? (_pressbuttonValueCommand =
            new AsyncValueCommandEx<object, MainPageViewModel>(PressButtonValueCommandExecute, _canPressExpression, new WeakReference<MainPageViewModel>(this), allowMultipleExecutions: false));

        private CommandEx<object, MainPageViewModel> _buttonPressend2Command;

        public CommandEx<object, MainPageViewModel> ButtonPressend2Command => _buttonPressend2Command ??
                                                                              (_buttonPressend2Command =
                                                                                  new CommandEx<object,
                                                                                      MainPageViewModel>(
                                                                                      buttonPressend2CommandExecute,
                                                                                      _canPressExpression,
                                                                                      new WeakReference<
                                                                                          MainPageViewModel>(this)
                                                                                      ));

      

        private void ButtonCOmmand2Execute()
        {
            CanPress = !CanPress;
        }

        public MainPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService)
        {
            _pageDialogService = pageDialogService;
            Title = "Main Page";
        }

        private async Task PressButtonCommandExecute(object t)
        {
            await Task.Delay(2500);
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await _pageDialogService.DisplayAlertAsync("Hello", "Button pressed", "Ok");
            });
        }

        private async ValueTask PressButtonValueCommandExecute(object parameter)
        {
            await Task.Delay(2500);
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await _pageDialogService.DisplayAlertAsync("Hello", "Value Command button pressed", "Ok");
            });
        }

        private void buttonPressend2CommandExecute()
        {
           // Thread.Sleep(1500);
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await _pageDialogService.DisplayAlertAsync("Hello", "Command button pressed", "Ok");
            });
        }
    }
}
