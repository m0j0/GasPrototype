using System.Windows;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    internal class GraphValve
    {
        public GraphValve(IValve valve)
        {
            Rect = new Rect(valve.LayoutRect.Location + valve.Offset, valve.LayoutRect.Size);
            IsVisible = valve.IsVisible;
            Valve = valve;
        }

        public IValve Valve { get; }

        public Rect Rect { get; }

        public IPipeConnector Connector { get; set; }

        public bool IsVisible { get; }

        public bool Equals(IValve valve)
        {
            if (Valve != valve)
            {
                return false;
            }

            if (IsVisible != valve.IsVisible)
            {
                return false;
            }

            if (Rect != valve.LayoutRect)
            {
                return false;
            }

            return true;
        }
    }
}