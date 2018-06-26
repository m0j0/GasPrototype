using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using MugenMvvmToolkit.Binding;
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

        #endregion

        #region Constructors

        static Valve()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Valve), new FrameworkPropertyMetadata(typeof(Valve)));
            SizeChangedHandler = OnSizeChanged;
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
        public ValveState State
        {
            get { return (ValveState) GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        [Category("Model")]
        public IReadOnlyCollection<INamedCommand> MenuCommands
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

        bool IFlowControl.IsVisible => Visibility == Visibility.Visible;

        #endregion

        #region Methods

        public ISchemeContainer GetContainer()
        {
            return (ISchemeContainer) Parent;
        }

        public bool CanPassFlow(IFlowGraph graph, IPipeSegment pipeSegment)
        {
            return State == ValveState.Opened;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            DependencyPropertyDescriptor
                .FromProperty(OrientationProperty, typeof(Valve))
                .AddValueChanged(this, SizeChangedHandler);
            DependencyPropertyDescriptor
                .FromProperty(VisibilityProperty, typeof(Valve))
                .AddValueChanged(this, SizeChangedHandler);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            DependencyPropertyDescriptor
                .FromProperty(OrientationProperty, typeof(Valve))
                .RemoveValueChanged(this, SizeChangedHandler);
            DependencyPropertyDescriptor
                .FromProperty(VisibilityProperty, typeof(Valve))
                .RemoveValueChanged(this, SizeChangedHandler);
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

            valve.Bind(() => v => v.State).To(model, () => (m, ctx) => m.State).Build();
            valve.Bind(() => v => v.MenuCommands).To(model, () => (m, ctx) => m.Commands).Build();
            valve.Bind(() => v => v.Visibility)
                .To(model, () => (m, ctx) => m.IsPresent ? Visibility.Visible : Visibility.Collapsed).Build();
        }

        #endregion
    }
}