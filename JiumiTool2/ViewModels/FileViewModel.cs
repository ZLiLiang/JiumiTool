using System.Collections.ObjectModel;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JiumiTool2.Constants;
using JiumiTool2.Extensions;
using JiumiTool2.Views;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace JiumiTool2.ViewModels
{
    public partial class FileViewModel : ObservableObject
    {
        private IContentDialogService _contentDialogService;
        private FileConfigDialog _fileConfigDialog;

        public FileViewModel(IContentDialogService contentDialogService, FileConfigDialog fileConfigDialog)
        {
            _contentDialogService = contentDialogService;
            _fileConfigDialog = fileConfigDialog;
        }

        [ObservableProperty]
        private string _modifyPath = string.Empty;

        [ObservableProperty]
        private int _selectedMode = 0;

        [ObservableProperty]
        private bool _isBackup = false;

        [ObservableProperty]
        private ObservableCollection<string> _messageItems = [];

        [ObservableProperty]
        private List<string> _modes = EnumExtension.ToDescriptionList<ModifyMode>();

        [RelayCommand]
        private void PageSizeChanged(object sender)
        {
            // 确保sender是Page类型，尽管在本方法中sender通常就是当前Page实例
            if (sender is not Page currentPage)
            {
                return;
            }

            // 使用FindName方法查找子元素
            ListBox messageListBox = currentPage.FindName("MessageListBox") as ListBox;
            Border messageBorder = currentPage.FindName("MessageBorder") as Border;

            messageListBox.Height = 0;
            messageBorder.UpdateLayout();
            var actualHeight = messageBorder.ActualHeight;
            messageListBox.Height = actualHeight - 2;
        }

        [RelayCommand]
        private void SelectedPath()
        {

        }

        [RelayCommand]
        private void ExecuteModified()
        {

        }

        [RelayCommand]
        private async void ModifySetting()
        {
            var title = (ModifyMode)SelectedMode switch
            {
                ModifyMode.File => ModifyMode.File.GetDescription(),
                ModifyMode.Folder => ModifyMode.Folder.GetDescription(),
                _ => ""
            };

            _fileConfigDialog.Title = $"{title}配置";
            _fileConfigDialog.PrimaryButtonText = "确认";
            _fileConfigDialog.CloseButtonText = "取消";

            await _contentDialogService.ShowAsync(_fileConfigDialog, default);
        }
    }
}
