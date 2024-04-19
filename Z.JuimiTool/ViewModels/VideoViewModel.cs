using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;
using Prism.Commands;
using Prism.Mvvm;
using Z.JuimiTool.IServices;
using Z.JuimiTool.Models;

namespace Z.JuimiTool.ViewModels
{
    public class VideoViewModel : BindableBase
    {
        private readonly IHttpsProxyService httpsProxyService;
        private readonly IDownloadService downloadService;
        private ObservableCollection<Video> videos = new ObservableCollection<Video>();
        private ObservableCollection<string> message = new ObservableCollection<string>();

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

        public VideoViewModel(IHttpsProxyService httpsProxyService, IDownloadService downloadService)
        {
            StartCommand = new DelegateCommand(StartExecute);
            StopCommand = new DelegateCommand(StopExecute);
            DownloadCommand = new DelegateCommand(DownloadExecute);
            PathCommand = new DelegateCommand(PathExecute);

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
            //httpsProxyService.Start();
            //httpsProxyService.VideoAddedToList += AddVideo;
        }

        /// <summary>
        /// 停止监听
        /// </summary>
        private void StopExecute()
        {
            Message.Add("停止监听");
            httpsProxyService.Stop();
            httpsProxyService.VideoAddedToList -= AddVideo;
        }

        /// <summary>
        /// 下载视频
        /// </summary>
        private void DownloadExecute()
        {

        }

        /// <summary>
        /// 选择路径
        /// </summary>
        private void PathExecute()
        {

        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 新增视频到列表
        /// </summary>
        /// <param name="info"></param>
        private async void AddVideo(VideoDownloadInfo info)
        {
            var imageBytes = await downloadService.DownloadImage(info.ThumbUrl);

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad; // 在加载完成后释放原始流
            bitmapImage.StreamSource = new MemoryStream(imageBytes);
            bitmapImage.EndInit();


            videos.Add(new Video
            {
                ImageData = bitmapImage,
                BriefTitle = info.Description,
                WholeTitle = info.Description,
                VideoPlayLength = info.VideoPlayLength,
                Size = info.Size / 1024
            });
        }

        #endregion
    }
}
