using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using Z.JuimiTool.Extensions;
using Z.JuimiTool.Constants;

namespace Z.JuimiTool.ViewModels
{
    public class FileViewModel : BindableBase
    {
        private int selectedMode = 0;
        private string filePath = string.Empty;
        private ObservableCollection<string> message = new ObservableCollection<string>();

        /// <summary>
        /// 文件/文件夹模式
        /// </summary>
        public List<string> Modes { get; set; } = EnumExtensions.ToList<ModifyMode>();
        /// <summary>
        /// 选择模式
        /// </summary>
        public int SelectedMode { get => selectedMode; set => SetProperty(ref selectedMode, value); }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get => filePath; set => SetProperty(ref filePath, value); }
        /// <summary>
        /// 信息列表
        /// </summary>
        public ObservableCollection<string> Message
        {
            get { return message; }
            set { message = value; RaisePropertyChanged(); }
        }

        public DelegateCommand SelectCommand { get; private set; }
        public DelegateCommand ModifyCommand { get; private set; }
        public DelegateCommand ClearCommand { get; private set; }

        public FileViewModel()
        {
            SelectCommand = new DelegateCommand(SelectExecute);
            ModifyCommand = new DelegateCommand(ModifyExecute);
            ClearCommand = new DelegateCommand(ClearExecute);
        }

        #region 私有执行

        /// <summary>
        /// 选择文件/文件夹
        /// </summary>
        private void SelectExecute()
        {
            
        }

        /// <summary>
        /// 修改文件/文件夹
        /// </summary>
        private void ModifyExecute()
        {

        }

        /// <summary>
        /// 清除listbox与textblock的内容
        /// </summary>
        private void ClearExecute()
        {
            FilePath = string.Empty;
            Message.Clear();
        }

        #endregion
    }
}
