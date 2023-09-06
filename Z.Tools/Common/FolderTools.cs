using System.Collections.Generic;
using System.IO;
using System.Linq;
using Z.Tools.Modle;

namespace Z.Tools.Common
{
    public class FolderTools : ResourceEdit
    {
        /// <summary>
        /// 获取路径下的文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="IOException"></exception>
        public List<Resource> GetFolders(string path)
        {
            //检测是否为目录
            if (!Directory.Exists(path))
            {
                throw new IOException("不是一个有效的目录路径");
            }
            DirectoryInfo info = new DirectoryInfo(path); //创建目录实例
            List<Resource> folders = new List<Resource>();
            //遍历文件夹
            foreach (var item in info.GetDirectories())
            {
                Folder folder = new Folder();
                folder.SetInfo(item.Name, item.Parent.FullName, item.FullName, item.Parent.Name);
                folders.Add(folder);
            }
            return folders;
        }

        /// <summary>
        /// 从路径数组加载文件夹
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public List<Resource> GetFolders(string[] paths)
        {
            List<Resource> folders = new List<Resource>();
            foreach (var path in paths)
            {
                //跳过非文件
                if (!Directory.Exists(path))
                {
                    continue;
                }
                DirectoryInfo info = new DirectoryInfo(path);
                Folder folder = new Folder();
                folder.SetInfo(info.Name, info.Parent.FullName, info.FullName, info.Parent.Name);
                folders.Add(folder);
            }
            return folders;
        }

        /// <summary>
        /// 对文件夹进行重名
        /// </summary>
        /// <param name="resources"></param>
        /// <returns></returns>
        public override List<string> ChangeName(List<Resource> resources)
        {
            int count = 0;
            List<string> result = new List<string>();
            foreach (var item in resources)
            {
                if (item.Name == "备份")
                {
                    continue;
                }
                count++;
                string srcFullName = item.FullName;
                string destName = RemoveParentheses(item.Name, count);
                string destFullName = Path.Combine(item.Path, destName);
                Directory.Move(srcFullName, destFullName);
                result.Add(string.Join(" -> ", item.Name, destName));
            }
            return result;
        }

        /// <summary>
        /// 对文件夹进行备份
        /// </summary>
        /// <param name="resources"></param>
        /// <returns></returns>
        public override string CreateBackUp(List<Resource> resources)
        {
            string backUpPath = $"{resources.First().Path}//备份";
            if (Directory.Exists(backUpPath))
            {
                return "存在名为备份的文件夹，自动跳过备份操作";
            }
            Directory.CreateDirectory(backUpPath); //创建目录
            foreach (var item in resources)
            {
                string srcFullName = item.FullName;
                string destFullName = Path.Combine(backUpPath, item.Name);
                CopyDirectory(srcFullName, destFullName); //拷贝文件夹
            }
            return $"备份路径:{backUpPath}";
        }

        /// <summary>
        /// 拷贝文件夹
        /// </summary>
        /// <param name="srcDir">源文件夹</param>
        /// <param name="destDir">目标文件夹</param>
        private void CopyDirectory(string sourceDir, string destinationDir)
        {
            DirectoryInfo srcDir = new DirectoryInfo(sourceDir);
            DirectoryInfo destDir = Directory.CreateDirectory(destinationDir);

            //复制文件
            foreach (var item in srcDir.GetFiles())
            {
                string targetPath = Path.Combine(destDir.FullName, item.Name); //拼接目标文件绝对路径
                item.CopyTo(targetPath);
            }

            //复制文件夹
            foreach (var item in srcDir.GetDirectories())
            {
                string targetPath = Path.Combine(destDir.FullName, item.Name); //拼接目标文件夹绝对路径
                CopyDirectory(item.FullName, targetPath);
            }
        }
    }
}
