using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DynamicDocsWPF.HelperClasses
{
    public class RevBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b ? Visibility.Hidden : Visibility.Visible;


            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}