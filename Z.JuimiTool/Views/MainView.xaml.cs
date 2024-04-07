using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Z.JuimiTool.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        private Point _pressedPosition;
        private bool _isDragMoved = false;

        public MainView()
        {
            InitializeComponent();

            btnMin.Click += (s, e) => WindowState = WindowState.Minimized;
            btnClose.Click += (s, e) => Close();

            dpMove.PreviewMouseLeftButtonDown += Window_PreviewMouseLeftButtonDown;
            dpMove.PreviewMouseLeftButtonUp += Window_PreviewMouseLeftButtonUp;
            dpMove.PreviewMouseMove += Window_PreviewMouseMove;

            spMove.PreviewMouseLeftButtonDown += Window_PreviewMouseLeftButtonDown;
            spMove.PreviewMouseLeftButtonUp += Window_PreviewMouseLeftButtonUp;
            spMove.PreviewMouseMove += Window_PreviewMouseMove;
        }

        #region 窗口移动

        private void Window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _pressedPosition = e.GetPosition(this);
        }

        private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed && _pressedPosition != e.GetPosition(this))
            {
                _isDragMoved = true;
                DragMove();
            }
        }

        private void Window_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragMoved)
            {
                _isDragMoved = false;
                e.Handled = true;
            }
        }

        #endregion
    }
}
