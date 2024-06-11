using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JiumiTool2.Commons;
using JiumiTool2.Extensions;
using JiumiTool2.IServices;
using JiumiTool2.Models;
using JiumiTool2.Views;
using Org.BouncyCastle.Cms;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace JiumiTool2.ViewModels
{
    public partial class VideoViewModel : ObservableObject
    {
        private readonly IContentDialogService _contentDialogService;
        private readonly IHttpsProxyService _httpsProxyService;
        private readonly IAppsettingsService _appsettingsService;
        private readonly IDownloadService _downloadService;
        private readonly VideoConfigDialog _videoConfigDialog;
        private readonly VideoDownloadDialog _videoDownloadDialog;
        private readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

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

        /// <summary>
        /// 被选择视频列表
        /// </summary>
        private List<Video> _selectedItems = new();

        public VideoViewModel(IContentDialogService contentDialogService,
                              VideoConfigDialog videoConfigDialog,
                              VideoDownloadDialog videoDownloadDialog,
                              IHttpsProxyService httpsProxyService,
                              IDownloadService downloadService,
                              IAppsettingsService appsettingsService)
        {
            _contentDialogService = contentDialogService;
            _videoConfigDialog = videoConfigDialog;
            _videoDownloadDialog = videoDownloadDialog;
            _httpsProxyService = httpsProxyService;
            _downloadService = downloadService;

            WeakReferenceMessenger.Default.Register<VideoViewModel, InjectionResult, string>(this, "injectionResult", MessageHandler);
            _appsettingsService = appsettingsService;
        }

        /// <summary>
        /// 执行监视
        /// </summary>
        [RelayCommand]
        private void ExecuteMonitor()
        {
            DownloadEnable = true;
            if (BeginMonitor == true)
            {
                _httpsProxyService.Start();
            }
            else
            {
                _httpsProxyService.Stop();
            }
        }

        /// <summary>
        /// 进行下载
        /// </summary>
        [RelayCommand]
        private async Task ExecuteDownload()
        {
            if (BeginMonitor == true)
            {
                MessageBox messageBox = new MessageBox
                {
                    Title = "提示",
                    Content = "停止监听再进行下载",
                    CloseButtonText = "确认"
                };
                await messageBox.ShowDialogAsync();

                return;
            }

            var path = _appsettingsService.GetAppsettings().VideoOptions.DownloadPath;
            if (Directory.Exists(path) == false)
            {
                MessageBox messageBox = new MessageBox
                {
                    Title = "错误",
                    Content = "请设置下载路径，再进行下载",
                    CloseButtonText = "确认"
                };
                await messageBox.ShowDialogAsync();
                return;
            }

            if (_selectedItems.Count == 0)
            {
                MessageBox messageBox = new MessageBox
                {
                    Title = "警告",
                    Content = "请选择需要下载的视频",
                    CloseButtonText = "确认"
                };
                await messageBox.ShowDialogAsync();
                return;
            }

            WeakReferenceMessenger.Default.Send<IList<Video>, string>(_selectedItems, "DownloadVideo");

            _videoDownloadDialog.Title = "下载视频ing";
            _videoDownloadDialog.CloseButtonText = "确认";

            await _contentDialogService.ShowAsync(_videoDownloadDialog, default);
        }

        /// <summary>
        /// 设置
        /// </summary>
        [RelayCommand]
        private async Task PathSetting()
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
            Wpf.Ui.Controls.ListView messageListBox = currentPage.FindName("VideoListView") as Wpf.Ui.Controls.ListView;
            Border messageBorder = currentPage.FindName("VideoBorder") as Border;

            messageListBox.Height = 0;
            messageBorder.UpdateLayout();
            var actualHeight = messageBorder.ActualHeight;
            messageListBox.Height = actualHeight - 2;
        }

        /// <summary>
        /// ListView选择事件
        /// </summary>
        /// <param name="e"></param>
        [RelayCommand]
        private void SelectionChanged(SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var video = e.AddedItems[0] as Video;
                var isExist = _selectedItems.Exists(model => model.Url.Equals(video.Url));
                if (isExist == false)
                    _selectedItems.Add(video);
            }
            else if (e.RemovedItems.Count > 0)
            {
                var video = e.RemovedItems[0] as Video;
                _selectedItems.RemoveAll(model => model.Url.Equals(video.Url));
            }
        }

        #region 私有方法

        /// <summary>
        /// 处理发过来的信息
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="message"></param>
        private void MessageHandler(VideoViewModel recipient, InjectionResult message)
        {
            var isContain = recipient.VideoItems.Any(video => video.Url.Split("&token=")[0].Equals(message.Url.Split("&token=")[0]));
            if (isContain == true) return;

            _dispatcher.InvokeAsync(async () =>
            {
                using MemoryStream stream = new MemoryStream(await _downloadService.DownloadImage(message.ThumbUrl));
                var image = new BitmapImage();
                image.BeginInit();
                // 在加载完成后释放原始流
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();

                var video = new Video
                {
                    ImageData = image,
                    Description = message.Description,
                    VideoPlayLength = message.VideoPlayLength,
                    Size = message.Size / 1024 / 1024,
                    Uploader = message.Uploader,
                    Url = message.Url,
                    DecryptionArray = new Rng(message.Decodekey).GetDecryptionArray()
                };
                recipient.VideoItems.Add(video);
            });
        }

        #endregion
    }
}
