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

        #endregion
    }
}