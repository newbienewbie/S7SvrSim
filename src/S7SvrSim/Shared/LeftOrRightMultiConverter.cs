using System;
using System.Globalization;
using System.Windows.Data;

namespace S7SvrSim.Shared
{
    public class LeftOrRightMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null) return null;
            if (values.Length == 0) return null;
            if (values[0] is not bool) throw new ArgumentException("First binding is condition and need be Boolean!");

            var condition = (bool)values[0];
            // left
            if (condition && values.Length >= 2) return values[1];
            // right
            if (!condition && values.Length >= 3) return values[2];

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
