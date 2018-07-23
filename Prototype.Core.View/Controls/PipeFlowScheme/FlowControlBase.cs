using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    public abstract class FlowControlBase : Control, IFlowControl
    {
        #region Fields

        private static readonly Dictionary<Type, DependencyProperty[]> SubscribedProperties = new Dictionary<Type, DependencyProperty[]>();
        private static readonly EventHandler SizeChangedHandler;

        #endregion

        #region Constructors

        static FlowControlBase()
        {
            SizeChangedHandler = OnSizeChanged;
        }

        protected FlowControlBase()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        #endregion

        #region Events

        public event EventHandler SchemeChanged;

        #endregion

        #region Properties

        Rect IFlowControl.LayoutRect => LayoutInformation.GetLayoutSlot(this);

        Vector IFlowControl.Offset { get; set; }

        bool IFlowControl.IsVisible => Visibility == Visibility.Visible;

        ISchemeContainer IFlowControl.SchemeContainer { get; set; }

        #endregion

        #region Methods

        protected static void SetSubscribedProperties(Type type, params DependencyProperty[] properties)
        {
            if (SubscribedProperties.ContainsKey(type))
            {
                throw new InvalidOperationException("Subscribed properties are already initialized.");
            }

            SubscribedProperties[type] = properties;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (var property in SubscribedProperties[GetType()])
            {
                DependencyPropertyDescriptor
                    .FromProperty(property, typeof(Pipe))
                    .AddValueChanged(this, SizeChangedHandler);
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            foreach (var property in SubscribedProperties[GetType()])
            {
                DependencyPropertyDescriptor
                    .FromProperty(property, typeof(Pipe))
                    .RemoveValueChanged(this, SizeChangedHandler);
            }
        }

        private static void OnSizeChanged(object sender, EventArgs e)
        {
            var control = (FlowControlBase) sender;
            control.SchemeChanged?.Invoke(control, EventArgs.Empty);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            SchemeChanged?.Invoke(this, EventArgs.Empty);

            return base.ArrangeOverride(arrangeBounds);
        }

        #endregion
    }
}