using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    public sealed class PipeFlowCanvas : Canvas, ISchemeContainer
    {
        private static readonly EventHandler OnFlowControlPositionChangedEventHandler = OnFlowControlPositionChanged;
        private static readonly EventHandler OnValveStateChangedEventHandler = OnValveStateChanged;

        private FlowGraph _scheme;
        private bool _isInvalidateCalled;

        public PipeFlowCanvas()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        public event EventHandler SchemeChanged;

        public double GetTop(IFlowControl control)
        {
            var top = GetTop((UIElement)control);
            return double.IsNaN(top) ? 0 : top;
        }

        public double GetLeft(IFlowControl control)
        {
            var left = GetLeft((UIElement) control);
            return double.IsNaN(left) ? 0 : left;
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            if (!IsLoaded)
            {
                return;
            }

            if (visualAdded is IFlowControl)
            {
                SubscribePositionChangedEvents(visualAdded);
            }

            if (visualRemoved is IFlowControl)
            {
                UnsubscribePositionChangedEvents(visualRemoved);
            }

            InvalidateScheme(false);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (DependencyObject child in Children)
            {
                SubscribePositionChangedEvents(child);
            }

            InvalidateScheme(true);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            foreach (DependencyObject child in Children)
            {
                UnsubscribePositionChangedEvents(child);
            }
        }

        private void SubscribePositionChangedEvents(DependencyObject element)
        {
            switch (element)
            {
                case IPipe pipe:
                    pipe.SchemeChanged += OnFlowControlPositionChangedEventHandler;
                    break;
                case IValve valve:
                    valve.SchemeChanged += OnFlowControlPositionChangedEventHandler;
                    valve.StateChanged += OnValveStateChangedEventHandler;
                    break;
            }

            DependencyPropertyDescriptor
                .FromProperty(TopProperty, typeof(DependencyObject))
                .AddValueChanged(element, OnFlowControlPositionChangedEventHandler);
            DependencyPropertyDescriptor
                .FromProperty(LeftProperty, typeof(DependencyObject))
                .AddValueChanged(element, OnFlowControlPositionChangedEventHandler);
        }

        private void UnsubscribePositionChangedEvents(DependencyObject element)
        {
            switch (element)
            {
                case IPipe pipe:
                    pipe.SchemeChanged -= OnFlowControlPositionChangedEventHandler;
                    break;
                case IValve valve:
                    valve.SchemeChanged -= OnFlowControlPositionChangedEventHandler;
                    valve.StateChanged -= OnValveStateChangedEventHandler;
                    break;
            }

            DependencyPropertyDescriptor
                .FromProperty(TopProperty, typeof(DependencyObject))
                .RemoveValueChanged(element, OnFlowControlPositionChangedEventHandler);
            DependencyPropertyDescriptor
                .FromProperty(LeftProperty, typeof(DependencyObject))
                .RemoveValueChanged(element, OnFlowControlPositionChangedEventHandler);
        }

        private static void OnFlowControlPositionChanged(object sender, EventArgs e)
        {
            switch (sender)
            {
                case IFlowControl flowControl:
                    ((PipeFlowCanvas) flowControl.GetContainer()).InvalidateScheme(false);
                    break;

                case PipeFlowCanvas canvas:
                    canvas.InvalidateScheme(false);
                    break;

                default:
                    throw new ArgumentException(nameof(sender));
            }
        }

        private static void OnValveStateChanged(object sender, EventArgs e)
        {
            var valve = (IValve) sender;
            ((PipeFlowCanvas) valve.GetContainer()).InvalidateSchemeFlow();
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
            Dispatcher.InvokeAsync(InvalidateSchemeImpl, DispatcherPriority.Background);
        }

        private void InvalidateSchemeImpl()
        {
            _isInvalidateCalled = false;

            var pipes = new List<IPipe>();
            var valves = new List<IValve>();

            foreach (var child in Children)
            {
                if (child is IPipe pipe)
                {
                    pipes.Add(pipe);
                }

                if (child is IValve valve)
                {
                    valves.Add(valve);
                }
            }

            if (_scheme != null && _scheme.Equals(this, pipes, valves))
            {
                return;
            }
            _scheme = new FlowGraph(this, pipes, valves);

            SchemeChanged?.Invoke(this, EventArgs.Empty);
        }

        private void InvalidateSchemeFlow()
        {
            _scheme.InvalidateFlow();
        }
    }
}