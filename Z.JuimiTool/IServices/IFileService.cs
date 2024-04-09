using System.Collections.Generic;
using Z.JuimiTool.Models;

namespace Z.JuimiTool.IServices
{
    public interface IFileService : IResourceService
    {
        /// <summary>
        /// 获取路径下的文件
        /// </summary>
        /// <param name="path">单个路径</param>
        /// <returns></returns>
        public List<File> GetFiles(string path);

        /// <summary>
        /// 获取多个路径的文件 <br/>
        /// 路径包括文件夹和文件
        /// </summary>
        /// <param name="paths">多个路径</param>
        /// <returns></returns>
        public List<File> GetFiles(IEnumerable<string> paths);
    }
}
