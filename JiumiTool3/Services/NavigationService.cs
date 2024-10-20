using Avalonia.Controls;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Media.Animation;
using JiumiTool3.Contract;
using JiumiTool3.IServices;
using JiumiTool3.Pages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace JiumiTool3.Services;

public class NavigationService : INavigationService
{
    public void Navigate(Type type)
    {
        throw new NotImplementedException();
    }

    public void NavigateFromContext(object dataContext, NavigationTransitionInfo transitionInfo = null)
    {
        throw new NotImplementedException();
    }

    public void SetFrame(Frame frame)
    {
        throw new NotImplementedException();
    }
}

public class NavigationFactory : INavigationPageFactory
{
    private readonly Dictionary<PagesEnum, Control> CorePages = new Dictionary<PagesEnum, Control>
    {
        { PagesEnum.SettingsPage, App.Services.GetRequiredService<SettingsPage>() }
    };

    public Control GetPage(Type srcType)
    {
        if (App.Services.GetRequiredService(srcType) is not Control control)
        {
            throw new InvalidOperationException("srcType is not exist!");
        }

        return control;
    }

    public Control GetPageFromObject(object target)
    {
        if (target is not PagesEnum pagesEnum)
        {
            throw new ArgumentException("target must is pagesEnum!");
        }

        if (CorePages.TryGetValue(pagesEnum, out Control control) is false)
        {
            throw new InvalidOperationException("target is not exist!");
        }

        return control;
    }
}
