using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    public sealed class PipeFlowCanvas : Canvas, ISchemeContainer
    {
        private readonly PipeFlowScheme _scheme;

        public PipeFlowCanvas()
        {
            _scheme = new PipeFlowScheme(this);

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

            if (visualAdded is IFlowControl addedControl)
            {
                _scheme.Add(addedControl);
                SubscribePositionChangedEvents(visualAdded);
            }

            if (visualRemoved is IFlowControl removedControl)
            {
                _scheme.Remove(removedControl);
                UnsubscribePositionChangedEvents(visualRemoved);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var flowControls = new List<IFlowControl>();
            foreach (DependencyObject child in Children)
            {
                if (child is IFlowControl flowControl)
                {
                    if (!_scheme.Contains(flowControl))
                    {
                        flowControls.Add(flowControl);
                    }

                    SubscribePositionChangedEvents(child);
                }
            }

            _scheme.Add(flowControls);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // TODO scheme also should unsubscribe
            foreach (DependencyObject child in Children)
            {
                if (child is IFlowControl)
                {
                    UnsubscribePositionChangedEvents(child);
                }
            }
        }

        private void SubscribePositionChangedEvents(DependencyObject element)
        {
            var topDescriptor = DependencyPropertyDescriptor.FromProperty(TopProperty, typeof(DependencyObject));
            var leftDescriptor = DependencyPropertyDescriptor.FromProperty(LeftProperty, typeof(DependencyObject));
            topDescriptor.AddValueChanged(element, OnFlowControlPositionChanged);
            leftDescriptor.AddValueChanged(element, OnFlowControlPositionChanged);
        }

        private void UnsubscribePositionChangedEvents(DependencyObject element)
        {
            var topDescriptor = DependencyPropertyDescriptor.FromProperty(TopProperty, typeof(DependencyObject));
            var leftDescriptor = DependencyPropertyDescriptor.FromProperty(LeftProperty, typeof(DependencyObject));
            topDescriptor.RemoveValueChanged(element, OnFlowControlPositionChanged);
            leftDescriptor.RemoveValueChanged(element, OnFlowControlPositionChanged);
        }

        private void OnFlowControlPositionChanged(object sender, EventArgs e)
        {
            SchemeChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}