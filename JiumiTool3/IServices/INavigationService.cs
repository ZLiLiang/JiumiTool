using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Media.Animation;
using System;

namespace JiumiTool3.IServices;

public interface INavigationService
{
    void SetFrame(Frame frame);

    void Navigate(Type type);

    void NavigateFromContext(object dataContext, NavigationTransitionInfo transitionInfo = null);
}
