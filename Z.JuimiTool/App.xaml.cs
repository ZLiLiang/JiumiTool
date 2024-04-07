using System;
using System.Windows;
using Prism.DryIoc;
using Prism.Ioc;
using Z.JuimiTool.ViewModels;
using Z.JuimiTool.Views;

namespace Z.JuimiTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<JuimiView>();
            containerRegistry.RegisterForNavigation<FileView, FileViewModel>();
            containerRegistry.RegisterForNavigation<VideoView, VideoViewModel>();
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainView>();
        }

        protected override void OnInitialized()
        {
            var service = App.Current.MainWindow.DataContext as MainViewModel;
            service.InitRegion();

            base.OnInitialized();
        }
    }
}
