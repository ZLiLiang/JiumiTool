using Avalonia.Controls;
using JiumiTool3.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace JiumiTool3.Pages;

public partial class SettingsPage : UserControl
{
    public SettingsPage()
    {
        InitializeComponent();

        DataContext = App.Services.GetRequiredService<SettingsPageViewModel>();
    }
}