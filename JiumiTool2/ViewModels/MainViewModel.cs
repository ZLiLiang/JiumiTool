using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JiumiTool2.Views;
using Wpf.Ui.Controls;

namespace JiumiTool2.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private ObservableCollection<object> _navigationItems = [];

        [ObservableProperty]
        private ObservableCollection<object> _navigationFooter = [];

        [RelayCommand]
        private void NavigationSelectionChanged(object sender)
        {
            if (sender is not NavigationView navigationView)
            {
                return;
            }

            navigationView.SetCurrentValue(
                NavigationView.HeaderVisibilityProperty,
                navigationView.SelectedItem?.TargetPageType != typeof(JiumiView)
                    ? Visibility.Visible
                    : Visibility.Collapsed
            );
        }

        public MainViewModel()
        {
            if (!_isInitialized)
            {
                InitializeViewModel();
            }
        }

        private void InitializeViewModel()
        {
            Title = "JiumiTool2";

            NavigationItems =
            [
                new NavigationViewItem()
                {
                    Content = "啾咪一下",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                    TargetPageType = typeof(JiumiView)
                },
                new NavigationViewItem()
                {
                    Content = "修改文件",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.Document24 },
                    TargetPageType = typeof(FileView)
                },
                new NavigationViewItem()
                {
                    Content = "下载视频",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.Video24 },
                    TargetPageType = typeof(VideoView)
                },
                //new NavigationViewItem()
                //{
                //    Content = "图片转换",
                //    Icon = new SymbolIcon { Symbol = SymbolRegular.Image24 },
                //    TargetPageType = typeof(VideoView)
                //}
            ];


            NavigationFooter =
            [
                new NavigationViewItem()
                {
                    Content = "设置",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                    TargetPageType = typeof(SettingsView)
                }
            ];

            _isInitialized = true;
            
        }
    }
}
