using JiumiTool3.ViewModels;
using JiumiTool3.Views;
using Microsoft.Extensions.DependencyInjection;

namespace JiumiTool3.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddTransient<MainViewModel>();
    }
}
