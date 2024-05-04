using System.Collections.ObjectModel;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JiumiTool2.Constants;
using JiumiTool2.Extensions;

namespace JiumiTool2.ViewModels
{
    public partial class FileConfigDialogModel : ObservableObject
    {
        /// <summary>
        /// 用于存储起始项目的索引
        /// </summary>
        private int startIndex, endIndex;

        /// <summary>
        /// 用于标记是否正在选择范围
        /// </summary>
        private bool isSelectingRange = false;

        [ObservableProperty]
        private string _fileName = string.Empty;

        [ObservableProperty]
        private string _selectedOption = FileModifySeat.Prefix.GetDescription();

        public List<char> FileNameChars = new List<char>();

        [RelayCommand]
        private void SelectionChanged(SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && !isSelectingRange)
            {
                startIndex=
            }
            else if (e.AddedItems.Count > 0 && isSelectingRange)
            {

            }


            // 遍历已选中的项
            foreach (char item in e.AddedItems)
            {
                if (!FileNameChars.Contains(item))
                    FileNameChars.Add(item);
            }

            // 遍历已移除的项
            foreach (char item in e.RemovedItems)
            {
                if (FileNameChars.Contains(item))
                    FileNameChars.Remove(item);
            }
        }
    }
}
