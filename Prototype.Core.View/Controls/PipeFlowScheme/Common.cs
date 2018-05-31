using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    internal static class Common
    {
        #region Constants

        internal const double PipeWidth = 5;
        internal const double BridgeLength = 27;
        internal const double BridgeOffset = (BridgeLength - PipeWidth) / 2;
        internal static Vector ConnectorVector = new Vector(PipeWidth, PipeWidth);

        #endregion

        #region Methods

        internal static double GetBridgeHorizontalConnectorOffset(Orientation pipeOrientation)
        {
            return pipeOrientation == Orientation.Horizontal ? BridgeOffset : 0;
        }

        internal static double GetBridgeVerticalConnectorOffset(Orientation pipeOrientation)
        {
            return pipeOrientation == Orientation.Horizontal ? 0 : BridgeOffset;
        }

        internal static bool IsSizeValid(ProcessPipe pipe)
        {
            return IsSizeValid(pipe.Rect, pipe.Orientation);
        }

        internal static bool IsSizeValid(Rect rect, Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.Horizontal:
                    return rect.Height == PipeWidth && rect.Width > 0;
                case Orientation.Vertical:
                    return rect.Width == PipeWidth && rect.Height > 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
        }

        internal static bool IsIntersectionSizeValid(Rect intersectionRect)
        {
            return intersectionRect.Width == Common.PipeWidth &&
                   intersectionRect.Height == Common.PipeWidth;
        }

        internal static Rect GetAbsoluteRect(ISchemeContainer container, IFlowControl control)
        {
            return new Rect(container.GetLeft(control), container.GetTop(control), control.Width, control.Height);
        }

        internal static double GetLength(Rect rect, Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.Horizontal:
                    return rect.Width;
                case Orientation.Vertical:
                    return rect.Height;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
        }

        #endregion
    }
}