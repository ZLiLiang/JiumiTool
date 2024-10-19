using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using JiumiTool3.Extensions;
using JiumiTool3.Views;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace JiumiTool3;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = GetServiceProvider();

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        LogSetup();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView();
        }

        Log.Information("OK");

        base.OnFrameworkInitializationCompleted();
    }

    private static IServiceProvider GetServiceProvider()
    {
        // 注册应用程序运行所需的所有服务
        var collection = new ServiceCollection();
        collection.AddCommonServices();

        // 从 collection 提供的 IServiceCollection 中创建包含服务的 ServiceProvider
        var services = collection.BuildServiceProvider();

        return services;
    }

    private void LogSetup()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(
                Path.Combine(AppContext.BaseDirectory, $@"Logs\log-.txt"),
                //每天一个文件 ,生成类似：log-20230914.txt
                //fileSizeLimitBytes: null 文件限制大小：无   如果不配置默认是：1GB 1GB就不记录了
                //retainedFileCountLimit: null   保留文件数量   如果不配置只保留31天的日志
                //https://github.com/serilog/serilog-sinks-file 
                rollingInterval: RollingInterval.Day, retainedFileCountLimit: null,
                //fileSizeLimitBytes: null,
                //单个文件大小： 1024000 1024000是1M
                //rollOnFileSizeLimit: true 就是滚动文件,如果超过单个文件大小，会滚动文件 产生类似：log.txt log_001.txt log_002.txt
                fileSizeLimitBytes: 3024000, rollOnFileSizeLimit: true,
                //非必填：指定最小等级
                restrictedToMinimumLevel: LogEventLevel.Information,
                //非必填： 也可以指定输出格式：这种格式好像与系统默认没有什么区别
                //outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                //outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception} {NewLine}{Version}{myval}"
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception} {NewLine}{Version}{myval} {NewLine}{UserId}{myid}{NewLine}")
            .CreateLogger();
    }
}
