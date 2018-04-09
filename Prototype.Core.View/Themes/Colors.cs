using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Prototype.Core.Themes
{
    public static class AmatColors
    {
        #region Fields

        private static readonly Dictionary<Color, Brush> SolidColorBrushes = new Dictionary<Color, Brush>();
        private static readonly Dictionary<Color, Brush> HatchedColorBrushes = new Dictionary<Color, Brush>();

        public static readonly Color DarkBlue;

        public static readonly Color MediumBlue1;

        public static readonly Color MediumBlue2;

        public static readonly Color MediumBlue3;

        public static readonly Color LightBlue1;

        public static readonly Color LightBlue2;

        public static readonly Color LightBlue3;

        public static readonly Color DarkGreen;

        public static readonly Color Red;

        public static readonly Color Blue;

        public static readonly Color Yellow;

        public static readonly Color Green;

        public static readonly Color LightRed;

        public static readonly Color LightBlue;

        public static readonly Color Orange;

        public static readonly Color LightGreen;

        public static readonly Color DarkGray;

        public static readonly Color MediumGray1;

        public static readonly Color MediumGray2;

        public static readonly Color LightGray;

        public static readonly Color Blue1;

        public static readonly Color MediumBlue;

        public static readonly Color Beige;

        public static readonly Color Magenta;

        #endregion

        #region Constructors

        static AmatColors()
        {
            var resources = Application.Current.Resources;
            DarkBlue = (Color) resources["AmatColorDarkBlue"];
            MediumBlue1 = (Color) resources["AmatColorMediumBlue1"];
            MediumBlue2 = (Color) resources["AmatColorMediumBlue2"];
            MediumBlue3 = (Color) resources["AmatColorMediumBlue3"];
            LightBlue1 = (Color) resources["AmatColorLightBlue1"];
            LightBlue2 = (Color) resources["AmatColorLightBlue2"];
            LightBlue3 = (Color) resources["AmatColorLightBlue3"];
            DarkGreen = (Color) resources["AmatColorDarkGreen"];
            Red = (Color) resources["AmatColorRed"];
            Blue = (Color) resources["AmatColorBlue"];
            Yellow = (Color) resources["AmatColorYellow"];
            Green = (Color) resources["AmatColorGreen"];
            LightRed = (Color) resources["AmatColorLightRed"];
            LightBlue = (Color) resources["AmatColorLightBlue"];
            Orange = (Color) resources["AmatColorOrange"];
            LightGreen = (Color) resources["AmatColorLightGreen"];
            DarkGray = (Color) resources["AmatColorDarkGray"];
            MediumGray1 = (Color) resources["AmatColorMediumGray1"];
            MediumGray2 = (Color) resources["AmatColorMediumGray2"];
            LightGray = (Color) resources["AmatColorLightGray"];
            Blue1 = (Color) resources["AmatColorBlue1"];
            MediumBlue = (Color) resources["AmatColorMediumBlue"];
            Beige = (Color) resources["AmatColorBeige"];
            Magenta = (Color) resources["AmatColorMagenta"];
        }

        #endregion

        #region Methods

        public static Brush GetSolidColorBrush(Color color)
        {
            Brush brush;
            if (!SolidColorBrushes.TryGetValue(color, out brush))
            {
                brush = new SolidColorBrush(color);
                SolidColorBrushes[color] = brush;
            }
            return SolidColorBrushes[color];
        }

        public static Brush GetHatchedColorBrush(Color color)
        {
            Brush brush;
            if (!HatchedColorBrushes.TryGetValue(color, out brush))
            {
                brush = (Brush) XamlReader.Parse(string.Format(@"     <VisualBrush xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
      TileMode=""Tile"" Viewport=""0,0,10,10"" 
      ViewportUnits=""Absolute"" Viewbox=""0,0,10,10""    
      ViewboxUnits=""Absolute"">
      <VisualBrush.Visual>
        <Canvas>
           <Rectangle Fill=""{0}"" Width=""10"" Height=""10"" />
           <Path Stroke=""{1}"" Data=""M 0 10 l 10 -10"" />
        </Canvas>
      </VisualBrush.Visual>
   </VisualBrush>   ", color, Color.FromArgb((byte) (255 * 0.2), 0, 0, 0)));
                HatchedColorBrushes[color] = brush;
            }
            return HatchedColorBrushes[color];
        }

        #endregion
    }
}