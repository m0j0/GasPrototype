using System;
using System.Globalization;
using System.Windows.Data;

namespace Prototype.Core.Converters
{
    public sealed class EnumEqualityConverter : IValueConverter
    {
        public static readonly EnumEqualityConverter Instance = new EnumEqualityConverter();
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Equals(value, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }
}