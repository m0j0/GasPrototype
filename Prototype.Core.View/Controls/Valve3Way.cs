using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MugenMvvmToolkit.Binding;
using Prototype.Core.Controls.PipeFlowScheme;
using Prototype.Core.Interfaces;
using Prototype.Core.Interfaces.Controls;
using Prototype.Core.Models;

namespace Prototype.Core.Controls
{
    public sealed class Valve3Way : Control, IValve
    {
        #region Fields

        private static readonly EventHandler SizeChangedHandler;

        #endregion

        #region Constructors

        static Valve3Way()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Valve3Way), new FrameworkPropertyMetadata(typeof(Valve3Way)));
            SizeChangedHandler = OnSizeChanged;
        }

        public Valve3Way()
        {
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

        public static readonly DependencyProperty UseAuxiliaryPathWhenOpenedProperty = DependencyProperty.Register(
            "UseAuxiliaryPathWhenOpened", typeof(bool), typeof(Valve3Way), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsPrimaryPathFlowInvertedProperty = DependencyProperty.Register(
            "IsPrimaryPathFlowInverted", typeof(bool), typeof(Valve3Way), new PropertyMetadata(default(bool)));

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

        public bool UseAuxiliaryPathWhenOpened
        {
            get { return (bool) GetValue(UseAuxiliaryPathWhenOpenedProperty); }
            set { SetValue(UseAuxiliaryPathWhenOpenedProperty, value); }
        }

        public bool IsPrimaryPathFlowInverted
        {
            get { return (bool) GetValue(IsPrimaryPathFlowInvertedProperty); }
            set { SetValue(IsPrimaryPathFlowInvertedProperty, value); }
        }

        bool IFlowControl.IsVisible => Visibility == Visibility.Visible;

        #endregion

        #region Methods

        public ISchemeContainer GetContainer()
        {
            return (ISchemeContainer)Parent;
        }

        public bool CanPassFlow(IFlowGraph graph, IPipeSegment pipeSegment)
        {
            var pipe = graph.FindPipe(pipeSegment);

            var valveAbsoluteRect = graph.GetAbsoluteRect(this);
            var intersectedPipeAbsoluteRect = graph.GetAbsoluteRect(pipe);
            var valveOffset = new Vector(-valveAbsoluteRect.TopLeft.X, -valveAbsoluteRect.TopLeft.Y);

            intersectedPipeAbsoluteRect.Offset(valveOffset);

            var intersection = Common.FindIntersection(intersectedPipeAbsoluteRect, GetPrimaryUpperPipeRect(Rotation));
            if (intersection == GetPrimaryUpperPipeRect(Rotation))
            {
                return CanPrimaryUpperPipePassFlow();
            }
            intersection = Common.FindIntersection(intersectedPipeAbsoluteRect, GetPrimaryLowerPipeRect(Rotation));
            if (intersection == GetPrimaryLowerPipeRect(Rotation))
            {
                return CanPrimaryLowerPipePassFlow();
            }
            intersection = Common.FindIntersection(intersectedPipeAbsoluteRect, GetAuxiliaryPipeRect(Rotation));
            if (intersection == GetAuxiliaryPipeRect(Rotation))
            {
                return CanAuxiliaryPipePassFlow();
            }

            return State == ValveState.Opened;
        }



        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            DependencyPropertyDescriptor
                .FromProperty(RotationProperty, typeof(Valve3Way))
                .AddValueChanged(this, SizeChangedHandler);
            DependencyPropertyDescriptor
                .FromProperty(VisibilityProperty, typeof(Valve3Way))
                .AddValueChanged(this, SizeChangedHandler);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            DependencyPropertyDescriptor
                .FromProperty(RotationProperty, typeof(Valve3Way))
                .RemoveValueChanged(this, SizeChangedHandler);
            DependencyPropertyDescriptor
                .FromProperty(VisibilityProperty, typeof(Valve3Way))
                .RemoveValueChanged(this, SizeChangedHandler);
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

            valve.Bind(() => v => v.State).To(model, () => (m, ctx) => m.State).Build();
            valve.Bind(() => v => v.MenuCommands).To(model, () => (m, ctx) => m.Commands).Build();
            valve.Bind(() => v => v.Visibility)
                .To(model, () => (m, ctx) => m.IsPresent ? Visibility.Visible : Visibility.Collapsed).Build();
        }

        #endregion

        #region Flow logic

        private static Rect GetPrimaryUpperPipeRect(Rotation rotation)
        {
            switch (rotation)
            {
                case Rotation.Rotate0:
                    return new Rect(19, 0, 5, 24);
                case Rotation.Rotate90:
                    return new Rect(5, 5, 100, 5);
                case Rotation.Rotate180:
                    return new Rect(5, 5, 100, 5);
                case Rotation.Rotate270:
                    return new Rect(5, 5, 100, 5);
                default:
                    throw new ArgumentOutOfRangeException(nameof(rotation), rotation, null);
            }
        }

        private static Rect GetPrimaryLowerPipeRect(Rotation rotation)
        {
            switch (rotation)
            {
                case Rotation.Rotate0:
                    return new Rect(19, 19, 5, 24);
                case Rotation.Rotate90:
                    return new Rect(5, 5, 100, 5);
                case Rotation.Rotate180:
                    return new Rect(5, 5, 100, 5);
                case Rotation.Rotate270:
                    return new Rect(5, 5, 100, 5);
                default:
                    throw new ArgumentOutOfRangeException(nameof(rotation), rotation, null);
            }
        }

        private static Rect GetAuxiliaryPipeRect(Rotation rotation)
        {
            switch (rotation)
            {
                case Rotation.Rotate0:
                    return new Rect(0, 19, 24, 5);
                case Rotation.Rotate90:
                    return new Rect(5, 5, 100, 5);
                case Rotation.Rotate180:
                    return new Rect(5, 5, 100, 5);
                case Rotation.Rotate270:
                    return new Rect(5, 5, 100, 5);
                default:
                    throw new ArgumentOutOfRangeException(nameof(rotation), rotation, null);
            }
        }

        private bool CanPrimaryUpperPipePassFlow()
        {
            return true;
        }

        private bool CanPrimaryLowerPipePassFlow()
        {
            return State == ValveState.Opened;
        }

        private bool CanAuxiliaryPipePassFlow()
        {
            return State == ValveState.Closed;
        }

        #endregion
    }
}