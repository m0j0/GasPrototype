using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Prototype.Core.Controls.PipeFlowScheme;

namespace Prototype.Core.Controls
{
    public sealed class Pipe : Control, IPipe
    {
        #region Fields

        private static readonly EventHandler SizeChangedHandler;
        private static readonly DependencyProperty[] SubscribedProperties;

        #endregion

        #region Constructors

        static Pipe()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Pipe), new FrameworkPropertyMetadata(typeof(Pipe)));
            SizeChangedHandler = OnSizeChanged;
            SubscribedProperties = new[] {OrientationProperty, VisibilityProperty, TypeProperty};
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

        public static readonly DependencyProperty SegmentsProperty = DependencyProperty.Register(
            "Segments", typeof(IList<IPipeSegment>), typeof(Pipe), new PropertyMetadata(new List<IPipeSegment>()));

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

        public IList<IPipeSegment> Segments
        {
            get { return (IList<IPipeSegment>) GetValue(SegmentsProperty); }
            set { SetValue(SegmentsProperty, value is INotifyCollectionChanged ? value : new ObservableCollection<IPipeSegment>(value)); }
        }

        Rect IFlowControl.LayoutRect => LayoutInformation.GetLayoutSlot(this);

        bool IFlowControl.IsVisible => Visibility == Visibility.Visible;

        ISchemeContainer IFlowControl.SchemeContainer { get; set; }

        #endregion

        #region Methods

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!(Parent is ISchemeContainerOwner))
            {
                throw new InvalidOperationException("Pipe can be placed only on ISchemeContainer");
            }

            foreach (var property in SubscribedProperties)
            {
                DependencyPropertyDescriptor
                    .FromProperty(property, typeof(Pipe))
                    .AddValueChanged(this, SizeChangedHandler);
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            foreach (var property in SubscribedProperties)
            {
                DependencyPropertyDescriptor
                    .FromProperty(property, typeof(Pipe))
                    .RemoveValueChanged(this, SizeChangedHandler);
            }
        }

        private static void OnSizeChanged(object sender, EventArgs e)
        {
            var pipe = (Pipe) sender;
            pipe.SchemeChanged?.Invoke(pipe, EventArgs.Empty);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            SchemeChanged?.Invoke(this, EventArgs.Empty);

            return base.ArrangeOverride(arrangeBounds);
        }

        #endregion
    }
}