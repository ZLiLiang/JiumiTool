using System;
using System.Threading.Tasks;
using System.Windows;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Services.Dialogs;
using Z.JuimiTool.Common;
using Z.JuimiTool.IServices;
using Z.JuimiTool.Services;
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
            //服务
            containerRegistry.Register<IFileService, FileService>();
            containerRegistry.Register<IFolderService, FolderService>();

            //弹窗
            containerRegistry.RegisterDialog<WelcomeView, WelcomeViewModel>();

            //导航
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
            var dialog = Container.Resolve<IDialogService>();
            dialog.ShowDialog(typeof(WelcomeView).Name);

            var service = Current.MainWindow.DataContext as IConfigureService;
            service.Configure();

            base.OnInitialized();
        }
    }
}
