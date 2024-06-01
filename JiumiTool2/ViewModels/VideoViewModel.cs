using System.Collections.ObjectModel;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JiumiTool2.Models;
using JiumiTool2.Views;
using Wpf.Ui;

namespace JiumiTool2.ViewModels
{
    public partial class VideoViewModel : ObservableObject
    {
        private readonly IContentDialogService _contentDialogService;
        private readonly VideoConfigDialog _videoConfigDialog;

        /// <summary>
        /// 开始监视
        /// </summary>
        [ObservableProperty]
        private bool _beginMonitor = false;

        /// <summary>
        /// 启用下载按钮
        /// </summary>
        [ObservableProperty]
        private bool _downloadEnable = false;

        /// <summary>
        /// 视频列表
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Video> _videoItems = new();

        public VideoViewModel(IContentDialogService contentDialogService, VideoConfigDialog videoConfigDialog)
        {
            _contentDialogService = contentDialogService;
            _videoConfigDialog = videoConfigDialog;
        }

        /// <summary>
        /// 执行监视
        /// </summary>
        [RelayCommand]
        private void ExecuteMonitor()
        {

        }

        /// <summary>
        /// 进行下载
        /// </summary>
        [RelayCommand]
        private void ExecuteDownload()
        {

        }

        /// <summary>
        /// 设置
        /// </summary>
        [RelayCommand]
        private async void PathSetting()
        {
            _videoConfigDialog.Title = "下载路径配置";
            _videoConfigDialog.PrimaryButtonText = "确认";
            _videoConfigDialog.CloseButtonText = "取消";

            await _contentDialogService.ShowAsync(_videoConfigDialog, default);
        }

        /// <summary>
        /// 使listView跟随窗体大小变化
        /// </summary>
        /// <param name="sender"></param>
        [RelayCommand]
        private void PageSizeChanged(object sender)
        {
            // 确保sender是Page类型，尽管在本方法中sender通常就是当前Page实例
            if (sender is not Page currentPage)
            {
                return;
            }

            // 使用FindName方法查找子元素
            ListView messageListBox = currentPage.FindName("VideoListView") as ListView;
            Border messageBorder = currentPage.FindName("VideoBorder") as Border;

            messageListBox.Height = 0;
            messageBorder.UpdateLayout();
            var actualHeight = messageBorder.ActualHeight;
            messageListBox.Height = actualHeight - 2;
        }
    }
}
