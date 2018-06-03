using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    internal interface IPipeConnector
    {
        Rect Rect { get; }

        IPipeSegment CreateSegment(GraphPipe pipe);
    }

    internal class BridgePipeConnector : IPipeConnector
    {
        public BridgePipeConnector(Rect rect, GraphPipe pipe1, GraphPipe pipe2)
        {
            if (pipe1 == null || pipe2 == null)
            {
                throw new Exception("!!!");
            }

            Rect = rect;
            Pipe1 = pipe1;
            Pipe2 = pipe2;
            pipe1.Connectors.Add(this);
            pipe2.Connectors.Add(this);
        }

        public Rect Rect { get; }

        public GraphPipe Pipe1 { get; }

        public GraphPipe Pipe2 { get; }

        public IPipeSegment CreateSegment(GraphPipe pipe)
        {
            if (pipe == Pipe1)
            {
                return new BridgeSegment(
                    new Point(
                        Rect.Left - pipe.Rect.Left - Common.GetBridgeHorizontalConnectorOffset(pipe.Orientation),
                        Rect.Top - pipe.Rect.Top - Common.GetBridgeVerticalConnectorOffset(pipe.Orientation)),
                    pipe.Orientation
                );
            }

            if (pipe == Pipe2)
            {
                return new LineSegment(
                    new Point(Rect.Left - pipe.Rect.Left, Rect.Top - pipe.Rect.Top),
                    pipe.Orientation == Orientation.Horizontal ? Rect.Right - Rect.Left : Rect.Bottom - Rect.Top,
                    pipe.Orientation);
            }

            throw new ArgumentException("!!!");
        }

        public override string ToString()
        {
            return $"Bridge {Rect}";
        }
    }

    internal class PipeConnector : IPipeConnector
    {
        private readonly GraphPipe[] _pipes;

        public PipeConnector(Rect rect)
        {
            Rect = rect;
            _pipes = new GraphPipe[4];
        }

        public Rect Rect { get; }

        public IVertex Vertex { get; set; }
        
        public GraphPipe Pipe1 => _pipes[0];

        public GraphPipe Pipe2 => _pipes[1];

        public GraphPipe Pipe3 => _pipes[2];

        public GraphPipe Pipe4 => _pipes[3];

        public void AddPipe(GraphPipe pipe)
        {
            if (pipe.Connectors.Contains(this))
            {
                return;
            }

            pipe.Connectors.Add(this);

            bool set = false;
            for (int i = 0; i < _pipes.Length; i++)
            {
                if (_pipes[i] == null)
                {
                    _pipes[i] = pipe;
                    set = true;
                    break;
                }
            }

            if (!set)
            {
                throw new Exception("!!!");
            }
        }

        public IEnumerable<GraphPipe> GetPipes()
        {
            return _pipes.Where(pipe => pipe != null);
        }

        public IPipeSegment CreateSegment(GraphPipe pipe)
        {
            if (!_pipes.Contains(pipe))
            {
                throw new ArgumentException("!!!");
            }

            var side = Side.All;
            if (_pipes.Any(p => p != null &&
                                p.Rect.Left < Rect.Left))
            {
                side = side & ~Side.Left;
            }

            if (_pipes.Any(p => p != null &&
                                p.Rect.Top < Rect.Top))
            {
                side = side & ~Side.Top;
            }

            if (_pipes.Any(p => p != null &&
                                p.Rect.Right > Rect.Right))
            {
                side = side & ~Side.Right;
            }

            if (_pipes.Any(p => p != null &&
                                p.Rect.Bottom > Rect.Bottom))
            {
                side = side & ~Side.Bottom;
            }

            return new ConnectorSegment(
                new Point(Rect.Left - pipe.Rect.Left, Rect.Top - pipe.Rect.Top),
                pipe.Orientation,
                side
            );
        }

        public override string ToString()
        {
            return $"Corner {Rect}";
        }
    }
}