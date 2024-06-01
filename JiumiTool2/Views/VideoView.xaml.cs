using System.Windows.Controls;
using JiumiTool2.ViewModels;

namespace JiumiTool2.Views
{
    /// <summary>
    /// VideoView.xaml 的交互逻辑
    /// </summary>
    public partial class VideoView : Page
    {
        public VideoViewModel ViewModel { get; set; }

        public VideoView(VideoViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}
