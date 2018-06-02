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
    }
}