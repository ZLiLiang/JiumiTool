using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using Z.JuimiTool.Extensions;
using Z.JuimiTool.Constants;
using Microsoft.Win32;
using Z.JuimiTool.Models;
using Z.JuimiTool.IServices;
using System.Linq;
using System.Windows;
using System;

namespace Z.JuimiTool.ViewModels
{
    public class FileViewModel : BindableBase
    {
        private IFileService fileService;
        private IFolderService folderService;
        private int selectedMode = 0;
        private string filePath = string.Empty;
        private bool isBackup = false;
        private bool isModify = false;
        private ObservableCollection<string> message = new ObservableCollection<string>();
        /// <summary>
        /// 待修改队列
        /// </summary>
        private Queue<Resource> modifyResources { get; set; } = new Queue<Resource>();
        /// <summary>
        /// 唯一资源字典
        /// </summary>
        private Dictionary<string, Resource> uniqueResources = new Dictionary<string, Resource>();
        /// <summary>
        /// 唯一路径
        /// </summary>
        private HashSet<string> uniquePath = new HashSet<string>();

        /// <summary>
        /// 文件/文件夹模式
        /// </summary>
        public List<string> Modes { get; set; } = EnumExtension.ToList<ModifyMode>();
        /// <summary>
        /// 选择模式
        /// </summary>
        public int SelectedMode { get => selectedMode; set => SetProperty(ref selectedMode, value); }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get => filePath; set => SetProperty(ref filePath, value); }
        /// <summary>
        /// 是否备份
        /// </summary>
        public bool IsBackup { get => isBackup; set => SetProperty(ref isBackup, value); }
        /// <summary>
        /// 是否修改
        /// </summary>
        public bool IsModify { get => isModify; set => SetProperty(ref isModify, value); }

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
        public DelegateCommand<DragEventArgs> DragDropCommand { get; private set; }
        public DelegateCommand<DragEventArgs> PreviewDragOverCommand { get; private set; }

        public FileViewModel(IFileService fileService, IFolderService folderService)
        {
            SelectCommand = new DelegateCommand(SelectExecute);
            ModifyCommand = new DelegateCommand(ModifyExecute);
            ClearCommand = new DelegateCommand(ClearExecute);
            DragDropCommand = new DelegateCommand<DragEventArgs>(DragDropExecute);
            PreviewDragOverCommand = new DelegateCommand<DragEventArgs>(PreviewDragOverExecute);
            this.fileService = fileService;
            this.folderService = folderService;
        }

        #region 私有执行

        /// <summary>
        /// 选择文件/文件夹
        /// </summary>
        private void SelectExecute()
        {
            ClearExecute();
            OpenFolderDialog dialog = new OpenFolderDialog();
            dialog.Title = "选择要处理的文件夹";

            if (dialog.ShowDialog() == true)
            {
                FilePath = dialog.FolderName;
                var result = new List<Resource>();
                if (SelectedMode == (int)ModifyMode.File)
                {
                    //获取目录下的所有文件
                    result = fileService.GetFiles(dialog.FolderName)
                        .Cast<Resource>()
                        .ToList();
                }
                else
                {
                    //获取目录下的所有文件夹
                    result = folderService.GetFolders(dialog.FolderName)
                        .Cast<Resource>()
                        .ToList();
                }
                foreach (var item in result)
                {
                    Message.Add(item.Name);
                    modifyResources.Enqueue(item);
                }
                IsModify = true;
            }
        }

        /// <summary>
        /// 修改文件/文件夹
        /// </summary>
        private void ModifyExecute()
        {
            try
            {
                List<string> result;
                //文件模式
                if (SelectedMode == (int)ModifyMode.File)
                {
                    var resources = modifyResources.DequeueToList();
                    if (IsBackup == true)
                    {
                        var backupPath = fileService.Backup(resources);
                        Message.Add($"备份完成：{backupPath}");
                    }
                    //对文件重命名
                    result = fileService.RepeatName(resources);
                }
                //文件夹模式
                else
                {
                    var resources = modifyResources.DequeueToList();
                    if (IsBackup == true)
                    {
                        var backupPath = folderService.Backup(resources);
                        Message.Add($"备份完成：{backupPath}");
                    }
                    //对文件夹重命名
                    result = folderService.RepeatName(resources);
                }

                foreach (var item in result)
                {
                    Message.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Message.Add($"\n-------------------------------\n" +
                    $"Failed to modify {ex.Message}" +
                    $"\n-------------------------------\n");
            }
            finally
            {
                modifyResources.Clear();
                uniqueResources.Clear();
                uniquePath.Clear();
                IsModify = false;
                isBackup = false;
            }
        }

        /// <summary>
        /// 清除listbox、textblock和队列的内容
        /// </summary>
        private void ClearExecute()
        {
            FilePath = string.Empty;
            Message.Clear();
            modifyResources.Clear();
            uniqueResources.Clear();
            uniquePath.Clear();
            IsModify = false;
        }

        /// <summary>
        /// textbox拖拽文件
        /// </summary>
        /// <param name="e"></param>
        private void DragDropExecute(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var item in files)
                {
                    uniquePath.Add(item);
                }
                files = uniquePath.ToArray();
                FilePath = string.Join(",", files);


                var result = new List<Resource>();
                if (SelectedMode == (int)ModifyMode.File)
                {
                    //获取目录下的所有文件
                    result = fileService.GetFiles(files)
                        .Cast<Resource>()
                        .ToList();
                }
                else
                {
                    result = folderService.GetFolders(files)
                            .Cast<Resource>()
                            .ToList();
                }

                foreach (var item in result)
                {
                    uniqueResources.TryAdd(item.FullName, item);
                }
                Message.Clear();
                modifyResources.Clear();
                foreach (var item in uniqueResources.Values)
                {
                    Message.Add(item.Name);
                    modifyResources.Enqueue(item);
                }

                IsModify = true;
            }
        }

        /// <summary>
        /// 允许进行拖拽
        /// </summary>
        /// <param name="e"></param>
        private void PreviewDragOverExecute(DragEventArgs e)
        {
            e.Handled = true;
        }

        #endregion
    }
}
