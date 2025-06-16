using S7SvrSim.S7Signal;
using System;
using System.Globalization;
using System.Windows.Data;

namespace S7SvrSim.Shared
{
    public class AddressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return (SignalAddress)null;
            }

            if (value is string str)
            {
                return new SignalAddress(str);
            }

            return value;
        }
    }
}
