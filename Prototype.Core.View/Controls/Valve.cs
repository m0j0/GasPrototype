using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using MugenMvvmToolkit.Binding;
using Prototype.Core.Controls.PipeFlowScheme;
using Prototype.Core.Interfaces;
using Prototype.Core.Interfaces.GasPanel;
using Prototype.Core.Models;
using Prototype.Core.Models.GasPanel;

namespace Prototype.Core.Controls
{
    public sealed class Valve : Control, IValve
    {
        #region Constructors

        static Valve()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Valve), new FrameworkPropertyMetadata(typeof(Valve)));
        }

        #endregion

        #region Dependency properties

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation", typeof(Orientation), typeof(Valve), new PropertyMetadata(Orientation.Vertical));

        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
            "State", typeof(ValveState), typeof(Valve), new PropertyMetadata(ValveState.Unknown));

        public static readonly DependencyProperty MenuCommandsProperty = DependencyProperty.Register(
            "MenuCommands", typeof(IReadOnlyCollection<INamedCommand>), typeof(Valve), new PropertyMetadata(default(IReadOnlyCollection<INamedCommand>)));

        public static readonly DependencyProperty ValveModelProperty = DependencyProperty.Register(
            "ValveVm", typeof(IValveVm), typeof(Valve), new PropertyMetadata(default(IValveVm), ValveVmPropertyChangedCallback));

        #endregion

        #region Events

        public event EventHandler SizeChanged;

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

        #endregion

        #region Methods

        public bool CanPassFlow(IPipe pipe1, IPipe pipe2)
        {
            return State == ValveState.Opened;
        }

        private static void ValveVmPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var valve = (Valve) dependencyObject;
            var model = args.NewValue as IValveVm;

            if (model == null)
            {
                return;
            }
            valve.Bind(() => v => v.State).To(model, () => (m, ctx) => m.State).Build();
            valve.Bind(() => v => v.MenuCommands).To(model, () => (m, ctx) => m.Commands).Build();
            valve.Bind(() => v => v.Visibility).To(model, () => (m, ctx) => m.IsPresent ? Visibility.Visible : Visibility.Collapsed).Build();
        }

        #endregion
    }
}