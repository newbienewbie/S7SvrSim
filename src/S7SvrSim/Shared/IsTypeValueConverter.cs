using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace S7SvrSim.Shared
{
    public class IsTypeValueConverter : IValueConverter
    {
        public Type Type { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            if (value.GetType() == Type)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
