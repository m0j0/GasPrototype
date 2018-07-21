using System.Windows;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    internal class GraphValve
    {
        public GraphValve(ISchemeContainer container, IValve valve)
        {
            Rect = new Rect(valve.LayoutRect.Location + valve.Offset, valve.LayoutRect.Size);
            Valve = valve;
        }

        public IValve Valve { get; }

        public Rect Rect { get; }

        public IPipeConnector Connector { get; set; }

        public bool Equals(IValve valve)
        {
            if (Valve != valve)
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