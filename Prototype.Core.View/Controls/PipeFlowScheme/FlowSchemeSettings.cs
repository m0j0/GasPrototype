using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    public static class FlowSchemeSettings
    {
        public static readonly DependencyProperty ShowFlowProperty = DependencyProperty.RegisterAttached(
            "ShowFlow", typeof(bool), typeof(FlowSchemeSettings), new PropertyMetadata(default(bool)));

        public static void SetShowFlow(DependencyObject element, bool value)
        {
            element.SetValue(ShowFlowProperty, value);
        }

        public static bool GetShowFlow(DependencyObject element)
        {
            return (bool) element.GetValue(ShowFlowProperty);
        }

        internal static readonly DependencyProperty ContainerProperty = DependencyProperty.RegisterAttached(
            "Container", typeof(SchemeContainer2), typeof(FlowSchemeSettings), new PropertyMetadata(default(SchemeContainer2)));

        internal static void SetContainer(DependencyObject element, SchemeContainer2 value)
        {
            element.SetValue(ContainerProperty, value);
        }

        internal static SchemeContainer2 GetContainer(DependencyObject element)
        {
            return (SchemeContainer2) element.GetValue(ContainerProperty);
        }
    }

    public sealed class SchemeContainer2 : ISchemeContainer
    {
        private List<IFlowControl> _controls = new List<IFlowControl>();

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

        public void InvalidateIfNotExists(IFlowControl flowControl)
        {

        }

        public void InvalidateScheme()
        {
            if (_isInvalidateCalled)
            {
                return;
            }

            // hack to avoid massive pack of calls to one
            _isInvalidateCalled = true;
            Dispatcher.CurrentDispatcher.InvokeAsync(InvalidateSchemeImpl, DispatcherPriority.Background);
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
                flowControl.SchemeContainer = this;
                if (flowControl is IPipe pipe)
                {
                    pipes.Add(pipe);
                }
                if (flowControl is IValve valve)
                {
                    valves.Add(valve);
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