using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    internal class ProcessPipe
    {
        public ProcessPipe(IContainer container, IPipe pipe)
        {
            Rect = new Rect(container.GetLeft(pipe), container.GetTop(pipe), pipe.Width, pipe.Height);
            Pipe = pipe;
            Segments = new List<IPipeSegment>();
            Connectors = new List<IConnector>();
        }

        public IPipe Pipe { get; }
        public Rect Rect { get; }
        public Orientation Orientation => Pipe.Orientation;

        public bool IsFailed { get; set; }

        public List<IPipeSegment> Segments { get; }

        public List<IConnector> Connectors { get; }

        public override string ToString()
        {
            return $"{Rect} {Orientation}";
        }
    }
}