using System.Windows.Controls;
using JiumiTool2.ViewModels;
using Wpf.Ui.Controls;

namespace JiumiTool2.Views
{
    /// <summary>
    /// JiumiView.xaml 的交互逻辑
    /// </summary>
    public partial class JiumiView : INavigableView<JiumiViewModel>
    {
        public JiumiViewModel ViewModel { get; }

        public JiumiView(JiumiViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }

    }
}
