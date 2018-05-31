using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Prototype.Core.Controls.PipeFlowScheme;

namespace Prototype.Core.Converters
{
    internal class FailedSegmentFailTypeConverter : IValueConverter
    {
        public static readonly FailedSegmentFailTypeConverter Instance = new FailedSegmentFailTypeConverter();

        private static Dictionary<FailType, Brush> _brushes;

        private FailedSegmentFailTypeConverter()
        {
            _brushes = new Dictionary<FailType, Brush>
            {
                [FailType.WrongSize] = new SolidColorBrush(Colors.Red),
                [FailType.IntersectionNotSupported] = new SolidColorBrush(Colors.Green),
                [FailType.BridgeNotEnoughSpace] = new SolidColorBrush(Colors.Blue),
            };
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var failType = (FailType) value;
            if (_brushes.TryGetValue(failType, out Brush brush))
            {
                return brush;
            }

            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}