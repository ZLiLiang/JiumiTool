using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Z.Tools.Common;
using Z.Tools.Extensions;
using Z.Tools.Modle;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace Z.Tools
{
    public partial class MainForm : Form
    {
        FileTools fileTools = new FileTools();
        FolderTools folderTools = new FolderTools();
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            pathTB.Text = "拖拽文件(夹)";
            pathTB.ForeColor = Color.Gray;

            rPMenuItem.Checked = true;
            rPMenuItem.BackColor = SystemColors.ControlDark;
            rPLayoutPanel.Visible = true;

            msgLB.Items.Add("想修改文件夹时，别忘记调为文件夹模式哦。");
            msgLB.Items.Add("选择按钮只能选择想修改的文件（夹）的父目录。");
            
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
                string[] inputStr = ((string[])e.Data.GetData(DataFormats.FileDrop)); //支持多文件拖拽

                pathTB.Text = string.Join(",", inputStr);
                List<Resource> resources = null;
                if (folderMode.CheckState == CheckState.Checked) //文件夹模式
                {
                    if (!Directory.Exists(inputStr.First()))
                    {
                        MessageBox.Show("文件夹模式请输入文件夹");
                        return;
                    }
                    if (inputStr.Length > 1)
                    {
                        resources = folderTools.GetFolders(inputStr); //输入多个绝对路径文件夹，则获取这些文件夹
                    }
                    else
                    {
                        resources = folderTools.GetFolders(inputStr.First()); //输入一个目录绝对路径，则获取目录下的文件夹
                    }
                }
                else //文件模式
                {
                    if (inputStr.Length > 1)
                    {
                        resources = fileTools.GetFiles(inputStr); //输入多个绝对路径文件，则获取这些文件
                    }
                    else
                    {
                        resources = fileTools.GetFiles(inputStr.First()); //输入一个目录绝对路径，则获取目录下的文件
                    }
                }
                foreach (var resource in resources)
                {
                    msgLB.Items.Add(resource.FullName);
                }
                changeBtn.Enabled = true;
                pathTB.ForeColor = Color.Black;
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
            if (pathTB.Text == "拖拽文件(夹)")
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
                pathTB.Text = "拖拽文件(夹)";
                pathTB.ForeColor = Color.Gray;
            }
        }

        #endregion

        #region Button

        /// <summary>
        /// 选择要处理的文件夹(支持多选)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectResourceBtn_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.Title = "选择要处理的文件(夹)"; //设置标题
            dialog.IsFolderPicker = true; //设置为选择文件夹
            dialog.RestoreDirectory = true; //设置

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                pathTB.Text = string.Join(",", dialog.FileNames);
                pathTB.ForeColor = Color.Black;
                msgLB.Items.Clear();
                List<Resource> resources = null;
                if (folderMode.CheckState == CheckState.Checked)
                {
                    resources = folderTools.GetFolders(dialog.FileName);
                }
                else
                {
                    resources = fileTools.GetFiles(dialog.FileName);
                }
                foreach (var item in resources)
                {
                    msgLB.Items.Add(item.FullName);
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
                List<string> list;
                if (folderMode.CheckState == CheckState.Checked) //文件夹模式
                {
                    List<Resource> resources = folderTools.GetFolders(pathTB.Text.GetPath());
                    if (isBackUp.CheckState == CheckState.Checked)
                    {
                        string backUpPath = folderTools.CreateBackUp(resources);
                        msgLB.Items.Add($"备份完成：{backUpPath}");
                    }

                    list = folderTools.ChangeName(resources); //修改文件名称，并返回结果
                }
                else
                {
                    List<Resource> resources = fileTools.GetFiles(pathTB.Text.GetPath());
                    if (isBackUp.CheckState == CheckState.Checked)
                    {
                        string backUpPath = fileTools.CreateBackUp(resources);
                        msgLB.Items.Add($"备份完成：{backUpPath}");
                    }

                    list = fileTools.ChangeName(resources); //修改文件名称，并返回结果
                }
                changeBtn.Enabled = false;
                isBackUp.Checked = false;
                foreach (var item in list)
                {
                    msgLB.Items.Add(item);
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

        private void folderMode_CheckedChanged(object sender, EventArgs e)
        {
            if (folderMode.CheckState == CheckState.Checked)
            {
                selectResourceBtn.Text = "选择文件夹";
                selectResourceBtn.BackColor = Color.NavajoWhite;
                pathTB.BackColor = Color.NavajoWhite;
            }
            else
            {
                selectResourceBtn.Text = "选择文件";
                selectResourceBtn.BackColor = SystemColors.Control;
                pathTB.BackColor = SystemColors.Control;
            }
            pathTB.Text = string.Empty;
            msgLB.Items.Clear();
        }
    }
}
