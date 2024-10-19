using Avalonia.Controls;
using JiumiTool3.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace JiumiTool3.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        DataContext = App.Services.GetRequiredService<MainViewModel>();
    }
}
