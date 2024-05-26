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

        public IDisposable ChangeToNotification(Action action)
        {
            var result = _optionsMonitor.OnChange((appsettings, name) =>
            {
                action();
                Console.WriteLine(name);
            });

            return result;
        }

        public void UpdateAppsettings(Action<Appsettings> action)
        {
            // 配置文件路径
            string configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

            // 读取JSON文件到Appsettings对象
            var content = System.IO.File.ReadAllText(configPath);
            var appSettings = JsonConvert.DeserializeObject<Appsettings>(content);

            // 修改属性值
            action.Invoke(appSettings);

            // 将更改保存回文件
            var updatedContent = JsonConvert.SerializeObject(appSettings, Formatting.Indented);
            System.IO.File.WriteAllText(configPath, updatedContent);
        }

        public async Task UpdateAppsettingsAsync(Action<Appsettings> action)
        {
            // 配置文件路径
            string configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

            // 读取JSON文件到Appsettings对象
            var content = await System.IO.File.ReadAllTextAsync(configPath);
            var appSettings = JsonConvert.DeserializeObject<Appsettings>(content);

            // 修改属性值
            action.Invoke(appSettings);

            // 将更改保存回文件
            var updatedContent = JsonConvert.SerializeObject(appSettings, Formatting.Indented);
            await System.IO.File.WriteAllTextAsync(configPath, updatedContent);
        }
    }
}
