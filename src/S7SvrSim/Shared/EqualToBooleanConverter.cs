using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace S7SvrSim.Shared
{
    public class EqualToBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0) return false;
            if (values.Length == 1) return true;

            var first = values.First();

            return values.Skip(1).All(first.Equals);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
