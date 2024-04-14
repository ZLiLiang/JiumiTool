using System.Collections.Generic;
using Z.JuimiTool.Models;

namespace Z.JuimiTool.IServices
{
    public interface IHttpsProxyService
    {
        /// <summary>
        /// 视频信息列表
        /// </summary>
        public List<Video> VideoInfos { get; set; }

        /// <summary>
        /// 开始监听
        /// </summary>
        public void Start();

        /// <summary>
        /// 停止监听
        /// </summary>
        public void Stop();
    }
}
