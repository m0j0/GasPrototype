using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Prototype.Core.Controls.PipeFlowScheme;
using Prototype.Core.Interfaces;
using Prototype.Core.Interfaces.Controls;
using Prototype.Core.Models;
using Prototype.Core.Models.GasPanel;

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
            SubscribedProperties = new[] { HeightProperty, WidthProperty, OrientationProperty, VisibilityProperty };
        }

        public Valve()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        #endregion

        #region Dependency properties

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation", typeof(Orientation), typeof(Valve), new PropertyMetadata(Orientation.Horizontal));

        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
            "State", typeof(ValveState), typeof(Valve),
            new PropertyMetadata(ValveState.Unknown, OnStatePropertyChangedCallback));

        public static readonly DependencyProperty MenuCommandsProperty = DependencyProperty.Register(
            "MenuCommands", typeof(IReadOnlyCollection<INamedCommand>), typeof(Valve),
            new PropertyMetadata(default(IReadOnlyCollection<INamedCommand>)));

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

        [Category("Model")]
        internal ValveState State
        {
            get { return (ValveState) GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        [Category("Model")]
        internal IReadOnlyCollection<INamedCommand> MenuCommands
        {
            get { return (IReadOnlyCollection<INamedCommand>) GetValue(MenuCommandsProperty); }
            set { SetValue(MenuCommandsProperty, value); }
        }

        [Category("Model")]
        public IValveVm ValveVm
        {
            get { return (IValveVm) GetValue(ValveModelProperty); }
            set { SetValue(ValveModelProperty, value); }
        }

        Rect IFlowControl.LayoutRect => LayoutInformation.GetLayoutSlot(this);

        bool IFlowControl.IsVisible => Visibility == Visibility.Visible;

        #endregion

        #region Methods

        public ISchemeContainer GetContainer()
        {
            return (ISchemeContainer) Parent;
        }

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

        private static void ValveVmPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs args)
        {
            var valve = (Valve) dependencyObject;
            var model = args.NewValue as IValveVm;

            if (model == null)
            {
                return;
            }

            CoreViewExtensions.SetOneTimeBinding(valve, StateProperty, nameof(IValveVm.State), model);
            CoreViewExtensions.SetOneTimeBinding(valve, MenuCommandsProperty, nameof(IValveVm.Commands), model);
            CoreViewExtensions.SetOneTimeBinding(valve, VisibilityProperty, nameof(IValveVm.IsPresent), model, CoreViewExtensions.BooleanToVisibilityConverterInstance);
        }

        #endregion
    }
}