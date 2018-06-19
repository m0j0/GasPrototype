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
        public BridgePipeConnector(Rect rect, GraphPipe pipe)
        {
            if (pipe == null)
            {
                throw new Exception("!!!");
            }

            Rect = rect;
            Pipe = pipe;
            pipe.AddConnector(this);
        }

        public Rect Rect { get; }

        public GraphPipe Pipe { get; }

        public IPipeSegment CreateSegment(GraphPipe pipe)
        {
            if (pipe != Pipe)
            {
                throw new ArgumentException("!!!");
            }

            return new BridgeSegment(
                new Point(
                    Rect.Left - pipe.Rect.Left - Common.GetBridgeHorizontalConnectorOffset(pipe.Orientation),
                    Rect.Top - pipe.Rect.Top - Common.GetBridgeVerticalConnectorOffset(pipe.Orientation)),
                pipe.Orientation
            );
        }

        public override string ToString()
        {
            return $"Bridge {Rect}";
        }
    }

    internal class PipeConnector : IPipeConnector
    {
        private readonly GraphPipe[] _pipes;

        public PipeConnector(Rect rect, params GraphPipe[] pipes)
        {
            Rect = rect;
            _pipes = new GraphPipe[4];

            foreach (var pipe in pipes)
            {
                AddPipe(pipe);
            }
        }

        public Rect Rect { get; }

        public void AddPipe(GraphPipe pipe)
        {
            if (pipe.Connectors.Contains(this))
            {
                return;
            }

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

            pipe.AddConnector(this);
        }

        public bool IsEndConnector()
        {
            return _pipes.Count(pipe => pipe != null) == 1;
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