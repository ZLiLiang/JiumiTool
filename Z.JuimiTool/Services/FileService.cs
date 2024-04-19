using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Z.JiumiTool.Extensions;
using Z.JiumiTool.IServices;
using Z.JiumiTool.Models;

namespace Z.JiumiTool.Services
{
    public class FileService : IFileService
    {
        public List<Models.File> GetFiles(string path)
        {
            //检测是否为目录
            if (Directory.Exists(path) is false)
            {
                throw new IOException("不是一个有效的目录路径");
            }

            //创建该路径的目录实例
            var info = new DirectoryInfo(path);
            //获取当前目录的所有文件
            var files = info.GetFiles();
            var result = new List<Models.File>();

            foreach (var file in files)
            {
                result.Add(new Models.File
                {
                    Name = file.Name,
                    FullName = file.FullName,
                    Path = file.DirectoryName,
                    Directory = file.Directory.Name
                });
            }

            return result;
        }

        public List<Models.File> GetFiles(IEnumerable<string> paths)
        {
            var result = new List<Models.File>();
            foreach (var path in paths)
            {
                //路径是文件夹时获取所有文件
                if (Directory.Exists(path))
                {
                    var files = GetFiles(path);
                    result.AddRange(files);
                }
                var file = new FileInfo(path);
                result.Add(new Models.File
                {
                    Name = file.Name,
                    FullName = file.FullName,
                    Path = file.DirectoryName,
                    Directory = file.Directory.Name
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
                System.IO.File.Move(srcFullName, destFullName);
                result.Add(string.Join(" -> ", item.Name, destName));
            }

            return result;
        }

        public string Backup(IEnumerable<Resource> resources)
        {
            var backUpPath = $"{resources.First().Path}\\备份-{DateTime.Now:yyyy/MM/dd HH:mm:ss}";
            //创建目录
            Directory.CreateDirectory(backUpPath);
            foreach (var item in resources)
            {
                var srcFullName = item.FullName;
                var destFullName = Path.Combine(backUpPath, item.Name);
                //拷贝文件
                System.IO.File.Copy(srcFullName, destFullName);
            }

            return $"备份路径:{backUpPath}";
        }
    }
}
