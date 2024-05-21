using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using JiumiTool2.Constants;
using JiumiTool2.IServices;
using Wpf.Ui.Controls;

namespace JiumiTool2.Views
{
    /// <summary>
    /// FileConfigDialog.xaml 的交互逻辑
    /// </summary>
    public partial class FileConfigDialog : ContentDialog
    {
        private IAppsettingsService _appsettingsService;

        /// <summary>
        /// 用于存储起始项目的索引
        /// </summary>
        private int _startIndex, _endIndex;

        /// <summary>
        /// 用于标记是否正在选择范围
        /// </summary>
        private bool _isSelectingRange = false;

        /// <summary>
        /// 标记是否在处理选项
        /// </summary>
        private bool _isHandlingSelectionChange = false;

        /// <summary>
        /// 匹配字符数组
        /// </summary>
        private List<char> _matchArray = new();

        public FileConfigDialog(IAppsettingsService appsettingsService)
        {
            _appsettingsService = appsettingsService;

            InitializeComponent();
        }

        private void OnListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isHandlingSelectionChange == true)
                return;

            _isHandlingSelectionChange = true;

            if (e.AddedItems.Count >= 0 && _isSelectingRange == false)
            {
                _isSelectingRange = true;
                _startIndex = listBox.SelectedIndex;
                // 清理选中列表
                listBox.SelectedItems.Clear();

                // 设置选中项
                var selectedItem = listBox.ItemContainerGenerator.ContainerFromIndex(_startIndex) as ListBoxItem;
                selectedItem.IsSelected = true;
            }
            else if (e.AddedItems.Count > 0 && _isSelectingRange == true)
            {
                _isSelectingRange = false;
                _endIndex = listBox.SelectedIndex;
                _matchArray.Clear();

                int start = 0, end = 0;
                if (_startIndex < _endIndex)
                {
                    start = _startIndex;
                    end = _endIndex;
                }
                else
                {
                    start = _endIndex;
                    end = _startIndex;
                }

                for (int i = start; i <= end; i++)
                {
                    // 设置选中项
                    var selectedItem = listBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                    selectedItem.IsSelected = true;
                    _matchArray.Add((char)selectedItem.Content);
                }
            }

            _isHandlingSelectionChange = false;

            if (e.RemovedItems.Count > 0)
            {

            }
        }

        protected override async void OnButtonClick(ContentDialogButton button)
        {
            if (button == ContentDialogButton.Primary && _matchArray.Count == 0)
            {
                var messageBox = new MessageBox
                {
                    Title = "提示",
                    Content = "请选择匹配范围！",
                    CloseButtonText = "确认"
                };
                messageBox.ShowDialogAsync();
                return;
            }

            // 模板
            string pattern = string.Join(string.Empty, _matchArray);
            // 位置
            string seat = FileModifySeat.Prefix.ToString();
            if (prefixMatch.IsChecked == true)
            {
                seat = FileModifySeat.Prefix.ToString();
            }
            else
            {
                seat = FileModifySeat.Suffix.ToString();
            }

            WeakReferenceMessenger.Default.Send("UpdateMessage", "FileViewModel");
            await _appsettingsService.UpdateAppsettingsAsync(action =>
            {
                action.FileOptions.Pattern = pattern;
                action.FileOptions.Seat = seat;
            });

            base.OnButtonClick(button);
        }
    }
}
