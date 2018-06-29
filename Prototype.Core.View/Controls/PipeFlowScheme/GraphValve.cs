using System.Windows;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    internal class GraphValve
    {
        public GraphValve(ISchemeContainer container, IValve valve)
        {
            Rect = Common.GetAbsoluteRect(container, valve);
            Valve = valve;
        }

        public IValve Valve { get; }

        public Rect Rect { get; }

        public IPipeConnector Connector { get; set; }

        public bool Equals(ISchemeContainer container, IValve valve)
        {
            if (Valve != valve)
            {
                return false;
            }

            if (Rect != Common.GetAbsoluteRect(container, valve))
            {
                return false;
            }

            return true;
        }
    }
}