using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DynamicDocsWPF.HelperClasses
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b ? new SolidColorBrush(Color.FromRgb(183, 183, 183)) : new SolidColorBrush(Colors.Transparent);
            
            
            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}