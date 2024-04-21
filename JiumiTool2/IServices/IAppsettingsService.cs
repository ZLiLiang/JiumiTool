using JiumiTool2.Models;

namespace JiumiTool2.IServices
{
    public interface IAppsettingsService
    {
        public Appsettings GetAppsettings();

        public Task UpdateAppsettingsAsync(string key, string value);
    }
}
