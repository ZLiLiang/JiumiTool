using System.Collections.ObjectModel;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wpf.Ui.Controls;

namespace JiumiTool2.ViewModels
{
    public partial class FileViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<string> _messageItems = [];

        public FileViewModel()
        {
            MessageItems.Add("test");
            MessageItems.Add("test");
            MessageItems.Add("test");
            MessageItems.Add("test");
            MessageItems.Add("test");
            MessageItems.Add("test");
            MessageItems.Add("test");
            MessageItems.Add("test");
            MessageItems.Add("test");
            MessageItems.Add("test");
            MessageItems.Add("test");
            MessageItems.Add("test");
            MessageItems.Add("test");
            MessageItems.Add("test");
            MessageItems.Add("test");
            MessageItems.Add("test");
            MessageItems.Add("test");
            MessageItems.Add("test");
            MessageItems.Add("test");
            MessageItems.Add("test");
        }

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
    }
}
