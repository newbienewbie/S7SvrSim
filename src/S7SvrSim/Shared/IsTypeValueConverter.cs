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
            if (targetType == typeof(bool))
            {
                if (value == null)
                {
                    return false;
                }

                return value.GetType() == Type;
            }
            else if (targetType == typeof(Visibility))
            {
                if (value == null)
                {
                    return Visibility.Hidden;
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

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
