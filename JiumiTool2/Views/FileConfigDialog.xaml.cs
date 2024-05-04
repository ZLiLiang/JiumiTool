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
        private int startIndex, endIndex;

        /// <summary>
        /// 用于标记是否正在选择范围
        /// </summary>
        private bool isSelectingRange = false;

        public FileConfigDialog( IAppsettingsService appsettingsService)
        {
            _appsettingsService = appsettingsService;

            InitializeComponent();
        }

        private void OnListBoxSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        protected override void OnButtonClick(ContentDialogButton button)
        {
            if (button == ContentDialogButton.Primary)
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


            base.OnButtonClick(button);
        }

        //private string GetMatchString(List<char> chars, FileModifySeat modifySeat)
        //{

        //}
    }
}
