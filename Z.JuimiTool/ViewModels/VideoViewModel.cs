using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Z.JiumiTool.IServices;
using Z.JiumiTool.Models;

namespace Z.JiumiTool.ViewModels
{
    public class VideoViewModel : BindableBase
    {
        private readonly IHttpsProxyService httpsProxyService;
        private readonly IDownloadService downloadService;
        /// <summary>
        /// 保存路径
        /// </summary>
        private string savePath = string.Empty;
        private bool isStartEnabled = true;
        private bool isStopEnabled = false;
        private bool isDownloadEnabled = false;
        private readonly Dispatcher uiDispatcher;
        private ObservableCollection<Video> videos = new ObservableCollection<Video>();
        private ObservableCollection<string> message = new ObservableCollection<string>();
        private List<Video> selectedVideos = new List<Video>();

        public bool IsStartEnabled { get => isStartEnabled; set => SetProperty(ref isStartEnabled, value); }
        public bool IsStopEnabled { get => isStopEnabled; set => SetProperty(ref isStopEnabled, value); }
        public bool IsDownloadEnabled { get => isDownloadEnabled; set => SetProperty(ref isDownloadEnabled, value); }

        /// <summary>
        /// 视频列表
        /// </summary>
        public ObservableCollection<Video> Videos
        {
            get { return videos; }
            set { videos = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 信息列表
        /// </summary>
        public ObservableCollection<string> Message
        {
            get { return message; }
            set { message = value; RaisePropertyChanged(); }
        }

        public DelegateCommand StartCommand { get; set; }
        public DelegateCommand StopCommand { get; set; }
        public DelegateCommand DownloadCommand { get; set; }
        public DelegateCommand PathCommand { get; set; }
        public DelegateCommand<SelectionChangedEventArgs> SelectionChangedCommand { get; set; }

        public VideoViewModel(IHttpsProxyService httpsProxyService, IDownloadService downloadService)
        {
            StartCommand = new DelegateCommand(StartExecute);
            StopCommand = new DelegateCommand(StopExecute);
            DownloadCommand = new DelegateCommand(DownloadExecute);
            PathCommand = new DelegateCommand(PathExecute);
            SelectionChangedCommand = new DelegateCommand<SelectionChangedEventArgs>(SelectionChangedExecute);
            uiDispatcher = Dispatcher.CurrentDispatcher;

            this.httpsProxyService = httpsProxyService;
            this.downloadService = downloadService;
        }

        #region 私有执行

        /// <summary>
        /// 开始监听
        /// </summary>
        private void StartExecute()
        {
            Message.Add("开始监听");
            httpsProxyService.Start();
            httpsProxyService.VideoAddedToList += AddVideo;

            IsStartEnabled = false;
            IsStopEnabled = true;
            IsDownloadEnabled = true;
        }

        /// <summary>
        /// 停止监听
        /// </summary>
        private void StopExecute()
        {
            Message.Add("停止监听");
            httpsProxyService.Stop();
            httpsProxyService.VideoAddedToList -= AddVideo;

            IsStartEnabled = true;
            IsStopEnabled = false;
        }

        /// <summary>
        /// 下载视频
        /// </summary>
        private async void DownloadExecute()
        {
            try
            {
                if (selectedVideos.Count == 0)
                {
                    throw new ArgumentNullException("selectedVideos", "请选中下载视频");
                }

                if (string.IsNullOrWhiteSpace(savePath))
                {
                    throw new ArgumentNullException("savePath", "请选中下载路径");
                }

                foreach (var item in selectedVideos)
                {
                    var videoPath = Path.Combine(savePath, $"{item.BriefTitle}.mp4");
                    Message.Add($"开始下载：{item.BriefTitle}.mp4");

                    var result = await downloadService.DownloadVideo(videoPath, item.Url, item.DecryptionArray);

                    Message.Add($"下载成功：{result}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Tips");
            }
        }

        /// <summary>
        /// 选择路径
        /// </summary>
        private void PathExecute()
        {
            OpenFolderDialog folderDialog = new OpenFolderDialog();
            if (folderDialog.ShowDialog() == true)
            {
                savePath = folderDialog.FolderName;
                Message.Add($"保存位置：{savePath}");
            }
        }

        /// <summary>
        /// 多选事件
        /// </summary>
        /// <param name="e"></param>
        private void SelectionChangedExecute(SelectionChangedEventArgs e)
        {
            // 遍历已选中的项
            foreach (var addedItem in e.AddedItems)
            {
                if (addedItem is Video selected)
                {
                    // 如果是新添加的选中项，添加到SelectedList
                    if (!selectedVideos.Contains(selected))
                    {
                        selectedVideos.Add(selected);
                    }
                }
            }

            foreach (var removedItem in e.RemovedItems)
            {
                if (removedItem is Video deselected)
                {
                    // 如果是从选中状态移除的项，从SelectedList中移除
                    selectedVideos.Remove(deselected);
                }
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 新增视频到列表
        /// </summary>
        /// <param name="info"></param>
        private async void AddVideo(VideoDownloadInfo info)
        {
            try
            {


                foreach (var item in Videos)
                {
                    if (string.Equals(item.Key, info.Key))
                    {
                        return;
                    }
                }

                var imageBytes = await downloadService.DownloadImage(info.ThumbUrl);

                await uiDispatcher.InvokeAsync(() =>
                {
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad; // 在加载完成后释放原始流
                    bitmapImage.StreamSource = new MemoryStream(imageBytes);
                    bitmapImage.EndInit();

                    var briefTitle = info.Description.Split('\n').First();

                    var video = new Video
                    {
                        Key = info.Key,
                        ImageData = bitmapImage,
                        BriefTitle = briefTitle,
                        WholeTitle = info.Description,
                        VideoPlayLength = info.VideoPlayLength,
                        Size = info.Size / 1024 / 1024,
                        Uploader = info.Uploader,
                        Url = info.Url,
                        DecryptionArray = info.DecryptionArray
                    };

                    Videos.Add(video);
                });
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Tips");
            }
        }

        #endregion
    }
}
