using System.IO;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using JiumiTool2.IServices;
using JiumiTool2.Models;
using Wpf.Ui.Controls;

namespace JiumiTool2.Views
{
    /// <summary>
    /// VideoDownloadDialog.xaml 的交互逻辑
    /// </summary>
    public partial class VideoDownloadDialog : ContentDialog
    {
        private readonly IDownloadService _downloadService;
        private readonly IAppsettingsService _appsettingsService;
        private List<Video> DownloadVideos = new List<Video>();

        public VideoDownloadDialog(IDownloadService downloadService, IAppsettingsService appsettingsService)
        {
            _downloadService = downloadService;
            _appsettingsService = appsettingsService;

            InitializeComponent();

            WeakReferenceMessenger.Default.Register<VideoDownloadDialog, IList<Video>, string>(this, "DownloadVideo", MessageHandler);
        }

        protected override void OnButtonClick(ContentDialogButton button)
        {
            base.OnButtonClick(button);
        }

        /// <summary>
        /// 信息处理
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="messages"></param>
        private void MessageHandler(VideoDownloadDialog recipient, IList<Video> messages)
        {
            recipient.DownloadVideos.Clear();
            MainGrid.RowDefinitions.Clear();
            MainGrid.Children.Clear();
            foreach (var item in messages)
            {
                recipient.DownloadVideos.Add(item);
            }

            for (int i = 0; i < recipient.DownloadVideos.Count; i++)
            {
                MainGrid.RowDefinitions.Add(new RowDefinition());
                var subGrid = CreateDownloadComponent(messages[i]);
                MainGrid.Children.Add(subGrid);
                Grid.SetRow(subGrid, i);
            }
        }

        private Grid CreateDownloadComponent(Video video)
        {
            Grid grid = new Grid();
            grid.Margin = new Thickness(3);
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());

            Wpf.Ui.Controls.TextBlock textBlock = new Wpf.Ui.Controls.TextBlock();
            textBlock.TextTrimming = TextTrimming.CharacterEllipsis;
            textBlock.Text = video.Description;
            textBlock.ToolTip = video.Description;
            ProgressBar progressBar = new ProgressBar();
            Progress<double> progress = new Progress<double>(x => progressBar.Value = x);

            grid.Children.Add(textBlock);
            grid.Children.Add(progressBar);
            Grid.SetRow(textBlock, 0);
            Grid.SetRow(progressBar, 1);

            var outputPath = $"{Path.Combine(_appsettingsService.GetAppsettings().VideoOptions.DownloadPath, video.Description)}.mp4";
            _downloadService.DownloadVideo(video.Url, outputPath, video.DecryptionArray, progress);

            return grid;
        }
    }
}
