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
            _container.InvalidateRequired += OnInvalidateRequired;
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
            Should.NotBeNull(control, nameof(control));
            control.InvalidateRequired -= OnInvalidateRequired;
            switch (control)
            {
                case IPipe pipe:
                    _pipes.Remove(pipe);
                    break;

                case IValve valve:
                    _valves.Remove(valve);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        private void Invalidate()
        {
            PipeDrawing.SplitPipeToSegments(_container, _pipes);
        }

        private void AddInternal(IFlowControl control)
        {
            Should.NotBeNull(control, nameof(control));
            control.InvalidateRequired += OnInvalidateRequired;
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

        private void OnInvalidateRequired(object sender, EventArgs e)
        {
            Invalidate();
        }
    }
}