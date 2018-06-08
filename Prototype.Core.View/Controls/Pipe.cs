using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using MugenMvvmToolkit.Binding;
using Prototype.Core.Controls.PipeFlowScheme;
using Prototype.Core.Interfaces.GasPanel;
using Prototype.Core.Models.GasPanel;

namespace Prototype.Core.Controls
{
    public sealed class Pipe : Control, IPipe
    {
        #region Fields

        private static readonly EventHandler SizeChangedHandler;

        #endregion

        #region Constructors

        static Pipe()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Pipe), new FrameworkPropertyMetadata(typeof(Pipe)));
            SizeChangedHandler = OnSizeChanged;
        }

        public Pipe()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        #endregion

        #region Dependency properties

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation", typeof(Orientation), typeof(Pipe), new PropertyMetadata(Orientation.Horizontal));

        public static readonly DependencyProperty SubstanceTypeProperty = DependencyProperty.Register(
            "SubstanceType", typeof(SubstanceType), typeof(Pipe), new PropertyMetadata(SubstanceType.Gas));

        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(
            "Type", typeof(PipeType), typeof(Pipe), new PropertyMetadata(default(PipeType)));

        internal static readonly DependencyProperty PipeModelProperty = DependencyProperty.Register(
            "PipeVm", typeof(IPipeVm), typeof(Pipe),
            new PropertyMetadata(default(IPipeVm), PipeVmPropertyChangedCallback));

        public static readonly DependencyProperty SegmentsProperty = DependencyProperty.Register(
            "Segments", typeof(IList<IPipeSegment>), typeof(Pipe),
            new PropertyMetadata(default(IReadOnlyCollection<IPipeSegment>)));

        #endregion

        #region Events

        public event EventHandler SchemeChanged;

        #endregion

        #region Properties

        [Category("Layout")]
        public Orientation Orientation
        {
            get { return (Orientation) GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        [Category("Model")]
        public SubstanceType SubstanceType
        {
            get { return (SubstanceType) GetValue(SubstanceTypeProperty); }
            set { SetValue(SubstanceTypeProperty, value); }
        }

        public PipeType Type
        {
            get { return (PipeType) GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        [Category("Model")]
        internal IPipeVm PipeVm
        {
            get { return (IPipeVm) GetValue(PipeModelProperty); }
            set { SetValue(PipeModelProperty, value); }
        }

        public IList<IPipeSegment> Segments
        {
            get { return (IList<IPipeSegment>) GetValue(SegmentsProperty); }
            set { SetValue(SegmentsProperty, value); }
        }

        #endregion

        #region Methods

        public ISchemeContainer GetContainer()
        {
            return (ISchemeContainer) Parent;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            DependencyPropertyDescriptor
                .FromProperty(HeightProperty, typeof(Pipe))
                .AddValueChanged(this, SizeChangedHandler);
            DependencyPropertyDescriptor
                .FromProperty(WidthProperty, typeof(Pipe))
                .AddValueChanged(this, SizeChangedHandler);
            DependencyPropertyDescriptor
                .FromProperty(OrientationProperty, typeof(Pipe))
                .AddValueChanged(this, SizeChangedHandler);
            DependencyPropertyDescriptor
                .FromProperty(VisibilityProperty, typeof(Pipe))
                .AddValueChanged(this, SizeChangedHandler);
            DependencyPropertyDescriptor
                .FromProperty(TypeProperty, typeof(Pipe))
                .AddValueChanged(this, SizeChangedHandler);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            DependencyPropertyDescriptor
                .FromProperty(HeightProperty, typeof(Pipe))
                .RemoveValueChanged(this, SizeChangedHandler);
            DependencyPropertyDescriptor
                .FromProperty(WidthProperty, typeof(Pipe))
                .RemoveValueChanged(this, SizeChangedHandler);
            DependencyPropertyDescriptor
                .FromProperty(OrientationProperty, typeof(Pipe))
                .RemoveValueChanged(this, SizeChangedHandler);
            DependencyPropertyDescriptor
                .FromProperty(VisibilityProperty, typeof(Pipe))
                .RemoveValueChanged(this, SizeChangedHandler);
            DependencyPropertyDescriptor
                .FromProperty(TypeProperty, typeof(Pipe))
                .RemoveValueChanged(this, SizeChangedHandler);
        }

        private static void OnSizeChanged(object sender, EventArgs e)
        {
            var pipe = (Pipe) sender;
            pipe.SchemeChanged?.Invoke(pipe, EventArgs.Empty);
        }

        private static void PipeVmPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs args)
        {
            var pipe = (Pipe) dependencyObject;
            var model = args.NewValue as IPipeVm;

            if (model == null)
            {
                return;
            }

            pipe.Bind(() => v => v.SubstanceType).To(model, () => (m, ctx) => m.SubstanceType).Build();
            pipe.Bind(() => v => v.Visibility)
                .To(model, () => (m, ctx) => m.IsPresent ? Visibility.Visible : Visibility.Collapsed).Build();
        }

        #endregion
    }
}