using System.Windows;
using JiumiTool2.Extensions;
using JiumiTool2.IServices;
using JiumiTool2.Models;
using JiumiTool2.ViewModels;
using Microsoft.Extensions.Options;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace JiumiTool2.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : INavigationWindow
    {
        private readonly IAppsettingsService _appsettingsService;

        public MainViewModel ViewModel { get; }

        public MainView(MainViewModel viewModel,
                        IPageService pageService,
                        INavigationService navigationService,
                        IAppsettingsService appsettingsService,
                        IContentDialogService contentDialogService)
        {

            _appsettingsService = appsettingsService;
            ViewModel = viewModel;
            DataContext = this;

            SystemThemeWatcher.Watch(this);
            
            InitializeComponent();

            SetPageService(pageService);
            navigationService.SetNavigationControl(RootNavigation);
            contentDialogService.SetDialogHost(RootContentDialog);

            var theme = _appsettingsService.GetAppsettings().ApplicationTheme.ToEnum<ApplicationTheme>();
            ApplicationThemeManager.Apply(theme);
        }

        public INavigationView GetNavigation() => RootNavigation;

        public bool Navigate(Type pageType) => RootNavigation.Navigate(pageType);

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }

        public void SetPageService(IPageService pageService) => RootNavigation.SetPageService(pageService);

        public void ShowWindow() => Show();

        public void CloseWindow() => Close();

        /// <summary>
        /// 引发关闭事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }
    }
}
