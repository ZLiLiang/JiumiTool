﻿using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
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
                }
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
