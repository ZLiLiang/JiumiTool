using System.IO;
using JiumiTool2.IServices;
using Microsoft.Win32;
using Wpf.Ui.Controls;

namespace JiumiTool2.Views
{
    /// <summary>
    /// VideoConfigDialog.xaml 的交互逻辑
    /// </summary>
    public partial class VideoConfigDialog : ContentDialog
    {
        private readonly IAppsettingsService _appsettingsService;

        public VideoConfigDialog(IAppsettingsService appsettingsService)
        {
            InitializeComponent();

            _appsettingsService = appsettingsService;
            PathTextBox.Text = appsettingsService.GetAppsettings().VideoOptions.DownloadPath;
        }

        protected override async void OnButtonClick(ContentDialogButton button)
        {
            if (button == ContentDialogButton.Primary && Directory.Exists(PathTextBox.Text) == false)
            {
                MessageBox messageBox = new MessageBox
                {
                    Title = "错误",
                    Content = "路径不存在！",
                    CloseButtonText = "确认"
                };
                await messageBox.ShowDialogAsync();
                return;
            }
            else if (button == ContentDialogButton.Primary)
            {
                await _appsettingsService.UpdateAppsettingsAsync(action =>
                {
                    action.VideoOptions.DownloadPath = PathTextBox.Text;
                });
            }
            base.OnButtonClick(button);
        }

        /// <summary>
        /// 选择下载路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectDownloadPath(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFolderDialog folderDialog = new OpenFolderDialog();
            folderDialog.Title = "选择下载路径";

            if (folderDialog.ShowDialog() == true)
            {
                PathTextBox.Text = folderDialog.FolderName;
            }
        }
    }
}
