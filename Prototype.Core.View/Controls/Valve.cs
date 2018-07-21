using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using Prototype.Core.Controls.PipeFlowScheme;
using Prototype.Core.Interfaces;
using Prototype.Core.Interfaces.Controls;
using Prototype.Core.Models;

namespace Prototype.Core.Controls
{
    public sealed class Valve : Control, IValve
    {
        #region Fields

        private static readonly EventHandler SizeChangedHandler;
        private static readonly DependencyProperty[] SubscribedProperties;

        #endregion

        #region Constructors

        static Valve()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Valve), new FrameworkPropertyMetadata(typeof(Valve)));
            SizeChangedHandler = OnSizeChanged;
            SubscribedProperties = new[] { OrientationProperty, VisibilityProperty };
        }

        public Valve()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        #endregion

        #region Dependency properties

        internal static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation", typeof(Orientation), typeof(Valve), new PropertyMetadata(Orientation.Horizontal));

        internal static readonly DependencyProperty StateProperty = DependencyProperty.Register(
            "State", typeof(ValveState), typeof(Valve),
            new PropertyMetadata(ValveState.Unknown, OnStatePropertyChangedCallback));

        internal static readonly DependencyProperty MenuProperty = DependencyProperty.Register(
            "Menu", typeof(IMenu), typeof(Valve), new PropertyMetadata(default(IMenu)));

        public static readonly DependencyProperty ValveModelProperty = DependencyProperty.Register(
            "ValveVm", typeof(IValveVm), typeof(Valve),
            new PropertyMetadata(default(IValveVm), ValveVmPropertyChangedCallback));

        #endregion

        #region Events

        public event EventHandler SchemeChanged;

        public event EventHandler StateChanged;

        #endregion

        #region Properties

        [Category("Layout")]
        public Orientation Orientation
        {
            get { return (Orientation) GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
        
        internal ValveState State
        {
            get { return (ValveState) GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        internal IMenu Menu
        {
            get { return (IMenu) GetValue(MenuProperty); }
            set { SetValue(MenuProperty, value); }
        }

        [Category("Model")]
        public IValveVm ValveVm
        {
            get { return (IValveVm) GetValue(ValveModelProperty); }
            set { SetValue(ValveModelProperty, value); }
        }

        Rect IFlowControl.LayoutRect => LayoutInformation.GetLayoutSlot(this);

        Vector IFlowControl.Offset { get; set; }

        bool IFlowControl.IsVisible => Visibility == Visibility.Visible;

        ISchemeContainer IFlowControl.SchemeContainer { get; set; }

        #endregion

        #region Methods

        public bool CanPassFlow(IFlowGraph graph, IPipeSegment pipeSegment)
        {
            return State == ValveState.Open;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (var property in SubscribedProperties)
            {
                DependencyPropertyDescriptor
                    .FromProperty(property, typeof(Valve))
                    .AddValueChanged(this, SizeChangedHandler);
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            foreach (var property in SubscribedProperties)
            {
                DependencyPropertyDescriptor
                    .FromProperty(property, typeof(Valve))
                    .RemoveValueChanged(this, SizeChangedHandler);
            }
        }

        private static void OnSizeChanged(object sender, EventArgs e)
        {
            var valve = (Valve) sender;
            valve.SchemeChanged?.Invoke(valve, EventArgs.Empty);
        }

        private static void OnStatePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var valve = (Valve) d;
            valve.StateChanged?.Invoke(valve, EventArgs.Empty);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            SchemeChanged?.Invoke(this, EventArgs.Empty);

            return base.ArrangeOverride(arrangeBounds);
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            var rect = new Rect(new Point(), RenderSize);
            return rect.Contains(hitTestParameters.HitPoint) ? new PointHitTestResult(this, hitTestParameters.HitPoint) : null;
        }

        private static void ValveVmPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs args)
        {
            var valve = (Valve) dependencyObject;
            var model = args.NewValue as IValveVm;

            if (model == null)
            {
                BindingOperations.ClearBinding(valve, StateProperty);
                BindingOperations.ClearBinding(valve, MenuProperty);
                BindingOperations.ClearBinding(valve, VisibilityProperty);
                return;
            }

            CoreViewExtensions.SetOneTimeBinding(valve, StateProperty, nameof(IValveVm.State), model);
            CoreViewExtensions.SetOneTimeBinding(valve, MenuProperty, nameof(IValveVm.Menu), model);
            CoreViewExtensions.SetOneTimeBinding(valve, VisibilityProperty, nameof(IValveVm.IsPresent), model, CoreViewExtensions.BooleanToVisibilityConverterInstance);
        }

        #endregion
    }
}