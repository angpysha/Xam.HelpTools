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
using Xam.HelpTools.Demo.Views.CommandsPage;
using Xam.HelpTools.Demo.Views.AndroidFonts;
using Xam.HelpTools.Demo.Views.BrushedTextPage;

namespace Xam.HelpTools.Demo.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private AsyncCommandEx<EPageType> _navigateCommand;
        public AsyncCommandEx<EPageType> NavigateCommand => _navigateCommand ?? (_navigateCommand = new AsyncCommandEx<EPageType>(NavigateCommandExecute));

        public bool IsAndroid => Xamarin.Essentials.DeviceInfo.Platform == DevicePlatform.Android;

        public MainPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService)
        {
         
            Title = "Main Page";
        }
        private async Task NavigateCommandExecute(EPageType obj)
        {
            var pageName = obj switch
            {
                EPageType.Commands => nameof(CommandsPage),
                EPageType.AndroidText => nameof(AndroidFontsPage),
                EPageType.Brushes => nameof(BrushedTextPage),
                _ => throw new NotSupportedException()
            };
            await NavigationService.NavigateAsync(pageName);
        }

    }

    public enum EPageType
    {
        Commands,
        AndroidText,
        Brushes
    }
}
