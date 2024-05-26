using System.IO;
using System.Text.RegularExpressions;
using JiumiTool2.IServices;
using JiumiTool2.Models;

namespace JiumiTool2.Services
{
    public class FileService : AResourceService, IFileService
    {
        public List<Models.File> GetFiles(string path)
        {
            // 检测是否为目录
            if (Directory.Exists(path) is false)
            {
                throw new IOException("不是一个有效的目录路径");
            }

            // 创建该路径的目录实例
            var info = new DirectoryInfo(path);
            // 获取当前目录的所有文件
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
                // 路径是文件夹时获取所有文件
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

        public List<string> RepeatName(IEnumerable<Resource> resources, Regex regex)
        {
            var result = base.RepeatName(resources, regex, System.IO.File.Move);

            return result;
        }

        public string Backup(IEnumerable<Resource> resources)
        {
            var result = base.Backup(resources, System.IO.File.Copy);

            return result;
        }
    }
}
