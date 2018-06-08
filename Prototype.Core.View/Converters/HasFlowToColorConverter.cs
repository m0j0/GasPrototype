using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Prototype.Core.Controls.PipeFlowScheme;

namespace Prototype.Core.Converters
{
    internal sealed class HasFlowToColorConverter : IValueConverter
    {
        public static readonly HasFlowToColorConverter Instance = new HasFlowToColorConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var hasFlow = (bool) value;
            if (hasFlow)
            {
                return new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0xFF));
            }

            return new SolidColorBrush(Color.FromRgb(0x8B, 0x8D, 0x8E));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}