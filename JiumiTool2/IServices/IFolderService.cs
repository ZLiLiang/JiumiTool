using System.Text.RegularExpressions;
using JiumiTool2.Models;

namespace JiumiTool2.IServices
{
    public interface IFolderService
    {
        /// <summary>
        /// 获取路径下的文件夹
        /// </summary>
        /// <param name="path">单个路径</param>
        /// <returns></returns>
        public List<Folder> GetFolders(string path);

        /// <summary>
        /// 获取多个路径的文件夹
        /// </summary>
        /// <param name="paths">多个路径</param>
        /// <returns></returns>
        public List<Folder> GetFolders(IEnumerable<string> paths);

        /// <summary>
        /// 重命名资源 
        /// </summary>
        /// <param name="resources">资源列表</param>
        /// <param name="regex">正则匹配器</param>
        /// <returns>路径列表</returns>
        public List<string> RepeatName(IEnumerable<Resource> resources, Regex regex);

        /// <summary>
        /// 备份资源 
        /// </summary>
        /// <param name="resources">资源列表</param>
        /// <returns>路径</returns>
        public string Backup(IEnumerable<Resource> resources);
    }
}
