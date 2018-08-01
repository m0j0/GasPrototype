using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Prototype.Core.Controls.PipeFlowScheme;

namespace Prototype.Core.Converters
{
    internal class PipeConnectorBorderThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var side = (Side) value;
            return new Thickness(
                side.HasFlagEx(Side.Left) ? 1 : 0,
                side.HasFlagEx(Side.Top) ? 1 : 0,
                side.HasFlagEx(Side.Right) ? 1 : 0,
                side.HasFlagEx(Side.Bottom) ? 1 : 0
            );
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}