using System;
using System.Globalization;
using System.Windows.Data;

namespace S7SvrSim.Shared
{
    public class EqualToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;
            if (value != null && parameter == null) return true;
            if (value.GetType() != parameter.GetType()) return false;

            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
