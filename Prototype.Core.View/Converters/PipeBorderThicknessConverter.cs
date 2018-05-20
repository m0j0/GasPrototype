using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Prototype.Core.Controls;

namespace Prototype.Core.Converters
{
  internal  class PipeBorderThicknessConverter : IValueConverter
    {
        public static readonly PipeBorderThicknessConverter Instance = new PipeBorderThicknessConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case BridgeSegment _:
                    return new Thickness(0);

                case ConnectorSegment connectorSegment:
                    var side = connectorSegment.Side;
                    return new Thickness(
                        side.HasFlagEx(Side.Left) ? 1 : 0,
                        side.HasFlagEx(Side.Top) ? 1 : 0,
                        side.HasFlagEx(Side.Right) ? 1 : 0,
                        side.HasFlagEx(Side.Bottom) ? 1 : 0
                    );

                case LinePipeSegment linePipeSegment:
                    return linePipeSegment.Orientation ==
                        Orientation.Horizontal ? new Thickness(0, 1, 0, 1) : new Thickness(1, 0, 1, 0);
            }

            return new Thickness(1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
