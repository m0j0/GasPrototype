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
    internal sealed class FlowDirectionToColorConverter : IValueConverter
    {
        public static readonly FlowDirectionToColorConverter Instance = new FlowDirectionToColorConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var flowDirection = (FlowDirection) value;
            switch (flowDirection)
            {
                case FlowDirection.None:
                    return new SolidColorBrush(Color.FromRgb(0x8B, 0x8D, 0x8E));
                case FlowDirection.Forward:
                case FlowDirection.Backward:
                case FlowDirection.Both:
                    return new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0xFF));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}