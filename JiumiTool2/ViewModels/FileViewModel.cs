using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JiumiTool2.Constants;
using JiumiTool2.Extensions;
using JiumiTool2.IServices;
using JiumiTool2.Views;
using Microsoft.Win32;
using Wpf.Ui;

namespace JiumiTool2.ViewModels
{
    public partial class FileViewModel : ObservableObject, IDisposable
    {
        private IContentDialogService _contentDialogService;
        private FileConfigDialog _fileConfigDialog;
        private IAppsettingsService _appsettingsService;

        public FileViewModel(IContentDialogService contentDialogService,
                             FileConfigDialog fileConfigDialog,
                             IAppsettingsService appsettingsService)
        {
            _contentDialogService = contentDialogService;
            _fileConfigDialog = fileConfigDialog;
            _appsettingsService = appsettingsService;

            InitialMessage();

            _appsettingsService.ChangeToNotification(() => { });

            WeakReferenceMessenger.Default.Register<FileViewModel, string, string>(this, "FileViewModel", (instance, message) =>
            {
                if (message.Equals("UpdateMessage"))
                {
                    instance.UpdateMessage();
                }
            });
        }

        #region 响应式私有字段

        /// <summary>
        /// 修改路径
        /// </summary>
        [ObservableProperty]
        private string _modifyPath = string.Empty;

        /// <summary>
        /// 选择模式
        /// </summary>
        [ObservableProperty]
        private int _selectedMode = 0;

        /// <summary>
        /// 启用修改按钮
        /// </summary>
        [ObservableProperty]
        private bool _modifyEnable = false;

        /// <summary>
        /// 启用清除按钮
        /// </summary>
        [ObservableProperty]
        private bool _clearEnable = false;

        /// <summary>
        /// 备份确认
        /// </summary>
        [ObservableProperty]
        private bool _isBackup = false;

        /// <summary>
        /// 信息列表
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<string> _messageItems = [];

        /// <summary>
        /// 模式列表
        /// </summary>
        [ObservableProperty]
        private List<string> _modes = EnumExtension.ToDescriptionList<FileModifyMode>();

        #endregion

        /// <summary>
        /// 使listbox跟随窗体大小变化
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
            ListBox messageListBox = currentPage.FindName("MessageListBox") as ListBox;
            Border messageBorder = currentPage.FindName("MessageBorder") as Border;

            messageListBox.Height = 0;
            messageBorder.UpdateLayout();
            var actualHeight = messageBorder.ActualHeight;
            messageListBox.Height = actualHeight - 2;
        }

        /// <summary>
        /// 选择文件(夹)路径
        /// </summary>
        [RelayCommand]
        private void SelectedPath()
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            openFolderDialog.Multiselect = false;
            openFolderDialog.Title = (FileModifyMode)SelectedMode == FileModifyMode.File ? "选择文件" : "选择文件夹";
            if (openFolderDialog.ShowDialog() == true)
            {
                ModifyPath = openFolderDialog.FolderName;
                if ((FileModifyMode)SelectedMode == FileModifyMode.File)
                {
                    LoadFileName();
                }
                else
                {
                    LoadFolderName();
                }
                ModifyEnable = true;
                ClearEnable = true;
            }
        }

        [RelayCommand]
        private void ExecuteModified()
        {

        }

        /// <summary>
        /// 清理记录
        /// </summary>
        [RelayCommand]
        private void ExecuteCleared()
        {
            MessageItems.Clear();
            InitialMessage();

            ModifyEnable = false;
            ClearEnable = false;
        }

        /// <summary>
        /// 弹出配置窗口
        /// </summary>
        [RelayCommand]
        private async void ModifySetting()
        {
            var title = (FileModifyMode)SelectedMode switch
            {
                FileModifyMode.File => FileModifyMode.File.GetDescription(),
                FileModifyMode.Folder => FileModifyMode.Folder.GetDescription(),
                _ => ""
            };

            _fileConfigDialog.Title = $"{title}配置";
            _fileConfigDialog.PrimaryButtonText = "确认";
            _fileConfigDialog.CloseButtonText = "取消";

            await _contentDialogService.ShowAsync(_fileConfigDialog, default);
        }

        /// <summary>
        /// 更新信息表
        /// </summary>
        private void InitialMessage()
        {
            var pattern = _appsettingsService.GetAppsettings().FileOptions.Pattern;
            var seat = _appsettingsService.GetAppsettings().FileOptions.Seat.ToEnum<FileModifySeat>().GetDescription();

            MessageItems.Add($"匹配模式:\"{pattern}\"");
            MessageItems.Add($"匹配位置:\"{seat}\"");
        }

        /// <summary>
        /// 更新信息
        /// </summary>
        private void UpdateMessage()
        {
            Task.Delay(500).Wait();

            var pattern = _appsettingsService.GetAppsettings().FileOptions.Pattern;
            var seat = _appsettingsService.GetAppsettings().FileOptions.Seat.ToEnum<FileModifySeat>().GetDescription();

            MessageItems.Add("============ Update Completed! ============");
            MessageItems.Add($"匹配模式:\"{pattern}\"");
            MessageItems.Add($"匹配位置:\"{seat}\"");
        }

        /// <summary>
        /// 加载文件名
        /// </summary>
        private void LoadFileName()
        {
            var files = Directory.GetFiles(ModifyPath);

            MessageItems.Add("============ Loaded FileName! ============");
            foreach (var item in files)
            {
                MessageItems.Add(item.Split('\\').Last());
            }
            MessageItems.Add(" ");
        }

        /// <summary>
        /// 加载文件夹名
        /// </summary>
        private void LoadFolderName()
        {
            var folders = Directory.GetDirectories(ModifyPath);

            MessageItems.Add("============ Loaded FolderName! ============");
            foreach (var item in folders)
            {
                MessageItems.Add(item.Split('\\').Last());
            }
            MessageItems.Add(" ");
        }

        public void Dispose()
        {
            WeakReferenceMessenger.Default.Unregister<string, string>(this, "FileViewModel");
        }
    }
}
