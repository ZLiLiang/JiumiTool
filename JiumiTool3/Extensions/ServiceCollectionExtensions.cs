using FluentAvalonia.UI.Controls;
using JiumiTool3.IServices;
using JiumiTool3.Pages;
using JiumiTool3.Services;
using JiumiTool3.ViewModels;
using JiumiTool3.Views;
using Microsoft.Extensions.DependencyInjection;

namespace JiumiTool3.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddViewAndModelServices(this IServiceCollection collection)
    {
        collection.AddTransient<MainViewModel>();
        collection.AddTransient<SettingsPageViewModel>();

        collection.AddTransient<MainWindow>();
        collection.AddTransient<MainView>();
        collection.AddTransient<SettingsPage>();
    }

    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddSingleton<INavigationPageFactory, NavigationFactory>();
        collection.AddSingleton<INavigationService, NavigationService>();
    }
}
