using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using MugenMvvmToolkit;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    internal sealed class SchemeContainer : ISchemeContainer
    {
        private readonly FrameworkElement _containerOwner;
        private FlowGraph _scheme;
        private bool _isInvalidateCalled;

        public SchemeContainer(FrameworkElement containerOwner)
        {
            Should.NotBeNull(containerOwner, nameof(containerOwner));
            _containerOwner = containerOwner;

            InvalidateSchemeImpl();
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

            var controls = new List<IFlowControl>();
            GatherChildrenFlowControls(_containerOwner, controls);

            var pipes = new List<IPipe>();
            var valves = new List<IValve>();

            foreach (var flowControl in controls)
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

            _scheme = new FlowGraph(pipes, valves, FlowSchemeSettings.GetMarkDeadPaths(_containerOwner));
        }

        private void GatherChildrenFlowControls(FrameworkElement containerOwner, List<IFlowControl> flowControls)
        {
            var li = LayoutInformation.GetLayoutSlot(containerOwner);
            var offset = new Vector(-li.X, -li.Y);
            ProcessChildren(containerOwner, offset, flowControls);
        }

        private void ProcessChildren(FrameworkElement element, Vector offset, List<IFlowControl> flowControls)
        {
            if (element == null)
            {
                return;
            }

            var li = LayoutInformation.GetLayoutSlot(element);
            offset.X += li.X;
            offset.Y += li.Y;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

                if (child is IFlowControl flowControl)
                {
                    flowControl.SetContrainer(this, offset);
                    flowControls.Add(flowControl);
                }

                ProcessChildren(child, offset, flowControls);
            }
        }
    }
}