using System;
using System.Collections.Generic;
using System.Linq;
using MugenMvvmToolkit;

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
            AddInternal(control);

            Invalidate();
        }

        private void AddInternal(IFlowControl control)
        {
            Should.NotBeNull(control, nameof(control));
            control.SizeChanged += (sender, args) => Invalidate();
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
        }

        public void Add(IReadOnlyCollection<IFlowControl> controls)
        {
            Should.NotBeNull(controls, nameof(controls));

            if (controls.Count == 0)
            {
                return;
            }

            foreach (var flowControl in controls)
            {
                AddInternal(flowControl);
            }

            Invalidate();
        }

        public bool Contains(IFlowControl flowControl)
        {
            return _pipes.Contains(flowControl) || _valves.Contains(flowControl);
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