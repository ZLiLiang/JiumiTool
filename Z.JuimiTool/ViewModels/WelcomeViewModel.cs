using System;
using System.Windows.Threading;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Z.JuimiTool.ViewModels
{
    public class WelcomeViewModel : BindableBase, IDialogAware
    {
        private readonly DispatcherTimer _timer;

        public WelcomeViewModel(DispatcherTimer timer)
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            _timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            // 关闭对话框
            //_timer.Stop();
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            // 清理操作，比如停止计时器
            _timer.Stop();
            _timer.Tick -= Timer_Tick;
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            // 启动计时器
            _timer.Start();
        }

        public string Title => "啾咪";

        public event Action<IDialogResult> RequestClose;
    }
}
