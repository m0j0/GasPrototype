using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Prototype.Core.Controls.PipeFlowScheme;
using Prototype.Core.Interfaces;
using Prototype.Core.Interfaces.Controls;
using Prototype.Core.Models;

namespace Prototype.Core.Controls
{
    public sealed class Valve3Way : Control, IValve3Way
    {
        #region Fields

        private static readonly EventHandler SizeChangedHandler;
        private static readonly DependencyProperty[] SubscribedProperties;
        private readonly Valve3WayModel _model;

        #endregion

        #region Constructors

        static Valve3Way()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Valve3Way), new FrameworkPropertyMetadata(typeof(Valve3Way)));
            SizeChangedHandler = OnSizeChanged;
            SubscribedProperties = new[] { HeightProperty, WidthProperty, RotationProperty, VisibilityProperty };
        }

        public Valve3Way()
        {
            _model = new Valve3WayModel(this);

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        #endregion

        #region Dependency properties
        
        public static readonly DependencyProperty RotationProperty = DependencyProperty.Register(
            "Rotation", typeof(Rotation), typeof(Valve3Way), new PropertyMetadata(Rotation.Rotate0));

        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
            "State", typeof(ValveState), typeof(Valve3Way),
            new PropertyMetadata(ValveState.Unknown, OnStatePropertyChangedCallback));

        public static readonly DependencyProperty MenuCommandsProperty = DependencyProperty.Register(
            "MenuCommands", typeof(IReadOnlyCollection<INamedCommand>), typeof(Valve3Way),
            new PropertyMetadata(default(IReadOnlyCollection<INamedCommand>)));

        public static readonly DependencyProperty ValveModelProperty = DependencyProperty.Register(
            "ValveVm", typeof(IValveVm), typeof(Valve3Way),
            new PropertyMetadata(default(IValveVm), ValveVmPropertyChangedCallback));

        public static readonly DependencyProperty PathWhenOpenProperty = DependencyProperty.Register(
            "PathWhenOpen", typeof(Valve3WayFlowPath), typeof(Valve3Way), new PropertyMetadata(default(Valve3WayFlowPath), OnStatePropertyChangedCallback));

        public static readonly DependencyProperty PathWhenClosedProperty = DependencyProperty.Register(
            "PathWhenClosed", typeof(Valve3WayFlowPath), typeof(Valve3Way), new PropertyMetadata(default(Valve3WayFlowPath), OnStatePropertyChangedCallback));

        #endregion

        #region Events

        public event EventHandler SchemeChanged;

        public event EventHandler StateChanged;

        #endregion

        #region Properties
        
        public Rotation Rotation
        {
            get { return (Rotation) GetValue(RotationProperty); }
            set { SetValue(RotationProperty, value); }
        }

        [Category("Model")]
        public ValveState State
        {
            get { return (ValveState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        [Category("Model")]
        public IReadOnlyCollection<INamedCommand> MenuCommands
        {
            get { return (IReadOnlyCollection<INamedCommand>)GetValue(MenuCommandsProperty); }
            set { SetValue(MenuCommandsProperty, value); }
        }

        [Category("Model")]
        public IValveVm ValveVm
        {
            get { return (IValveVm)GetValue(ValveModelProperty); }
            set { SetValue(ValveModelProperty, value); }
        }

        public Valve3WayFlowPath PathWhenOpen
        {
            get { return (Valve3WayFlowPath) GetValue(PathWhenOpenProperty); }
            set { SetValue(PathWhenOpenProperty, value); }
        }

        public Valve3WayFlowPath PathWhenClosed
        {
            get { return (Valve3WayFlowPath) GetValue(PathWhenClosedProperty); }
            set { SetValue(PathWhenClosedProperty, value); }
        }

        bool IFlowControl.IsVisible => Visibility == Visibility.Visible;

        bool IValve3Way.IsOpen => State == ValveState.Open;

        #endregion

        #region Methods

        public ISchemeContainer GetContainer()
        {
            return (ISchemeContainer)Parent;
        }

        public bool CanPassFlow(IFlowGraph graph, IPipeSegment pipeSegment)
        {
            return _model.CanPassFlow(graph, pipeSegment);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (var property in SubscribedProperties)
            {
                DependencyPropertyDescriptor
                    .FromProperty(property, typeof(Valve3Way))
                    .AddValueChanged(this, SizeChangedHandler);
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            foreach (var property in SubscribedProperties)
            {
                DependencyPropertyDescriptor
                    .FromProperty(property, typeof(Valve3Way))
                    .RemoveValueChanged(this, SizeChangedHandler);
            }
        }

        private static void OnSizeChanged(object sender, EventArgs e)
        {
            var valve = (Valve3Way)sender;
            valve.SchemeChanged?.Invoke(valve, EventArgs.Empty);
        }

        private static void OnStatePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var valve = (Valve3Way)d;
            valve.StateChanged?.Invoke(valve, EventArgs.Empty);
        }

        private static void ValveVmPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs args)
        {
            var valve = (Valve3Way)dependencyObject;
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