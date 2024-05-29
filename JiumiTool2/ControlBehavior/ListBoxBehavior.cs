using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Collections.Specialized;

namespace JiumiTool2.ControlBehavior
{
    public static class ListBoxBehavior
    {
        /// <summary>
        /// 属性的访问构造器
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetAutoScrollToEnd(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollToEndProperty);
        }

        /// <summary>
        /// 属性的设置构造器
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetAutoScrollToEnd(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollToEndProperty, value);
        }

        /// <summary>
        /// 设置一个属性
        /// </summary>
        public static readonly DependencyProperty AutoScrollToEndProperty =
            DependencyProperty.RegisterAttached(
                "AutoScrollToEnd",
                typeof(bool),
                typeof(ListBoxBehavior),
                new PropertyMetadata(false, OnAutoScrollToEndChanged));

        private static void OnAutoScrollToEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListBox listBox && (bool)e.NewValue)
            {
                listBox.Loaded += (s, ea) =>
                {
                    var collectionView = CollectionViewSource.GetDefaultView(listBox.ItemsSource);
                    if (collectionView is INotifyCollectionChanged notifyCollection)
                    {
                        notifyCollection.CollectionChanged += (sender, args) =>
                        {
                            if (listBox.Items.Count > 0)
                            {
                                listBox.ScrollIntoView(listBox.Items[listBox.Items.Count - 1]);
                            }
                        };
                    }
                };
            }
        }
    }
}
