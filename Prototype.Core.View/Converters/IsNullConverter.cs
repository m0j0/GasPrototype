using System;
using System.Globalization;
using System.Windows.Data;

namespace Prototype.Core.Converters
{
    public sealed class IsNullConverter : IValueConverter
    {
        public static readonly IsNullConverter Instance = new IsNullConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class IsNotNullConverter : IValueConverter
    {
        public static readonly IsNotNullConverter Instance = new IsNotNullConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}