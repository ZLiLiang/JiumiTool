using System;
using System.Threading.Tasks;
using System.Windows;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Services.Dialogs;
using Z.JiumiTool.Common;
using Z.JiumiTool.IServices;
using Z.JiumiTool.Services;
using Z.JiumiTool.ViewModels;
using Z.JiumiTool.Views;

namespace Z.JiumiTool
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
            containerRegistry.RegisterSingleton<IHttpsProxyService, HttpsProxyService>();
            containerRegistry.RegisterSingleton<IDownloadService, DownloadService>();

            //弹窗
            containerRegistry.RegisterDialog<WelcomeView, WelcomeViewModel>();

            //导航
            containerRegistry.RegisterForNavigation<JiumiView>();
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
