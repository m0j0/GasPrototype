using System;
using System.Collections.Generic;
using System.Linq;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    public sealed class PipeFlowScheme
    {
        private readonly IContainer _container;
        private readonly List<IPipe> _pipes;
        private readonly List<IValve> _valves;

        public PipeFlowScheme(IContainer container)
        {
            _container = container;
            _pipes = new List<IPipe>();
            _valves = new List<IValve>();
        }

        public IReadOnlyCollection<IPipe> Pipes => _pipes;

        public IReadOnlyCollection<IValve> Valves => _valves;

        public void Add(IFlowControl control)
        {
            switch (control)
            {
                case IPipe pipe:
                    _pipes.Add(pipe);
                    break;

                case IValve valve:
                    _valves.Add(valve);
                    break;

                default:
                    throw new NotSupportedException();
            }

            Invalidate();
        }

        public void Add(IEnumerable<IFlowControl> controls)
        {
            foreach (var flowControl in controls)
            {
                switch (flowControl)
                {
                    case IPipe pipe:
                        pipe.SizeChanged += (sender, args) => Invalidate();
                        _pipes.Add(pipe);
                        break;

                    case IValve valve:
                        _valves.Add(valve);
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }
            
            Invalidate();
        }

        public void Remove(IFlowControl control)
        {

        }

        public void Invalidate()
        {
            foreach (var pipe in _pipes)
            {
                pipe.Segments = PipeDrawing.SplitPipeToSegments(_container, pipe, _pipes).ToList();
            }
        }
    }
}
