using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace S7SvrSim.UserControls.Signals
{
    public class DescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            Type valueType;
            if (value is not Type)
            {
                valueType = value.GetType();
            }
            else
            {
                valueType = (Type)value;
            }

            return valueType.GetCustomAttribute<DescriptionAttribute>()?.Description;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
