using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    internal class ProcessPipe
    {
        public ProcessPipe(IContainer container, IPipe pipe)
        {
            Rect = Common.GetAbsoluteRect(container, pipe);
            Pipe = pipe;
            Segments = new List<IPipeSegment>();
            Connectors = new List<IConnector>();
        }

        public IPipe Pipe { get; }

        public Rect Rect { get; }

        public Orientation Orientation => Pipe.Orientation;

        public FailType FailType { get; set; }

        public bool IsFailed => FailType != FailType.None;

        public List<IPipeSegment> Segments { get; }

        public List<IConnector> Connectors { get; }
    }

    internal class ProcessValve
    {
        public ProcessValve(IContainer container, IValve valve)
        {
            Rect = Common.GetAbsoluteRect(container, valve);
            Valve = valve;
        }

        public IValve Valve { get; }

        public Rect Rect { get; }
    }
}