using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MugenMvvmToolkit;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    public sealed class PipeFlowCanvas : Canvas, ISchemeContainerOwner
    {
        private readonly SchemeContainer _schemeContainer;

        public PipeFlowCanvas()
        {
            _schemeContainer = new SchemeContainer(this);
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        public IEnumerable<IFlowControl> ChildrenFlowControls => Children.OfType<IFlowControl>();

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            var addedFlowControl = visualAdded as IFlowControl;
            var removedFlowControl = visualRemoved as IFlowControl;

            if (addedFlowControl != null || removedFlowControl != null)
            {
                _schemeContainer.OnVisualChildrenChanged(addedFlowControl, removedFlowControl);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _schemeContainer.OnLoaded();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _schemeContainer.OnUnloaded();
        }
    }

    public sealed class SchemeContainer : ISchemeContainer
    {
        private static readonly EventHandler OnFlowControlSchemeChangedEventHandler = OnFlowControlSchemeChanged;
        private static readonly EventHandler OnValveStateChangedEventHandler = OnValveStateChanged;

        private readonly ISchemeContainerOwner _owner;
        private FlowGraph _scheme;
        private bool _isInvalidateCalled;

        public SchemeContainer(ISchemeContainerOwner owner)
        {
            Should.NotBeNull(owner, nameof(owner));
            _owner = owner;
        }

        public void OnVisualChildrenChanged(IFlowControl visualAdded, IFlowControl visualRemoved)
        {
            if (!_owner.IsLoaded)
            {
                return;
            }

            if (visualAdded != null)
            {
                SubscribePositionChangedEvents(visualAdded);
            }

            if (visualRemoved != null)
            {
                UnsubscribePositionChangedEvents(visualRemoved);
            }

            InvalidateScheme(false);
        }

        public void OnLoaded()
        {
            foreach (var child in _owner.ChildrenFlowControls)
            {
                SubscribePositionChangedEvents(child);
            }

            InvalidateScheme(true);
        }

        public void OnUnloaded()
        {
            foreach (var child in _owner.ChildrenFlowControls)
            {
                UnsubscribePositionChangedEvents(child);
            }
        }

        private void SubscribePositionChangedEvents(IFlowControl element)
        {
            switch (element)
            {
                case IPipe pipe:
                    pipe.SchemeChanged += OnFlowControlSchemeChangedEventHandler;
                    break;

                case IValve valve:
                    valve.SchemeChanged += OnFlowControlSchemeChangedEventHandler;
                    valve.StateChanged += OnValveStateChangedEventHandler;
                    break;

                default:
                    throw new ArgumentException(nameof(element));
            }
        }

        private void UnsubscribePositionChangedEvents(IFlowControl element)
        {
            switch (element)
            {
                case IPipe pipe:
                    pipe.SchemeChanged -= OnFlowControlSchemeChangedEventHandler;
                    break;

                case IValve valve:
                    valve.SchemeChanged -= OnFlowControlSchemeChangedEventHandler;
                    valve.StateChanged -= OnValveStateChangedEventHandler;
                    break;

                default:
                    throw new ArgumentException(nameof(element));
            }
        }

        private static void OnFlowControlSchemeChanged(object sender, EventArgs e)
        {
            var flowControl = (IFlowControl) sender;
            var schemeContainer = (SchemeContainer) flowControl.SchemeContainer;
            schemeContainer.InvalidateScheme(false);
        }

        private static void OnValveStateChanged(object sender, EventArgs e)
        {
            var valve = (IValve)sender;
            var schemeContainer = (SchemeContainer)valve.SchemeContainer;
            schemeContainer.InvalidateSchemeFlow();
        }

        private void InvalidateScheme(bool isFirstLoad)
        {
            if (isFirstLoad)
            {
                InvalidateSchemeImpl();
                return;
            }

            if (_isInvalidateCalled)
            {
                return;
            }

            // hack to avoid massive pack of calls to one
            _isInvalidateCalled = true;
            Dispatcher.CurrentDispatcher.InvokeAsync(InvalidateSchemeImpl, DispatcherPriority.Background);
        }

        private void InvalidateSchemeImpl()
        {
            _isInvalidateCalled = false;

            var pipes = new List<IPipe>();
            var valves = new List<IValve>();

            foreach (var child in _owner.ChildrenFlowControls)
            {
                child.SchemeContainer = this;

                if (child is IPipe pipe)
                {
                    pipes.Add(pipe);
                }

                if (child is IValve valve)
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

        private void InvalidateSchemeFlow()
        {
            _scheme.InvalidateFlow();
        }
    }
}