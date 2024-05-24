using System.IO;
using JiumiTool2.IServices;
using JiumiTool2.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace JiumiTool2.Services
{
    public class AppsettingsService : IAppsettingsService
    {
        private readonly IOptionsMonitor<Appsettings> _optionsMonitor;

        public AppsettingsService(IOptionsMonitor<Appsettings> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor;
        }

        public Appsettings GetAppsettings()
        {
            return _optionsMonitor.CurrentValue;
        }

        public IOptionsMonitor<Appsettings> GetOptionsMonitor()
        {
            return _optionsMonitor;
        }

        public void UpdateAppsettings(Action<Appsettings> action)
        {
            // 配置文件路径
            string configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

            // 读取JSON文件到Appsettings对象
            var content = File.ReadAllText(configPath);
            var appSettings = JsonConvert.DeserializeObject<Appsettings>(content);

            // 修改属性值
            action.Invoke(appSettings);

            // 将更改保存回文件
            var updatedContent = JsonConvert.SerializeObject(appSettings, Formatting.Indented);
            File.WriteAllText(configPath, updatedContent);
        }

        public async Task UpdateAppsettingsAsync(Action<Appsettings> action)
        {
            // 配置文件路径
            string configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

            // 读取JSON文件到Appsettings对象
            var content = await File.ReadAllTextAsync(configPath);
            var appSettings = JsonConvert.DeserializeObject<Appsettings>(content);

            // 修改属性值
            action.Invoke(appSettings);

            // 将更改保存回文件
            var updatedContent = JsonConvert.SerializeObject(appSettings, Formatting.Indented);
            await File.WriteAllTextAsync(configPath, updatedContent);
        }
    }
}
