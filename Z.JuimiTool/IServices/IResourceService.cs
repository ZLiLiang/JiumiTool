using System.Collections.Generic;
using Z.JuimiTool.Models;

namespace Z.JuimiTool.IServices
{
    public interface IResourceService
    {
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
        public string Backup(IEnumerable<Resource> resources);
    }
}
