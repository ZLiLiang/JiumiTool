using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using System;
using System.IO;
using System.Threading.Tasks;

namespace JiumiTool3.Views;

public partial class MainAppSplashContent : UserControl
{
    public MainAppSplashContent()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (!Design.IsDesignMode)
        {
            Dispatcher.UIThread.Post(async () =>
            {
                await RunAnimationsAsync();
            }, DispatcherPriority.Background);
        }

        if (!Design.IsDesignMode)
        {
            Dispatcher.UIThread.Post(async () =>
            {
                await RunTextAsync();
            }, DispatcherPriority.Background);
        }
    }

    private async Task RunAnimationsAsync()
    {
        while (true)
        {
            for (int i = 1; i < 24; i++)
            {
                var path = Path.Combine(AppContext.BaseDirectory, $@"Assets\gif\{i}.png");
                image.Source = new Bitmap(path);
                await Task.Delay(95);
            }
        }
    }

    private async Task RunTextAsync()
    {
        var start = DateTime.Now.Ticks;
        var time = start;
        var progressValue = 0;

        while ((time - start) < TimeSpan.TicksPerSecond)
        {
            progressValue++;
            progressBar.Value = progressValue;
            await Task.Delay(100);
            time = DateTime.Now.Ticks;
        }

        start = time;
        loadingText.Text = "Initializing settings";
        var limit = TimeSpan.TicksPerSecond * 2.5;
        while ((time - start) < limit)
        {
            progressValue += 1;
            progressBar.Value = progressValue;
            await Task.Delay(150);
            time = DateTime.Now.Ticks;
        }

        loadingText.Text = "Preparing app...";
        while (progressValue < 100)
        {
            progressValue += 1;
            progressBar.Value = progressValue;
            await Task.Delay(10);
        }
    }
}