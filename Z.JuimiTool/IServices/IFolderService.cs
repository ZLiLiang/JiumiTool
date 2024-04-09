using System.Collections.Generic;
using Z.JuimiTool.Models;

namespace Z.JuimiTool.IServices
{
    public interface IFolderService : IResourceService
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
    }
}
