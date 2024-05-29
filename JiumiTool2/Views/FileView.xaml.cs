using System.Windows;
using System.Windows.Controls;
using JiumiTool2.ViewModels;
using Wpf.Ui.Controls;

namespace JiumiTool2.Views
{
    /// <summary>
    /// FileView.xaml 的交互逻辑
    /// </summary>
    public partial class FileView : INavigableView<FileViewModel>
    {
        public FileViewModel ViewModel { get; }

        public FileView(FileViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}
