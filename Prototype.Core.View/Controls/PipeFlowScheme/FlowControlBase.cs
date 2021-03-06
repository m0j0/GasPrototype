﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MugenMvvmToolkit;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    public abstract class FlowControlBase : Control, IFlowControl
    {
        #region Fields

        private static readonly Dictionary<Type, DependencyProperty[]> SubscribedProperties = new Dictionary<Type, DependencyProperty[]>();
        private static readonly EventHandler SizeChangedHandler;
        private static readonly PropertyInfo PreviousArrangeRectProperty;

        protected ISchemeContainer SchemeContainer;
        private Vector _offset;
        private Rect _previousArrangeRect;

        #endregion

        #region Constructors

        static FlowControlBase()
        {
            SizeChangedHandler = OnSizeChanged;

            PreviousArrangeRectProperty = typeof(UIElement).GetProperty("PreviousArrangeRect", BindingFlags.Instance | BindingFlags.NonPublic);
            Should.NotBeNull(PreviousArrangeRectProperty, nameof(PreviousArrangeRectProperty));
        }

        protected FlowControlBase()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            LayoutUpdated += OnLayoutUpdated;
        }

        #endregion

        #region Properties

        Rect IFlowControl.LayoutRect => LayoutInformation.GetLayoutSlot(this);

        Vector IFlowControl.Offset => _offset;

        bool IFlowControl.IsVisible => Visibility == Visibility.Visible;

        #endregion

        #region Methods

        public void SetContrainer(ISchemeContainer container, Vector offset)
        {
            SchemeContainer = container;
            _offset = offset;
        }

        protected static void SetSubscribedProperties(Type type, params DependencyProperty[] properties)
        {
            if (SubscribedProperties.ContainsKey(type))
            {
                throw new InvalidOperationException("Subscribed properties are already initialized.");
            }

            SubscribedProperties[type] = properties;
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            if (!IsLoaded)
            {
                return;
            }

            if (oldParent == null)
            {
                AddControlToScheme();
            }
            else
            {
                SchemeContainer?.InvalidateScheme();
                FindOrCreateSchemeContainer();
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            AddControlToScheme();

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

        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            var currentArrangeRect = PreviousArrangeRectProperty.GetValueEx<Rect>(this);
            if (currentArrangeRect != _previousArrangeRect)
            {
                _previousArrangeRect = currentArrangeRect;
                SchemeContainer?.InvalidateScheme();
            }
        }

        private static void OnSizeChanged(object sender, EventArgs e)
        {
            var control = (FlowControlBase) sender;
            control.SchemeContainer?.InvalidateScheme();
        }

        private void AddControlToScheme()
        {
            if (SchemeContainer != null)
            {
                return;
            }

            FindOrCreateSchemeContainer();
        }

        private void FindOrCreateSchemeContainer()
        {
            var containerOwner = VisualParent as FrameworkElement;

            var parent = containerOwner;
            while (parent != null)
            {
                if (FlowSchemeSettings.GetIsFlowSchemeContainer(parent))
                {
                    containerOwner = parent;
                    break;
                }

                parent = parent.Parent as FrameworkElement;
            }

            if (containerOwner == null)
            {
                return;
            }

            var schemeContainer = FlowSchemeSettings.GetContainer(containerOwner);
            if (schemeContainer == null)
            {
                schemeContainer = new SchemeContainer(containerOwner);
                FlowSchemeSettings.SetContainer(containerOwner, schemeContainer);
                return;
            }

            schemeContainer.InvalidateScheme();
        }

        #endregion
    }
}