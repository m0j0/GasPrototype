using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    public sealed class PipeFlowCanvas : Canvas, ISchemeContainer
    {
        private static readonly EventHandler OnFlowControlPositionChangedEventHandler = OnFlowControlPositionChanged;
        private static readonly EventHandler OnValveStateChangedEventHandler = OnValveStateChanged;

        private FlowGraph _scheme;

        public PipeFlowCanvas()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        public event EventHandler SchemeChanged;

        public double GetTop(IFlowControl control)
        {
            return GetTop((UIElement) control);
        }

        public double GetLeft(IFlowControl control)
        {
            return GetLeft((UIElement) control);
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
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (DependencyObject child in Children)
            {
                SubscribePositionChangedEvents(child);
            }

            // TODO optimize when there is no changes
            InvalidateScheme();
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
                    ((PipeFlowCanvas) flowControl.GetContainer()).InvalidateScheme();
                    break;

                case PipeFlowCanvas canvas:
                    canvas.InvalidateScheme();
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

        private void InvalidateScheme()
        {
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

            _scheme = new FlowGraph(this, pipes, valves);

            SchemeChanged?.Invoke(this, EventArgs.Empty);
        }

        private void InvalidateSchemeFlow()
        {
            _scheme.InvalidateFlow();
        }
    }
}