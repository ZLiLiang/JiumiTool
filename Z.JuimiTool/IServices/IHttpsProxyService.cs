using System;
using System.Collections.Generic;
using Z.JuimiTool.Models;

namespace Z.JuimiTool.IServices
{
    public interface IHttpsProxyService
    {
        /// <summary>
        /// 传播视频实体
        /// </summary>
        public event Action<VideoDownloadInfo> VideoAddedToList;

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
