using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JiumiTool2.Extensions;
using JiumiTool2.IServices;
using JiumiTool2.Models;
using Microsoft.Extensions.Options;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace JiumiTool2.ViewModels
{
    public partial class SettingsViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;
        private readonly IAppsettingsService _appsettingsService;

        public SettingsViewModel(IAppsettingsService appsettingsService)
        {
            _appsettingsService = appsettingsService;
        }

        [ObservableProperty]
        private ApplicationTheme _currentApplicationTheme = ApplicationTheme.Unknown;

        [RelayCommand]
        private void OnChangeTheme(string parameter)
        {
            switch (parameter)
            {
                case "theme_light":
                    if (CurrentApplicationTheme == ApplicationTheme.Light)
                    {
                        break;
                    }

                    ApplicationThemeManager.Apply(ApplicationTheme.Light);
                    CurrentApplicationTheme = ApplicationTheme.Light;
                    _appsettingsService.UpdateAppsettingsAsync(appsettings =>
                    {
                        appsettings.ApplicationTheme = ApplicationTheme.Light.ToString();
                    });

                    break;

                default:
                    if (CurrentApplicationTheme == ApplicationTheme.Dark)
                    {
                        break;
                    }

                    ApplicationThemeManager.Apply(ApplicationTheme.Dark);
                    CurrentApplicationTheme = ApplicationTheme.Dark;
                    _appsettingsService.UpdateAppsettingsAsync(appsettings =>
                    {
                        appsettings.ApplicationTheme = ApplicationTheme.Dark.ToString();
                    });

                    break;
            }
        }

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
            {
                InitializeViewModel();
            }
        }

        public void OnNavigatedFrom()
        {

        }

        private void InitializeViewModel()
        {
            CurrentApplicationTheme = ApplicationThemeManager.GetAppTheme();

            _isInitialized = true;
        }
    }
}
