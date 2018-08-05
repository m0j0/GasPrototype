using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Prototype.Core.Controls.PipeFlowScheme;
using Prototype.Core.Converters;
using LineSegment = Prototype.Core.Controls.PipeFlowScheme.LineSegment;

namespace Prototype.Core.Controls
{
    public sealed class Pipe : FlowControlBase, IPipe
    {
        #region Constants

        // TODO move
        internal const double PipeWidth = 5;
        private const double PipeBorderWidth = 1;
        private const double PipeSubstanceWidth = 3;

        #endregion

        #region Fields

        private IReadOnlyList<IPipeSegment> _segments;

        #endregion
        #region Constructors

        static Pipe()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Pipe), new FrameworkPropertyMetadata(typeof(Pipe)));
            SetSubscribedProperties(typeof(Pipe), OrientationProperty, VisibilityProperty, SubstanceTypeProperty,
                TypeProperty);
        }

        #endregion

        #region Dependency properties

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation", typeof(Orientation), typeof(Pipe), new PropertyMetadata(Orientation.Horizontal));

        public static readonly DependencyProperty SubstanceTypeProperty = DependencyProperty.Register(
            "SubstanceType", typeof(SubstanceType), typeof(Pipe), new PropertyMetadata(SubstanceType.Gas));

        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(
            "Type", typeof(PipeType), typeof(Pipe), new PropertyMetadata(default(PipeType)));

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

        public PipeType Type
        {
            get { return (PipeType) GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public IReadOnlyList<IPipeSegment> Segments
        {
            get => _segments;
            set
            {
                _segments = value;
                InvalidateVisual();
            }
        }

        #endregion
        
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (Segments == null || Segments.Count == 0)
            {
                return;
            }
            
            foreach (var segment in Segments)
            {
                segment.PropertyChanged += (sender, args) => InvalidateVisual();
                DrawSegment(drawingContext, segment);
            }
        }

        private void DrawSegment(DrawingContext drawingContext, IPipeSegment segment)
        {
            DrawPipeSegment(drawingContext, segment);
            switch (segment)
            {
                case LineSegment lineSegment:
                  //  DrawPipeSegment(drawingContext, lineSegment);

                    return;
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

            switch (pipeSegment.Orientation)
            {
                case Orientation.Horizontal:
                    drawingContext.DrawRectangle(borderBrush, null, new Rect(pipeSegment.StartPoint.X, pipeSegment.StartPoint.Y, pipeSegment.Length, PipeBorderWidth));
                    drawingContext.DrawRectangle(substanceBrush, null, new Rect(pipeSegment.StartPoint.X, pipeSegment.StartPoint.Y + PipeBorderWidth, pipeSegment.Length, PipeSubstanceWidth));
                    drawingContext.DrawRectangle(borderBrush, null, new Rect(pipeSegment.StartPoint.X, pipeSegment.StartPoint.Y + PipeBorderWidth + PipeSubstanceWidth, pipeSegment.Length, PipeBorderWidth));
                    break;

                case Orientation.Vertical:
                    drawingContext.DrawRectangle(borderBrush, null, new Rect(pipeSegment.StartPoint.X, pipeSegment.StartPoint.Y, PipeBorderWidth, pipeSegment.Length));
                    drawingContext.DrawRectangle(substanceBrush, null, new Rect(pipeSegment.StartPoint.X + PipeBorderWidth, pipeSegment.StartPoint.Y, PipeSubstanceWidth, pipeSegment.Length));
                    drawingContext.DrawRectangle(borderBrush, null, new Rect(pipeSegment.StartPoint.X + PipeBorderWidth + PipeSubstanceWidth, pipeSegment.StartPoint.Y, PipeBorderWidth, pipeSegment.Length));
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}