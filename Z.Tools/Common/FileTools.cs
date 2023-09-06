using System.Collections.Generic;
using System.IO;
using System.Linq;
using Z.Tools.Modle;
using File = Z.Tools.Modle.File;

namespace Z.Tools.Common
{
    public class FileTools : ResourceEdit
    {
        /// <summary>
        /// 获取目录下的文件
        /// </summary>
        /// <param name="path">目录路径</param>
        /// <returns></returns>
        public List<Resource> GetFiles(string path)
        {
            //检测是否为目录
            if (!Directory.Exists(path))
            {
                throw new IOException("不是一个有效的目录路径");
            }
            DirectoryInfo info = new DirectoryInfo(path); //创建目录实例
            List<Resource> files = new List<Resource>();
            //遍历文件
            foreach (var item in info.GetFiles())
            {
                File file = new File();
                file.SetInfo(item.Name, item.DirectoryName, item.FullName, item.Directory.Name);
                files.Add(file);
            }
            return files;
        }

        /// <summary>
        /// 从路径数组加载文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<Resource> GetFiles(string[] paths)
        {
            List<Resource> files = new List<Resource>();
            foreach (var path in paths)
            {
                //跳过非文件
                if (!System.IO.File.Exists(path))
                {
                    continue;
                }
                FileInfo info = new FileInfo(path);
                File file = new File();
                file.SetInfo(info.Name, info.DirectoryName, info.FullName, info.Directory.Name);
                files.Add(file);
            }
            return files;
        }

        /// <summary>
        /// 对文件进行重名
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
                System.IO.File.Move(srcFullName, destFullName);
                result.Add(string.Join(" -> ", item.Name, destName));
            }
            return result;
        }

        /// <summary>
        /// 创建备份
        /// </summary>
        /// <param name="resources"></param>
        /// <returns></returns>
        public override string CreateBackUp(List<Resource> resources)
        {
            string backUpPath = $"{resources.First().Path}\\备份";
            if (Directory.Exists(backUpPath))
            {
                return "存在名为备份的文件夹，自动跳过备份操作";
            }
            Directory.CreateDirectory(backUpPath); //创建目录
            foreach (var item in resources)
            {
                string srcFullName = item.FullName;
                string destFullName = Path.Combine(backUpPath, item.Name);
                System.IO.File.Copy(srcFullName, destFullName); //拷贝文件
            }
            return $"备份路径:{backUpPath}";
        }
    }
}
