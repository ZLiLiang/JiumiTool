using JiumiTool2.Models;
using Microsoft.Extensions.Options;

namespace JiumiTool2.IServices
{
    public interface IAppsettingsService
    {
        /// <summary>
        /// 获取配置类
        /// </summary>
        /// <returns></returns>
        public Appsettings GetAppsettings();

        /// <summary>
        /// 配置发送改变时，回调通知函数
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public IDisposable ChangeToNotification( Action action);

        /// <summary>
        /// 更新配置
        /// </summary>
        /// <param name="action"></param>
        public void UpdateAppsettings(Action<Appsettings> action);

        /// <summary>
        /// 异步更新配置
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public Task UpdateAppsettingsAsync(Action<Appsettings> action);
    }
}
