using System.Collections.Generic;
using System.Windows.Threading;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    internal sealed class SchemeContainer : ISchemeContainer
    {
        private readonly List<IFlowControl> _controls = new List<IFlowControl>();

        private FlowGraph _scheme;
        private bool _isInvalidateCalled;

        public void Add(IFlowControl flowControl)
        {
            _controls.Add(flowControl);
            InvalidateScheme();
        }

        public void Remove(IFlowControl flowControl)
        {
            _controls.Remove(flowControl);
            InvalidateScheme();
        }

        public void InvalidateScheme()
        {
            if (_isInvalidateCalled)
            {
                return;
            }

            // hack to avoid massive pack of calls to one
            _isInvalidateCalled = true;
            Dispatcher.CurrentDispatcher.InvokeAsync(InvalidateSchemeImpl, DispatcherPriority.Send);
        }

        public void InvalidateSchemeFlow()
        {
            _scheme?.InvalidateFlow();
        }

        private void InvalidateSchemeImpl()
        {
            _isInvalidateCalled = false;

            var pipes = new List<IPipe>();
            var valves = new List<IValve>();

            foreach (var flowControl in _controls)
            {
                switch (flowControl)
                {
                    case IPipe pipe:
                        pipes.Add(pipe);
                        break;

                    case IValve valve:
                        valves.Add(valve);
                        break;
                }
            }

            if (_scheme != null && _scheme.Equals(pipes, valves))
            {
                return;
            }

            _scheme = new FlowGraph(this, pipes, valves);
        }
    }
}