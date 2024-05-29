using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using Wpf.Ui.Controls;

namespace JiumiTool2.ViewModels
{
    public partial class FileViewModel : ObservableObject, IDisposable
    {
        private readonly IContentDialogService _contentDialogService;
        private readonly FileConfigDialog _fileConfigDialog;
        private readonly IAppsettingsService _appsettingsService;
        private readonly IMatchRegexService _matchRegexService;
        private readonly IFileService _fileService;
        private readonly IFolderService _folderService;

        public FileViewModel(IContentDialogService contentDialogService,
                             FileConfigDialog fileConfigDialog,
                             IAppsettingsService appsettingsService,
                             IMatchRegexService matchRegexService,
                             IFileService fileService,
                             IFolderService folderService)
        {
            _contentDialogService = contentDialogService;
            _fileConfigDialog = fileConfigDialog;
            _appsettingsService = appsettingsService;
            _matchRegexService = matchRegexService;
            _fileService = fileService;
            _folderService = folderService;

            InitialMessage();

            WeakReferenceMessenger.Default.Register<FileViewModel, string, string>(this, "FileViewModel", (instance, message) =>
            {
                var pattern = message.Split(',').First();
                var seat = message.Split(',').Last().ToEnum<FileModifySeat>().GetDescription();

                instance.UpdateMessage(pattern, seat);
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
        private async Task ExecuteModifiedAsync()
        {
            try
            {
                var chars = _appsettingsService.GetAppsettings().FileOptions.Pattern.ToList();
                var seat = _appsettingsService.GetAppsettings().FileOptions.Seat.ToEnum<FileModifySeat>();
                // 构建匹配模板
                var pattern = _matchRegexService.GeneratePattern(chars);
                // 构建正则对象
                var regex = _matchRegexService.GenerateRegex(pattern, seat);

                // 添加信息
                void AddRange(List<string> result)
                {
                    foreach (var item in result)
                    {
                        MessageItems.Add(item);
                    }
                }

                if ((FileModifyMode)SelectedMode == FileModifyMode.File)
                {
                    var filePaths = _fileService.GetFiles(ModifyPath);
                    if (IsBackup == true)
                    {
                        _fileService.Backup(filePaths);
                    }
                    var result = _fileService.RepeatName(filePaths, regex);
                    AddRange(result);
                }
                else
                {
                    var folderPaths = _folderService.GetFolders(ModifyPath);
                    if (IsBackup == true)
                    {
                        _folderService.Backup(folderPaths);
                    }
                    var result = _folderService.RepeatName(folderPaths, regex);
                    AddRange(result);
                }
            }
            catch (Exception ex)
            {
                var messageBox = new MessageBox
                {
                    Title = "提示",
                    Content = $"出现错误{ex.Message}",
                    CloseButtonText = "确认"
                };
                await messageBox.ShowDialogAsync();
            }
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

        #region 私有方法

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
        private void UpdateMessage(string pattern, string seat)
        {
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

        #endregion

        public void Dispose()
        {
            WeakReferenceMessenger.Default.Unregister<string, string>(this, "FileViewModel");
        }
    }
}
