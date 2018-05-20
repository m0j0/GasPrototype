using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Prototype.Core.Controls
{
    internal class ProcessPipe
    {
        public ProcessPipe(Pipe pipe)
        {
            Rect = new Rect(Canvas.GetLeft(pipe), Canvas.GetTop(pipe), pipe.Width, pipe.Height);
            Pipe = pipe;
            Segments = new List<IPipeSegment>();
            Connectors = new List<IConnector>();
        }

        public Pipe Pipe { get; }
        public Rect Rect { get; }
        public Orientation Orientation => Pipe.Orientation;

        public List<IPipeSegment> Segments { get; }

        public List<IConnector> Connectors { get; }

        public override string ToString()
        {
            return $"{Rect} {Orientation}";
        }
    }
}