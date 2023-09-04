using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Z.Tools.Common
{
    public static class FileTools
    {
        /// <summary>
        /// 获取文件夹中的文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static FileInfo[] GetFiles(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            FileInfo[] files = directory.GetFiles();
            return files;
        }

        /// <summary>
        /// 获取多个路径的文件
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static FileInfo[] GetFiles(string[] paths)
        {
            List<FileInfo> files = new List<FileInfo>();
            foreach (string path in paths)
            {
                FileInfo fileInfo = new FileInfo(path);
                files.Add(fileInfo);
            }
            return files.ToArray();
        }

        /// <summary>
        /// 获取文件夹中的文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static DirectoryInfo[] GetFolders(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            DirectoryInfo[] folders = directory.GetDirectories();
            return folders;
        }

        /// <summary>
        /// 获取多个路径的文件夹
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static DirectoryInfo[] GetFolders(string[] paths)
        {
            List<DirectoryInfo> folders = new List<DirectoryInfo>();
            foreach (string path in paths)
            {
                DirectoryInfo folder = new DirectoryInfo(path);
                folders.Add(folder);
            }
            return folders.ToArray();
        }

        /// <summary>
        /// 返回路径文件（夹）的名称
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetName(string path)
        {
            if (File.Exists(path))
            {
                return Path.GetFileName(path);
            }
            else
            {
                return Path.GetDirectoryName(path);
            }
        }

        /// <summary>
        /// 进行备份(文件)
        /// </summary>
        /// <param name="files"></param>
        public static string CreateBackUp(FileInfo[] files)
        {
            string pathDir = $"{files.First().DirectoryName}\\备份";
            DirectoryInfo directory = new DirectoryInfo(pathDir);
            if (directory.Exists == false)
            {
                directory.Create();
            }
            foreach (var file in files)
            {
                var destFileName = Path.Combine(directory.FullName, file.Name);
                if (!File.Exists(destFileName))
                {
                    file.CopyTo(destFileName);
                }
            }
            return pathDir;
        }

        /// <summary>
        /// 进行备份(文件夹)
        /// </summary>
        /// <param name="folders"></param>
        public static string CreateBackUp(DirectoryInfo[] folders)
        {
            string pathDir = $"{folders.First().Parent.FullName}\\备份";
            DirectoryInfo directory = new DirectoryInfo(pathDir);
            if (directory.Exists == false)
            {
                directory.Create();
            }

            CopyDirectory(folders, directory.FullName);

            return pathDir;
        }

        /// <summary>
        /// 修改文件的名称
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public static List<string> ChangeFileName(FileInfo[] files)
        {
            int count = 0;
            List<string> result = new List<string>();
            string srcPath = string.Empty;
            foreach (var file in files)
            {
                count++;
                srcPath = file.DirectoryName;
                string srcFileName = file.Name;
                string destFileName = RemoveParentheses(srcFileName, count);
                if (srcFileName != destFileName)
                {
                    result.Add(string.Join(" -> ", srcFileName, destFileName));
                    file.MoveTo(Path.Combine(srcPath, destFileName)); //修改文件名
                }
            }
            result.Add("保存位置：" + srcPath);
            return result;
        }

        /// <summary>
        /// 修改文件夹的名称
        /// </summary>
        /// <returns></returns>
        public static List<string> ChangeFileName(DirectoryInfo[] folders)
        {
            int count = 0;
            List<string> result = new List<string>();
            string srcPath = string.Empty;
            foreach (var folder in folders)
            {
                count++;
                srcPath = folder.Parent.FullName;
                string srcFileName = folder.Name;
                string destFileName = RemoveParentheses(srcFileName, count);
                if (srcFileName != destFileName)
                {
                    result.Add(string.Join(" -> ", srcFileName, destFileName));
                    folder.MoveTo(Path.Combine(srcPath, destFileName)); //修改文件名
                }
            }
            result.Add("保存位置：" + srcPath);
            return result;
        }

        /// <summary>
        /// 删除文件名的括号以及括号的内容
        /// </summary>
        /// <param name="srcFileName"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private static string RemoveParentheses(string srcFileName, int count)
        {
            string fileName = Regex.Replace(srcFileName, @"\([^)]*\)", ""); // 去除括号及内容

            fileName = Regex.Replace(fileName, @"(?:\s|\d)+(?=\.|\s|$)", math =>
            {
                return count.ToString();
            }); //对文件名进行编号

            return fileName;
        }

        /// <summary>
        /// 拷贝目录下的文件夹和文件
        /// </summary>
        /// <param name="folders">文件夹数组</param>
        /// <param name="destinationDir">目标目录</param>
        /// <param name="recursive">子目录</param>
        private static void CopyDirectory(DirectoryInfo[] folders, string destinationDir)
        {
            foreach (var folder in folders)
            {
                if (folder.Name == "备份") continue; //跳过名为备份的文件夹
                DirectoryInfo copyDirectory = new DirectoryInfo(Path.Combine(destinationDir, folder.Name)); //拼接路径不同父文件夹的同名文件夹
                if (copyDirectory.Exists == false) copyDirectory.Create(); //创建要拷贝的目录
                foreach (var file in folder.GetFiles())
                {
                    string targetFilePath = Path.Combine(copyDirectory.FullName, file.Name);
                    file.CopyTo(targetFilePath);
                }

                if (folder.GetDirectories().Length != 0) //检查是否存在子目录
                {
                    CopyDirectory(folder.GetDirectories(), copyDirectory.FullName);
                }
            }
        }
    }
}
