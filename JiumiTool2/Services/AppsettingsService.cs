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

        public async Task UpdateAppsettingsAsync(string key, string value)
        {
            // 确保使用正确的路径
            string configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

            // 读取JSON文件到MyAppSettings对象
            var content = await File.ReadAllTextAsync(configPath);
            var appSettings = JsonConvert.DeserializeObject<Appsettings>(content);

            // 修改属性值
            typeof(Appsettings).GetProperty(key)?.SetValue(appSettings, value);

            // 将更改保存回文件
            var updatedContent = JsonConvert.SerializeObject(appSettings, Formatting.Indented);
            await File.WriteAllTextAsync(configPath, updatedContent);
        }
    }
}
