using System.IO;
using JiumiTool2.IServices;
using Microsoft.Win32;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace JiumiTool2.Views
{
    /// <summary>
    /// VideoConfigDialog.xaml 的交互逻辑
    /// </summary>
    public partial class VideoConfigDialog : ContentDialog
    {
        private readonly IAppsettingsService _appsettingsService;
        private readonly ISnackbarService _snackbarService;

        public VideoConfigDialog(IAppsettingsService appsettingsService, ISnackbarService snackbarService)
        {
            InitializeComponent();

            _appsettingsService = appsettingsService;
            _snackbarService = snackbarService;
        }

        protected override async void OnButtonClick(ContentDialogButton button)
        {
            if (Directory.Exists(PathTextBox.Text) == false)
            {
                _snackbarService.Show(
                    "错误",
                    "路径不存在！",
                    ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.Fluent24),
                    TimeSpan.FromSeconds(1.5)
                    );
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
