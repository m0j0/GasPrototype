using System;
using System.Globalization;
using System.Windows.Data;
using Prototype.Infrastructure;

namespace Prototype.Converters
{
    public class AccessConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var t = (Access) value;
            var p = (Access) parameter;
            return t == p;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }

        #endregion
    }
}
