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
        private Vector _offset;

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

        Vector IFlowControl.Offset => _offset;

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

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            SchemeChanged?.Invoke(this, EventArgs.Empty);

            return base.ArrangeOverride(arrangeBounds);
        }


        protected FrameworkElement _containerOwner;

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            if (!IsLoaded)
            {
                return;
            }
            if (oldParent == null)
            {

            }
            else
            {
                // TODO
            }
        }

        private void AddControlToContainer(FrameworkElement containerOwner)
        {
            var schemeContainer = FlowSchemeSettings.GetContainer(containerOwner);
            if (schemeContainer == null)
            {
                schemeContainer = new SchemeContainer2();
                FlowSchemeSettings.SetContainer(containerOwner, schemeContainer);
            }

            schemeContainer.Add(this);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (var property in SubscribedProperties[GetType()])
            {
                DependencyPropertyDescriptor
                    .FromProperty(property, typeof(Pipe))
                    .AddValueChanged(this, SizeChangedHandler);
            }
            
            FrameworkElement containerOwner = null;
            var parent = VisualParent as FrameworkElement;
            var offset = new Vector();
            while (parent != null)
            {
                if (FlowSchemeSettings.GetContainerType(parent) == ContainerType.Main ||
                    FlowSchemeSettings.GetContainerType(parent) == ContainerType.Child)
                {
                    containerOwner = parent;
                }

                var layout = LayoutInformation.GetLayoutSlot(parent);
                offset.X += layout.X;
                offset.Y += layout.Y;
                parent = parent.Parent as FrameworkElement;
            }

            if (containerOwner == null)
            {
                throw new InvalidOperationException("Can't place flow control without container.");
            }

            if (FlowSchemeSettings.GetContainerType(containerOwner) == ContainerType.Child)
            {
                if (DesignerProperties.GetIsInDesignMode(this))
                {
                    AddControlToContainer(containerOwner);
                }
                else
                {
                    throw new InvalidOperationException("Can't find main container for child container.");
                }
            }

            _containerOwner = containerOwner;
            _offset = offset;

            if (_containerOwner != null)
            {
                AddControlToContainer(_containerOwner);
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

        #endregion
    }
}