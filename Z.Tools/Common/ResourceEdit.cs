using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Diagnostics;
using Z.Tools.Modle;
using File = Z.Tools.Modle.File;

namespace Z.Tools.Common
{
    public abstract class ResourceEdit
    {
        /// <summary>
        /// 返回路径下的文件和文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<Resource> GetResources(string path)
        {
            if (!Directory.Exists(path)) return null;
            List<Resource> resources = new List<Resource>();
            DirectoryInfo directory = new DirectoryInfo(path);
            //获取文件
            foreach (var item in directory.GetFiles())
            {
                File file = new File();
                file.SetInfo(item.Name, item.DirectoryName, item.FullName, item.Directory.Name);
                resources.Add(file);
            }
            //获取文件夹
            foreach (var item in directory.GetDirectories())
            {
                Folder folder = new Folder();
                folder.SetInfo(item.Name, item.Parent.Name, item.FullName, item.Parent.Name);
                resources.Add(folder);
            }

            return resources;
        }

        public abstract List<string> ChangeName(List<Resource> resources);

        public abstract string CreateBackUp(List<Resource> resources);

        /// <summary>
        /// 删除文件名的括号以及括号的内容
        /// </summary>
        /// <param name="srcName"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected static string RemoveParentheses(string srcName, int count)
        {
            string fileName = Regex.Replace(srcName, @"\([^)]*\)", ""); // 去除括号及内容

            fileName = Regex.Replace(fileName, @"(?:\s|\d)+(?=\.|\s|$)", math =>
            {
                return count.ToString();
            }); //对文件名进行编号

            return fileName;
        }
    }
}
