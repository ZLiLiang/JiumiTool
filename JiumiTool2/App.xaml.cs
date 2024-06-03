using System.IO;
using System.Windows;
using System.Windows.Threading;
using JiumiTool2.ViewModels;
using JiumiTool2.Services;
using JiumiTool2.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui;
using JiumiTool2.Models;
using JiumiTool2.IServices;

namespace JiumiTool2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// .NET泛型主机提供依赖注入
        /// </summary>
        private static readonly IHost _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(c =>
            {
                var basePath = Path.GetDirectoryName(AppContext.BaseDirectory) ?? throw new DirectoryNotFoundException("Unable to find the base directory of the application.");
                // 设置基础路径
                c.SetBasePath(basePath);
                // 加载appsettings.json
                c.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                // App Host
                services.AddHostedService<ApplicationHostService>();

                // 注册配置
                services.Configure<Appsettings>(context.Configuration);

                // Page resolver service
                services.AddSingleton<IPageService, PageService>();
                services.AddSingleton<IAppsettingsService, AppsettingsService>();
                services.AddSingleton<IContentDialogService, ContentDialogService>();
                services.AddSingleton<IMatchRegexService, MatchRegexService>();
                services.AddSingleton<IFileService, FileService>();
                services.AddSingleton<IFolderService, FolderService>();


                // Service containing navigation, same as INavigationWindow... but without window
                services.AddSingleton<INavigationService, NavigationService>();

                // Main window with navigation
                services.AddSingleton<INavigationWindow, MainView>();
                services.AddSingleton<MainViewModel>();

                // Views and ViewModels
                services.AddSingleton<JiumiView>();
                services.AddSingleton<JiumiViewModel>();
                services.AddSingleton<FileView>();
                services.AddSingleton<FileViewModel>();
                services.AddSingleton<VideoView>();
                services.AddSingleton<VideoViewModel>();
                services.AddSingleton<SettingsView>();
                services.AddSingleton<SettingsViewModel>();

                services.AddSingleton<FileConfigDialog>();
                services.AddSingleton<VideoConfigDialog>();
            })
            .Build();

        /// <summary>
        /// 返回服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T? GetService<T>()
        where T : class
        {
            return _host.Services.GetService(typeof(T)) as T;
        }

        /// <summary>
        /// 在应用加载时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();
        }

        /// <summary>
        /// 在应用退出时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();

            _host.Dispose();
        }

        /// <summary>
        /// 当应用程序引发但未处理异常时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {

        }
    }
}
