using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Prototype.Core.Controls
{
    internal interface IConnector
    {
        Rect Rect { get; }

        IPipeSegment CreateSegment(ProcessPipe pipe);
    }

    internal class BridgeConnector : IConnector
    {
        public BridgeConnector(Rect rect)
        {
            Rect = rect;
        }

        public Rect Rect { get; }

        public ProcessPipe Pipe1 { get; private set; }
        public ProcessPipe Pipe2 { get; private set; }

        public void AddPipes(ProcessPipe pipe1, ProcessPipe pipe2)
        {
            if (Pipe1 != null || Pipe2 != null)
            {
                throw new Exception("!!!");
            }

            if (pipe1 == null || pipe2 == null)
            {
                throw new Exception("!!!");
            }

            if (pipe1.Connectors.Contains(this))
            {
                throw new Exception("!!");
            }

            if (pipe2.Connectors.Contains(this))
            {
                throw new Exception("!!");
            }

            Pipe1 = pipe1;
            pipe1.Connectors.Add(this);

            Pipe2 = pipe2;
            pipe2.Connectors.Add(this);
        }

        public IPipeSegment CreateSegment(ProcessPipe pipe)
        {
            if (Pipe1 != pipe && Pipe2 != pipe)
            {
                throw new ArgumentException("!!!");
            }

            return new BridgeSegment(
                new Point(Rect.Left - pipe.Rect.Left, Rect.Top - pipe.Rect.Top),
                pipe.Orientation
            );
        }
    }

    internal class CornerConnector : IConnector
    {
        private readonly ProcessPipe[] _pipes;

        public CornerConnector(Rect rect)
        {
            Rect = rect;
            _pipes = new ProcessPipe[4];
        }

        public Rect Rect { get; }

        public ProcessPipe Pipe1 => _pipes[0];
        public ProcessPipe Pipe2 => _pipes[1];
        public ProcessPipe Pipe3 => _pipes[2];
        public ProcessPipe Pipe4 => _pipes[3];

        public void AddPipe(ProcessPipe pipe)
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

        public IPipeSegment CreateSegment(ProcessPipe pipe)
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
    }
}