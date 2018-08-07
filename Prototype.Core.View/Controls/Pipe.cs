using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MugenMvvmToolkit;
using Prototype.Core.Controls.PipeFlowScheme;
using Prototype.Core.Converters;
using LineSegment = Prototype.Core.Controls.PipeFlowScheme.LineSegment;

namespace Prototype.Core.Controls
{
    public sealed class Pipe : FlowControlBase, IPipe
    {
        #region Fields

        private readonly PropertyChangedEventHandler _segmentPropertyChangedHandler;
        private IReadOnlyList<IPipeSegment> _segments;

        #endregion

        #region Constructors

        static Pipe()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Pipe), new FrameworkPropertyMetadata(typeof(Pipe)));
            SetSubscribedProperties(typeof(Pipe), OrientationProperty, VisibilityProperty, SubstanceTypeProperty, TypeProperty);
        }

        public Pipe()
        {
            _segmentPropertyChangedHandler = ReflectionExtensions.MakeWeakPropertyChangedHandler(this, (pipe, sender, args) => pipe.InvalidateVisual());
        }

        #endregion

        #region Dependency properties

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation", typeof(Orientation), typeof(Pipe), new PropertyMetadata(Orientation.Horizontal, PropertyChangedCallback));

        public static readonly DependencyProperty SubstanceTypeProperty = DependencyProperty.Register(
            "SubstanceType", typeof(SubstanceType), typeof(Pipe), new PropertyMetadata(SubstanceType.Gas));

        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(
            "Type", typeof(PipeType), typeof(Pipe), new PropertyMetadata(PipeType.Regular, PropertyChangedCallback));

        #endregion

        #region Properties

        [Category("Layout")]
        public Orientation Orientation
        {
            get { return (Orientation) GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value);
            }
        }

        [Category("Model")]
        public SubstanceType SubstanceType
        {
            get { return (SubstanceType) GetValue(SubstanceTypeProperty); }
            set { SetValue(SubstanceTypeProperty, value); }
        }

        [Category("Model")]
        public PipeType Type
        {
            get { return (PipeType) GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        IReadOnlyList<IPipeSegment> IPipe.Segments
        {
            get => _segments;
            set
            {
                if (_segments != null)
                {
                    foreach (var segment in _segments)
                    {
                        segment.PropertyChanged -= _segmentPropertyChangedHandler;
                    }
                }

                _segments = value;
                if (_segments != null)
                foreach (var segment in _segments)
                {
                    segment.PropertyChanged += _segmentPropertyChangedHandler;
                }

                InvalidateVisual();
            }
        }

        #endregion

        #region Methods

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (_segments == null || _segments.Count == 0)
            {
                return;
            }

            foreach (var seg in _segments)
            {
                switch (seg)
                {
                    case LineSegment segment:
                        DrawPipeSegment(drawingContext, segment);
                        continue;

                    case ConnectorSegment segment:
                        DrawPipeSegment(drawingContext, segment);
                        continue;

                    case BridgeSegment segment:
                        DrawPipeSegment(drawingContext, segment);
                        continue;

                    case FailedSegment segment:
                        DrawPipeSegment(drawingContext, segment);
                        continue;

                    default:
                        throw new NotSupportedException("Invalid segment type");
                }
            }
        }

        private static void DrawPipeSegment(DrawingContext drawingContext, IPipeSegment pipeSegment)
        {
            var borderBrush = new SolidColorBrush(Color.FromRgb(0x4D, 0x4F, 0x53));
            var substanceBrush = PipeColorConverter.GetSubstanceColor(pipeSegment.SubstanceType, pipeSegment.HasFlow);

            if (pipeSegment is FailedSegment)
            {
                substanceBrush = new SolidColorBrush(Colors.GreenYellow);
            }

/*            var vector = pipeSegment.Orientation == Orientation.Horizontal ? new Vector(pipeSegment.Length, Common.PipeWidth) : new Vector(Common.PipeWidth, pipeSegment.Length);
            drawingContext.DrawRectangle(substanceBrush, 
                new Pen(borderBrush, 1) { }, 
                new Rect(pipeSegment.StartPoint, vector) );

            return;*/
            switch (pipeSegment.Orientation)
            {
                case Orientation.Horizontal:
                    drawingContext.DrawRectangle(borderBrush, null, new Rect(pipeSegment.StartPoint.X, pipeSegment.StartPoint.Y, pipeSegment.Length, Common.PipeBorderWidth));
                    drawingContext.DrawRectangle(substanceBrush, null, new Rect(pipeSegment.StartPoint.X, pipeSegment.StartPoint.Y + Common.PipeBorderWidth, pipeSegment.Length, Common.PipeSubstanceWidth));
                    drawingContext.DrawRectangle(borderBrush, null, new Rect(pipeSegment.StartPoint.X, pipeSegment.StartPoint.Y + Common.PipeBorderWidth + Common.PipeSubstanceWidth, pipeSegment.Length, Common.PipeBorderWidth));
                    break;

                case Orientation.Vertical:
                    drawingContext.DrawRectangle(borderBrush, null, new Rect(pipeSegment.StartPoint.X, pipeSegment.StartPoint.Y, Common.PipeBorderWidth, pipeSegment.Length));
                    drawingContext.DrawRectangle(substanceBrush, null, new Rect(pipeSegment.StartPoint.X + Common.PipeBorderWidth, pipeSegment.StartPoint.Y, Common.PipeSubstanceWidth, pipeSegment.Length));
                    drawingContext.DrawRectangle(borderBrush, null, new Rect(pipeSegment.StartPoint.X + Common.PipeBorderWidth + Common.PipeSubstanceWidth, pipeSegment.StartPoint.Y, Common.PipeBorderWidth, pipeSegment.Length));
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void DrawHorizontalLine(DrawingContext drawingContext, Brush brush, double width)
        {

        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pipe = (Pipe)d;
            pipe.InvalidateVisual();
        }

        #endregion
    }
}