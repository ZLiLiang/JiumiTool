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
        /// 获取配置监视器
        /// </summary>
        /// <returns></returns>
        public IOptionsMonitor<Appsettings> GetOptionsMonitor();

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
