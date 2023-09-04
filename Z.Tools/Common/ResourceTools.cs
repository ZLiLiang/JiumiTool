using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Z.Tools.Modle;

namespace Z.Tools.Common
{
    public static class ResourceTools
    {
        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static List<Resource> GetFiles(string[] paths)
        {
            List<Resource> fileInfo = new List<Resource>();
            foreach (var path in paths)
            {
                FileInfo info = new FileInfo(path);
                Resource resource = new Resource();
                string name = info.Name;
                string rPath = info.Directory.FullName;
                string directory = info.Directory.Name;
                string fullname = info.FullName;
                resource.SetInfo(name, rPath, directory, fullname);
                fileInfo.Add(resource);
            }
            return fileInfo;
        }

        /// <summary>
        /// 获取文件夹信息
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static List<Resource> GetFolders(string[] paths)
        {
            List<Resource> folerInfo = new List<Resource>();
            foreach (var path in paths)
            {
                DirectoryInfo info = new DirectoryInfo(path);
                Resource resource = new Resource();
                string name = info.Name;
                string rPath = info.Parent.FullName;
                string directory = info.Parent.Name;
                string fullName = info.FullName;
                resource.SetInfo(name, rPath, directory, fullName);
                folerInfo.Add(resource);
            }
            return folerInfo;
        }

        /// <summary>
        /// 修改文件（夹）的名称
        /// </summary>
        /// <param name="resources"></param>
        /// <returns></returns>
        public static List<string> ChangeName(List<Resource> resources)
        {
            int fileCount = 0, folerCount = 0;
            List<string> result = new List<string>();
            foreach (var resource in resources)
            {
                if (File.Exists(resource.FullName))
                {
                    fileCount++;
                    string destFileName = RemoveParentheses(resource.Name, fileCount); //修改
                    if (resource.Name != destFileName)
                    {
                        File.Move(resource.FullName, destFileName); //重名
                        result.Add((string.Join(" -> ", resource.Name, destFileName)));
                    }
                }
                else
                {
                    folerCount++;
                    string destFolderName = RemoveParentheses(resource.Name, folerCount); //修改
                    if (resource.Name != destFolderName)
                    {
                        Directory.Move(resource.FullName, destFolderName); //重名
                        result.Add((string.Join(" -> ", resource.Name, destFolderName)));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 进行备份
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static string CreateBackUp(List<Resource> resources)
        {
            string pathDir = $"{resources.First().Directory}\\备份";
            DirectoryInfo directory = Directory.CreateDirectory(pathDir);
            foreach (var resource in resources)
            {
                string destName = Path.Combine(directory.FullName, resource.Name);
                if (File.Exists(resource.FullName))
                {
                    File.Copy(resource.FullName, destName); //拷贝文件
                }
                else
                {
                    CopyDirectory(resource.FullName, destName); //拷贝文件夹
                }
            }
            return pathDir;
        }

        /// <summary>
        /// 拷贝目录下的文件夹和文件
        /// </summary>
        /// <param name="folders">文件夹数组</param>
        /// <param name="destinationDir">目标目录</param>
        /// <param name="recursive">子目录</param>
        private static void CopyDirectory(string sourceDir, string destinationDir)
        {
            DirectoryInfo srcDir = new DirectoryInfo(sourceDir);
            if (srcDir.Name == "备份")
            {
                return; //跳过名为备份的文件夹
            }

            DirectoryInfo destDir = Directory.CreateDirectory(destinationDir); //创建目标文件夹

            foreach (var file in srcDir.GetFiles()) //对文件进行拷贝
            {
                string targetPath = Path.Combine(destDir.FullName, file.Name);
                file.CopyTo(targetPath);
            }

            if (srcDir.GetDirectories().Length != 0) //检查是否存在子目录
            {
                //对子目录进行拷贝
                foreach (var floder in srcDir.GetDirectories())
                {
                    string targetPath = Path.Combine(destinationDir, floder.Name);
                    CopyDirectory(floder.FullName, targetPath);
                }
            }
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
    }
}
