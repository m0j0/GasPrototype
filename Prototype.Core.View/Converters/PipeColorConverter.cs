using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Prototype.Core.Controls.PipeFlowScheme;

namespace Prototype.Core.Converters
{
    internal sealed class PipeColorConverter : IMultiValueConverter
    {
        private static readonly Brush HasFlowBrush = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0xFF));
        private static readonly Brush GasBrush = new SolidColorBrush(Color.FromRgb(0x8B, 0x8D, 0x8E));
        private static readonly Brush PurgeBrush = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
        private static readonly Brush ChemicalBrush = new SolidColorBrush(Color.FromRgb(0xFF, 0x9B, 0xFF));

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var hasFlow = (bool) values[1];
            if (hasFlow)
            {
                return HasFlowBrush;
            }

            var substanceType = (SubstanceType) values[0];
            switch (substanceType)
            {
                case SubstanceType.Gas:
                    return GasBrush;
                case SubstanceType.Purge:
                    return PurgeBrush;
                case SubstanceType.Chemical:
                    return ChemicalBrush;
                default:
                    throw new ArgumentOutOfRangeException("substanceType");
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}