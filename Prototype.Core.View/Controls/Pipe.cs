using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MugenMvvmToolkit.Binding;
using Prototype.Core.Controls.PipeFlowScheme;
using Prototype.Core.Interfaces;
using Prototype.Core.Interfaces.GasPanel;
using Prototype.Core.Models;
using Prototype.Core.Models.GasPanel;

namespace Prototype.Core.Controls
{
    public sealed class Pipe : Control, IPipe
    {
        #region Constants

        internal const double PipeWidth = 5;

        #endregion

        #region Constructors

        static Pipe()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Pipe), new FrameworkPropertyMetadata(typeof(Pipe)));
        }

        public Pipe()
        {

        }

        #endregion

        #region Dependency properties

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation", typeof(Orientation), typeof(Pipe), new PropertyMetadata(Orientation.Horizontal));

        public static readonly DependencyProperty HasFlowProperty = DependencyProperty.Register(
            "HasFlow", typeof(bool), typeof(Pipe), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty SubstanceTypeProperty = DependencyProperty.Register(
            "SubstanceType", typeof(SubstanceType), typeof(Pipe), new PropertyMetadata(SubstanceType.Gas));

        internal static readonly DependencyProperty PipeModelProperty = DependencyProperty.Register(
            "PipeVm", typeof(IPipeVm), typeof(Pipe), new PropertyMetadata(default(IPipeVm), PipeVmPropertyChangedCallback));

        public static readonly DependencyProperty SegmentsProperty = DependencyProperty.Register(
            "Segments", typeof(IList<IPipeSegment>), typeof(Pipe), new PropertyMetadata(default(IReadOnlyCollection<IPipeSegment>)));

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
        public bool HasFlow
        {
            get { return (bool) GetValue(HasFlowProperty); }
            set { SetValue(HasFlowProperty, value); }
        }

        [Category("Model")]
        public SubstanceType SubstanceType
        {
            get { return (SubstanceType) GetValue(SubstanceTypeProperty); }
            set { SetValue(SubstanceTypeProperty, value); }
        }

        [Category("Model")]
        internal IPipeVm PipeVm
        {
            get { return (IPipeVm) GetValue(PipeModelProperty); }
            set { SetValue(PipeModelProperty, value); }
        }

        public IList<IPipeSegment> Segments
        {
            get { return (IList<IPipeSegment>)GetValue(SegmentsProperty); }
            set { SetValue(SegmentsProperty, value); }
        }

        #endregion

        #region Methods

        private static void PipeVmPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var pipe = (Pipe)dependencyObject;
            var model = args.NewValue as IPipeVm;

            if (model == null)
            {
                return;
            }
            pipe.Bind(() => v => v.HasFlow).To(model, () => (m, ctx) => m.HasFlow).Build();
            pipe.Bind(() => v => v.SubstanceType).To(model, () => (m, ctx) => m.SubstanceType).Build();
            pipe.Bind(() => v => v.Visibility).To(model, () => (m, ctx) => m.IsPresent ? Visibility.Visible : Visibility.Collapsed).Build();
        }

        #endregion
    }
}