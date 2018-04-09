using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Prototype.Core.Models;
using Prototype.Core.Themes;

namespace Prototype.Core.Converters
{
    public class WaferStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (WaferStatus) value;
            return GetWaferColor(status);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static Color GetWaferColor(WaferStatus status)
        {
            switch (status)
            {
                case WaferStatus.NotPresentUnresolved:
                    return AmatColors.Beige;
                case WaferStatus.NotPresentResolved:
                    return Colors.Transparent;
                case WaferStatus.Unknown:
                    return AmatColors.MediumBlue1;
                case WaferStatus.Unprocessed:
                    return Colors.White;
                case WaferStatus.InProcess:
                    return AmatColors.LightBlue;
                case WaferStatus.PartialProcessed:
                    return AmatColors.Magenta;
                case WaferStatus.Processed:
                    return AmatColors.Green;
                case WaferStatus.FailureDuringProcess:
                    return AmatColors.Red;
                case WaferStatus.WarningDuringProcess:
                    return AmatColors.Yellow;
                case WaferStatus.PositionUnresolved:
                    return AmatColors.Orange;
                case WaferStatus.LotNotAssigned:
                    return AmatColors.Blue;
                case WaferStatus.DummyUnprocessed:
                    return Colors.White;
                case WaferStatus.DummyInProcess:
                    return AmatColors.LightBlue;
                case WaferStatus.DummyFault:
                    return AmatColors.Red;
                case WaferStatus.DummyWarning:
                    return AmatColors.Yellow;
                case WaferStatus.DummyUnknown:
                    return AmatColors.MediumBlue1;
                default:
                    return AmatColors.MediumGray1;
            }
        }
    }

    public class WaferStatusSolidBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (WaferStatus) value;
            return AmatColors.GetSolidColorBrush(WaferStatusColorConverter.GetWaferColor(status));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class WaferStatusHatchedBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (WaferStatus) value;
            return AmatColors.GetHatchedColorBrush(WaferStatusColorConverter.GetWaferColor(status));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
