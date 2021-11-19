using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xam.HelpTools.Commands;
using Xam.HelpTools.Demo.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Xam.HelpTools.Demo.Views.CommandsPage
{
    public class CommandsViewModel : ViewModelBase
    {
        private readonly IPageDialogService _pageDialogService;

        private AsyncCommandEx<object, CommandsViewModel> _pressbuttonCommand;
        public AsyncCommandEx<object, CommandsViewModel> PressButtonCommand => _pressbuttonCommand ?? (_pressbuttonCommand =
            new AsyncCommandEx<object, CommandsViewModel>(PressButtonCommandExecute, _canPressExpression, new WeakReference<CommandsViewModel>(this), allowMultipleExecutions: false));

        private Expression<Func<CommandsViewModel, bool>> _canPressExpression => x => x.CanPress;

        private bool _canPress;

        public bool CanPress
        {
            get => _canPress;
            set => SetProperty(ref _canPress, value);
        }

        private Command _buttonCOmmand2;
        public Command ButtonCOmmand2 => _buttonCOmmand2 ?? (_buttonCOmmand2 = new Command(ButtonCOmmand2Execute));


        private AsyncValueCommandEx<object, CommandsViewModel> _pressbuttonValueCommand;
        public AsyncValueCommandEx<object, CommandsViewModel> PressButtonValueCommand => _pressbuttonValueCommand ?? (_pressbuttonValueCommand =
            new AsyncValueCommandEx<object, CommandsViewModel>(PressButtonValueCommandExecute, _canPressExpression, new WeakReference<CommandsViewModel>(this), allowMultipleExecutions: false));

        // private CommandEx<object, CommandsViewModel> _buttonPressend2Command;
        //
        // public CommandEx<object, CommandsViewModel> ButtonPressend2Command => _buttonPressend2Command ??
        //                                                                       (_buttonPressend2Command =
        //                                                                           new CommandEx<object,
        //                                                                              CommandsViewModel>(
        //                                                                               buttonPressend2CommandExecute,
        //                                                                               _canPressExpression,
        //                                                                               new WeakReference<
        //                                                                                  CommandsViewModel>(this)
        //                                                                               ));
        //


        private void ButtonCOmmand2Execute()
        {
            CanPress = !CanPress;
        }
        public CommandsViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService)
        {
            _pageDialogService = pageDialogService;
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
