using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using Z.JuimiTool.Extensions;
using Z.JuimiTool.IServices;
using Z.JuimiTool.Models;

namespace Z.JuimiTool.Services
{
    public class FolderService : IFolderService
    {
        public List<Folder> GetFolders(string path)
        {
            //检测是否为目录
            if (Directory.Exists(path) is false)
            {
                throw new IOException("不是一个有效的目录路径");
            }

            //创建该路径的目录实例
            var info = new DirectoryInfo(path);
            //获取当前目录的所有文件夹
            var folders = info.GetDirectories();
            var result = new List<Folder>();

            foreach (var folder in folders)
            {
                result.Add(new Folder
                {
                    Name = folder.Name,
                    FullName = folder.FullName,
                    Parent = folder.Parent.Name,
                    Path = folder.Parent.FullName,
                });
            }

            return result;
        }

        public List<Folder> GetFolders(IEnumerable<string> paths)
        {
            var result = new List<Folder>();
            foreach (var path in paths)
            {
                //跳过非文件
                if (Directory.Exists(path) is false)
                {
                    continue;
                }

                var info = new DirectoryInfo(path);
                result.Add(new Folder
                {
                    Name = info.Name,
                    FullName = info.FullName,
                    Parent = info.Parent.Name,
                    Path = info.Parent.FullName,
                });
            }

            return result;
        }

        public List<string> RepeatName(IEnumerable<Resource> resources)
        {
            int count = 0;
            var result = new List<string>();

            foreach (var item in resources)
            {
                //跳过包含备份字符串的路径
                if (item.Name.Contains("备份"))
                {
                    continue;
                }

                count++;
                var srcFullName = item.FullName;
                //除去括号
                var destName = StringExtension.RemoveParentheses(item.Name, count);
                var destFullName = Path.Combine(item.Path, destName);
                //重命名文件
                Directory.Move(srcFullName, destFullName);
                result.Add(string.Join(" -> ", item.Name, destName));
            }

            return result;
        }

        public string BackUp(IEnumerable<Resource> resources)
        {
            var backUpPath = $"{resources.First().Path}\\备份-{DateTime.Now:yyyy/MM/dd HH:mm:ss}";
            //创建目录
            Directory.CreateDirectory(backUpPath);
            foreach (var item in resources)
            {
                var srcFullName = item.FullName;
                var destFullName = Path.Combine(backUpPath, item.Name);
                //拷贝文件夹
                CopyDirectory(srcFullName, destFullName);
            }

            return $"备份路径:{backUpPath}";
        }

        /// <summary>
        /// 拷贝文件夹
        /// </summary>
        /// <param name="sourceDir">源文件夹</param>
        /// <param name="destinationDir">目标文件夹</param>
        private void CopyDirectory(string sourceDir, string destinationDir)
        {
            //获取原文件夹信息
            DirectoryInfo srcDir = new DirectoryInfo(sourceDir);
            //创建目标文件夹
            DirectoryInfo destDir = Directory.CreateDirectory(destinationDir);

            //复制文件
            foreach (var item in srcDir.GetFiles())
            {
                //拼接目标文件绝对路径
                string targetPath = Path.Combine(destDir.FullName, item.Name);
                item.CopyTo(targetPath);
            }

            //复制文件夹
            foreach (var item in srcDir.GetDirectories())
            {
                //拼接目标文件夹绝对路径
                string targetPath = Path.Combine(destDir.FullName, item.Name);
                CopyDirectory(item.FullName, targetPath);
            }
        }
    }
}
