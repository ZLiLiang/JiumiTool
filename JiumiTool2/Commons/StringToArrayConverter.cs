using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace JiumiTool2.Commons
{
    public class StringToArrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                return new ObservableCollection<char>(text);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
