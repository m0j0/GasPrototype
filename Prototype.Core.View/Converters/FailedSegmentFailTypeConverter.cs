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
        private static readonly Dictionary<FailType, Brush> Brushes;

        static FailedSegmentFailTypeConverter()
        {
            Brushes = new Dictionary<FailType, Brush>
            {
                [FailType.WrongSize] = new SolidColorBrush(Colors.Red),
                [FailType.IntersectionIsNotSupported] = new SolidColorBrush(Colors.Green),
                [FailType.BridgeNotEnoughSpace] = new SolidColorBrush(Colors.Blue),
                [FailType.DeadPath] = new SolidColorBrush(Colors.Black),
                [FailType.BothSourceDestination] = new SolidColorBrush(Colors.Yellow),
            };
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var failType = (FailType) value;
            if (Brushes.TryGetValue(failType, out Brush brush))
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