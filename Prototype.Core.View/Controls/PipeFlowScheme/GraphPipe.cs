using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    internal class GraphPipe
    {
        public GraphPipe(ISchemeContainer container, IPipe pipe)
        {
            Rect = Common.GetAbsoluteRect(container, pipe);
            Pipe = pipe;
            Segments = new List<IPipeSegment>();
            Connectors = new List<IPipeConnector>();
        }

        public IPipe Pipe { get; }

        public Rect Rect { get; }

        public Orientation Orientation => Pipe.Orientation;

        public FailType FailType { get; set; }

        public bool IsFailed => FailType != FailType.None;

        public IList<IPipeSegment> Segments { get; }

        public IList<IPipeConnector> Connectors { get; }

        public PipeConnector StartConnector { get; set; }

        public PipeConnector EndConnector { get; set; }
    }
}