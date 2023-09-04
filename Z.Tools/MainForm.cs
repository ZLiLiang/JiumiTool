using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Z.Tools.Common;
using Z.Tools.Extensions;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace Z.Tools
{
    public partial class MainForm : Form
    {
        private bool isFile = true;
        private List<string> files = new List<string>();
        private List<string> folders = new List<string>();
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            pathTB.Text = "拖拽文件夹（仅支持单个文件夹）";
            pathTB.ForeColor = Color.Gray;

            rPMenuItem.Checked = true;
            rPMenuItem.BackColor = SystemColors.ControlDark;
            rPLayoutPanel.Visible = true;
        }

        #region TextBox

        /// <summary>
        /// 开始拖拽
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pathTB_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Link;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// 结束拖拽
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pathTB_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                //在此处获取文件路径  或者文件夹的路径
                //string tempStr = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                string[] tempStr = ((string[])e.Data.GetData(DataFormats.FileDrop)); //支持多文件拖拽

                pathTB.Text = string.Join(",", tempStr);
                

                //pathTB.Text = tempStr;
                //FileInfo[] files = FileTools.GetFiles(tempStr);
                //foreach (var file in files)
                //{
                //    msgLB.Items.Add(file.Name);
                //}
                //changeBtn.Enabled = true;
                //pathTB.ForeColor = Color.Black;
                //isFile = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 进入焦点则把提示文字清空
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pathTB_Enter(object sender, EventArgs e)
        {
            if (pathTB.Text == "拖拽文件夹（仅支持单个文件夹）")
            {
                pathTB.Text = "";
                pathTB.ForeColor = Color.Black;
            }
        }

        /// <summary>
        /// 离开焦点显示提示文字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pathTB_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(pathTB.Text))
            {
                pathTB.Text = "拖拽文件夹（仅支持单个文件夹）";
                pathTB.ForeColor = Color.Gray;
            }
        }

        #endregion

        #region Button

        /// <summary>
        /// 选择要处理的文件(支持多选)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectFileBtn_Click(object sender, EventArgs e)
        {
            isFile = true;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true; //设置为多选
            fileDialog.Title = "选择要处理的文件(支持多选)"; //设置标题
            fileDialog.RestoreDirectory = true; //设置

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                pathTB.Text = string.Join(",", fileDialog.FileNames);
                pathTB.ForeColor = Color.Black;
                msgLB.Items.Clear();
                foreach (var item in fileDialog.FileNames)
                {
                    msgLB.Items.Add(item);
                }
                changeBtn.Enabled = true;

            }
        }

        /// <summary>
        /// 选择要处理的文件夹(支持多选)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectFolderBtn_Click(object sender, EventArgs e)
        {
            isFile = false;
            CommonOpenFileDialog folderDialog = new CommonOpenFileDialog();
            folderDialog.IsFolderPicker = true; //设置为选择文件夹
            folderDialog.Multiselect = true; //设置为多选
            folderDialog.Title = "选择要处理的文件夹(支持多选)"; //设置标题
            folderDialog.RestoreDirectory = true; //设置

            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                pathTB.Text = string.Join(",", folderDialog.FileNames);
                pathTB.ForeColor = Color.Black;
                msgLB.Items.Clear();
                foreach (var item in folderDialog.FileNames)
                {
                    msgLB.Items.Add(item);
                }
                changeBtn.Enabled = true;
            }
        }

        /// <summary>
        /// 对文件或文件夹进行修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (isFile == true)
                {
                    ChangeFiles(); //修改文件
                }
                else
                {
                    ChangeFolders(); //修改文件夹
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 清除内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearBtn_Click(object sender, EventArgs e)
        {
            pathTB.Text = "";
            msgLB.Items.Clear();
        }

        #endregion

        #region MenuScript

        /// <summary>
        /// 选择去括号功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rPMenuItem_Click(object sender, EventArgs e)
        {
            noneMenuItem.Checked = false;
            noneMenuItem.BackColor = SystemColors.Control;

            rPMenuItem.Checked = true;
            rPMenuItem.BackColor = SystemColors.ControlDark;
            rPLayoutPanel.Visible = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void noneMenuItem_Click(object sender, EventArgs e)
        {
            rPMenuItem.Checked = false;
            rPMenuItem.BackColor = SystemColors.Control;
            rPLayoutPanel.Visible = false;

            noneMenuItem.Checked = true;
            noneMenuItem.BackColor = SystemColors.ControlDark;
        }

        #endregion

        #region MyPrivate

        /// <summary>
        /// 修改文件名称
        /// </summary>
        private void ChangeFiles()
        {
            msgLB.Items.Clear();
            FileInfo[] files = FileTools.GetFiles(pathTB.Text.GetPath()); //根据路径获取路径下的文件
            if (isBackUp.CheckState == CheckState.Checked)
            {
                string backUpPath = FileTools.CreateBackUp(files);
                msgLB.Items.Add($"备份完成：{backUpPath}");
            }
            changeBtn.Enabled = false;
            isBackUp.Checked = false;
            List<string> list = FileTools.ChangeFileName(files); //修改文件名称，并返回结果
            foreach (var item in list)
            {
                msgLB.Items.Add(item);
            }
        }

        /// <summary>
        /// 修改文件夹名称
        /// </summary>
        private void ChangeFolders()
        {
            msgLB.Items.Clear();
            DirectoryInfo[] folders = FileTools.GetFolders(pathTB.Text.GetPath()); //根据路径获取路径下的文件夹
            if (isBackUp.CheckState == CheckState.Checked)
            {
                string backUpPath = FileTools.CreateBackUp(folders);
                msgLB.Items.Add($"备份完成：{backUpPath}");
            }
            changeBtn.Enabled = false;
            isBackUp.Checked = false;
            List<string> list = FileTools.ChangeFileName(folders); //修改文件夹名称，并返回结果
            foreach (var item in list)
            {
                msgLB.Items.Add(item);
            }
        }

        #endregion
    }
}
