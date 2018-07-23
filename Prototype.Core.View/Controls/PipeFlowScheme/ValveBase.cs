using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Prototype.Core.Interfaces;
using Prototype.Core.Interfaces.Controls;
using Prototype.Core.Models;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    public abstract class ValveBase : FlowControlBase, IValve
    {
        #region Dependency properties

        internal static readonly DependencyProperty StateProperty = DependencyProperty.Register(
            "State", typeof(ValveState), typeof(ValveBase),
            new PropertyMetadata(ValveState.Unknown, OnStatePropertyChangedCallback));

        internal static readonly DependencyProperty MenuProperty = DependencyProperty.Register(
            "Menu", typeof(IMenu), typeof(ValveBase), new PropertyMetadata(default(IMenu)));

        public static readonly DependencyProperty ValveModelProperty = DependencyProperty.Register(
            "ValveVm", typeof(IValveVm), typeof(ValveBase),
            new PropertyMetadata(default(IValveVm), ValveVmPropertyChangedCallback));

        #endregion

        #region Properties

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

        #endregion

        #region Events

        public event EventHandler StateChanged;

        #endregion

        #region Methods

        public abstract bool CanPassFlow(IFlowGraph graph, IPipeSegment pipeSegment);
        
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            var rect = new Rect(new Point(), RenderSize);
            return rect.Contains(hitTestParameters.HitPoint) ? new PointHitTestResult(this, hitTestParameters.HitPoint) : null;
        }

        protected static void OnStatePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var valve = (ValveBase) d;
            valve.StateChanged?.Invoke(valve, EventArgs.Empty);
        }

        private static void ValveVmPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs args)
        {
            var valve = (ValveBase) dependencyObject;
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