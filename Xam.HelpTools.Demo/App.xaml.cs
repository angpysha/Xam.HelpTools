using Prism;
using Prism.Ioc;
using Xam.HelpTools.Demo.ViewModels;
using Xam.HelpTools.Demo.Views;
using Xam.HelpTools.Demo.Views.AndroidFonts;
using Xam.HelpTools.Demo.Views.BrushedTextPage;
using Xam.HelpTools.Demo.Views.CommandsPage;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace Xam.HelpTools.Demo
{
    public partial class App
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<CommandsPage, CommandsViewModel>();
            containerRegistry.RegisterForNavigation<AndroidFontsPage>();
            containerRegistry.RegisterForNavigation<BrushedTextPage>();
        }
    }
}
