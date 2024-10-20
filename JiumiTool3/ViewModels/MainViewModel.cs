using FluentAvalonia.UI.Controls;
using JiumiTool3.Services;

namespace JiumiTool3.ViewModels;

public class MainViewModel : ViewModelBase
{
    public INavigationPageFactory NavigationFactory { get; init; }

    public MainViewModel(INavigationPageFactory navigationFactory)
    {
        NavigationFactory = navigationFactory;
    }
}