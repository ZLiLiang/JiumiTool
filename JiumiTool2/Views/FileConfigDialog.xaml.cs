using JiumiTool2.Constants;
using JiumiTool2.Extensions;
using JiumiTool2.IServices;
using JiumiTool2.ViewModels;
using Wpf.Ui.Controls;

namespace JiumiTool2.Views
{
    /// <summary>
    /// FileConfigDialog.xaml 的交互逻辑
    /// </summary>
    public partial class FileConfigDialog : ContentDialog
    {
        private FileConfigDialogModel _viewModel;
        private IAppsettingsService _appsettingsService;

        public FileConfigDialog(FileConfigDialogModel fileConfigDialogModel, IAppsettingsService appsettingsService)
        {
            _viewModel = fileConfigDialogModel;
            _appsettingsService = appsettingsService;

            DataContext = _viewModel;
            InitializeComponent();
        }

        protected override void OnButtonClick(ContentDialogButton button)
        {
            if (_viewModel.FileNameChars.Count == 0 && button == ContentDialogButton.Primary)
            {
                var messageBox = new MessageBox
                {
                    Title = "提示",
                    Content = "不能为空！            ",
                    CloseButtonText = "确认"
                };
                messageBox.ShowDialogAsync();
                return;
            }

            _viewModel.FileName = string.Empty;

            base.OnButtonClick(button);
        }

        //private string GetMatchString(List<char> chars, FileModifySeat modifySeat)
        //{

        //}
    }
}
