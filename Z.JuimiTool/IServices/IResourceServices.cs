using System.Collections.Generic;
using Z.JuimiTool.Models;

namespace Z.JuimiTool.IServices
{
    public interface IResourceServices
    {
        /// <summary>
        /// 获取路径下的资源
        /// </summary>
        /// <param name="path">单个路径</param>
        /// <returns></returns>
        public List<Resource> GetResources(string path);

        /// <summary>
        /// 获取多个路径下的资源
        /// </summary>
        /// <param name="paths">多个路径</param>
        /// <returns></returns>
        public List<Resource> GetResources(IEnumerable<string> paths);

        /// <summary>
        /// 重命名资源
        /// </summary>
        /// <param name="resources">资源列表</param>
        /// <returns>路径列表</returns>
        public List<string> RepeatName(IEnumerable<Resource> resources);

        /// <summary>
        /// 备份资源
        /// </summary>
        /// <param name="resources">资源列表</param>
        /// <returns>路径</returns>
        public string BackUp(IEnumerable<Resource> resources);
    }
}
