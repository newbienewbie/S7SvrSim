using System;
using System.Globalization;
using System.Windows.Data;

namespace S7SvrSim.Shared
{
    public class IsTypeToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            if (parameter is not Type)
            {
                return false;
            }

            return value.GetType().IsSubclassOf((Type)parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
