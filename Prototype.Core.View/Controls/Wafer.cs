using System.Windows;
using System.Windows.Controls;
using MugenMvvmToolkit.Binding;
using Prototype.Core.Interfaces;
using Prototype.Core.Models;

namespace Prototype.Core.Controls
{
    public sealed class Wafer : Control
    {
        #region Fields

        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register(
            "Status", typeof(WaferStatus), typeof(Wafer), new PropertyMetadata(default(WaferStatus)));

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            "Label", typeof(string), typeof(Wafer), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty WorkingModeProperty = DependencyProperty.Register(
            "WorkingMode", typeof(WaferWorkingMode), typeof(Wafer), new PropertyMetadata(default(WaferWorkingMode)));

        public static readonly DependencyProperty WaferVmProperty = DependencyProperty.Register(
            "WaferVm", typeof(IWaferVm), typeof(Wafer), new PropertyMetadata(default(IWaferVm), WaferVmPropertyChangedCallback));

        #endregion

        #region Constructors

        static Wafer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Wafer), new FrameworkPropertyMetadata(typeof(Wafer)));
        }

        #endregion

        #region Properties

        public WaferStatus Status
        {
            get { return (WaferStatus) GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        public string Label
        {
            get { return (string) GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public WaferWorkingMode WorkingMode
        {
            get { return (WaferWorkingMode) GetValue(WorkingModeProperty); }
            set { SetValue(WorkingModeProperty, value); }
        }

        public IWaferVm WaferVm
        {
            get { return (IWaferVm) GetValue(WaferVmProperty); }
            set { SetValue(WaferVmProperty, value); }
        }

        #endregion

        #region Methods

        private static void WaferVmPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var wafer = (Wafer)dependencyObject;
            var model = args.NewValue as IWaferVm;

            if (model == null)
            {
                return;
            }
            wafer.Bind(() => v => v.Status).To(model, () => (m, ctx) => m.Status).Build();
            wafer.Bind(() => v => v.Label).To(model, () => (m, ctx) => m.Label).Build();
            wafer.Bind(() => v => v.WorkingMode).To(model, () => (m, ctx) => m.WorkingMode).Build();
        }

        #endregion
    }
}