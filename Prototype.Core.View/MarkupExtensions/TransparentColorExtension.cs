using System;
using System.Windows.Markup;
using System.Windows.Media;

namespace Prototype.Core.MarkupExtensions
{
    public sealed class TransparentColorExtension : MarkupExtension
    {
        #region Fields

        private readonly Color _blendedColor;

        #endregion

        #region Constructors

        public TransparentColorExtension(Color color, double opacity) : this(color, Colors.White, opacity)
        {
        }

        public TransparentColorExtension(Color color, Color baseColor, double opacity)
        {
            if (opacity < 0 || opacity > 1)
            {
                throw new ArgumentException("Opacity should be in [0; 1] interval");
            }

            _blendedColor = BlendColor(color, baseColor, opacity);
        }

        #endregion

        #region Methods

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _blendedColor;
        }

        // https://stackoverflow.com/a/12228643
        private static Color BlendColor(Color color, Color backColor, double opacity)
        {
            return Color.FromRgb(
                (byte)(backColor.R + (color.R - backColor.R) * opacity),
                (byte)(backColor.G + (color.G - backColor.G) * opacity),
                (byte)(backColor.B + (color.B - backColor.B) * opacity));
        }

        #endregion
    }
}
