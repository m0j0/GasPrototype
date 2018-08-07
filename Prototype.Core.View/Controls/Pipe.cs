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

        private static readonly Brush PipeBorderBrush = new SolidColorBrush(Color.FromRgb(0x4D, 0x4F, 0x53));
        private static readonly Brush HasFlowBrush = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0xFF));
        private static readonly Brush GasBrush = new SolidColorBrush(Color.FromRgb(0x8B, 0x8D, 0x8E));
        private static readonly Brush PurgeBrush = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
        private static readonly Brush ChemicalBrush = new SolidColorBrush(Color.FromRgb(0xFF, 0x9B, 0xFF));

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
                {
                    foreach (var segment in _segments)
                    {
                        segment.PropertyChanged += _segmentPropertyChangedHandler;
                    }
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
                        DrawLineSegment(drawingContext, segment);
                        continue;

                    case ConnectorSegment segment:
                        DrawConnectorSegment(drawingContext, segment);
                        continue;

                    case BridgeSegment segment:
                        DrawBridgeSegment(drawingContext, segment);
                        continue;

                    case FailedSegment segment:
                        DrawLineSegment(drawingContext, segment);
                        continue;

                    default:
                        throw new NotSupportedException("Invalid segment type");
                }
            }
        }

        private static void DrawLineSegment(DrawingContext drawingContext, IPipeSegment segment)
        {
            var substanceBrush = segment is FailedSegment ? 
                new SolidColorBrush(Colors.GreenYellow) : 
                GetSubstanceColor(segment.SubstanceType, segment.HasFlow);

            switch (segment.Orientation)
            {
                case Orientation.Horizontal:
                    DrawHorizontalLine(drawingContext, PipeBorderBrush, segment.StartPoint, segment.Length, Common.PipeBorderWidth, 0);
                    DrawHorizontalLine(drawingContext, substanceBrush, segment.StartPoint, segment.Length, Common.PipeSubstanceWidth, Common.PipeBorderWidth);
                    DrawHorizontalLine(drawingContext, PipeBorderBrush, segment.StartPoint, segment.Length, Common.PipeBorderWidth, Common.PipeEndBorderOffset);
                    break;

                case Orientation.Vertical:
                    DrawVerticalLine(drawingContext, PipeBorderBrush, segment.StartPoint, segment.Length, Common.PipeBorderWidth, 0);
                    DrawVerticalLine(drawingContext, substanceBrush, segment.StartPoint, segment.Length, Common.PipeSubstanceWidth, Common.PipeBorderWidth);
                    DrawVerticalLine(drawingContext, PipeBorderBrush, segment.StartPoint, segment.Length, Common.PipeBorderWidth, Common.PipeEndBorderOffset);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DrawConnectorSegment(DrawingContext drawingContext, ConnectorSegment segment)
        {
            var substanceBrush = GetSubstanceColor(segment.SubstanceType, segment.HasFlow);

            DrawHorizontalLine(drawingContext, substanceBrush, segment.StartPoint, segment.Length, Common.PipeWidth, 0);

            if (segment.Side.HasFlagEx(Side.Left))
            {
                DrawVerticalLine(drawingContext, PipeBorderBrush, segment.StartPoint, segment.Length, Common.PipeBorderWidth, 0);
            }

            if (segment.Side.HasFlagEx(Side.Top))
            {
                DrawHorizontalLine(drawingContext, PipeBorderBrush, segment.StartPoint, segment.Length, Common.PipeBorderWidth, 0);
            }

            if (segment.Side.HasFlagEx(Side.Right))
            {
                DrawVerticalLine(drawingContext, PipeBorderBrush, segment.StartPoint, segment.Length, Common.PipeBorderWidth, Common.PipeEndBorderOffset);
            }

            if (segment.Side.HasFlagEx(Side.Bottom))
            {
                DrawHorizontalLine(drawingContext, PipeBorderBrush, segment.StartPoint, segment.Length, Common.PipeBorderWidth, Common.PipeEndBorderOffset);
            }
        }

        private void DrawBridgeSegment(DrawingContext drawingContext, BridgeSegment segment)
        {
            DrawLineSegment(drawingContext, segment);
        }

        private static void DrawHorizontalLine(DrawingContext drawingContext, Brush brush, Point startPoint, double width, double height, double heightOffset)
        {
            drawingContext.DrawRectangle(brush, null, new Rect(startPoint.X, startPoint.Y + heightOffset, width, height));
        }

        private static void DrawVerticalLine(DrawingContext drawingContext, Brush brush, Point startPoint, double height, double width, double widthOffset)
        {
            drawingContext.DrawRectangle(brush, null, new Rect(startPoint.X + widthOffset, startPoint.Y, width, height));
        }

        private static Brush GetSubstanceColor(SubstanceType substanceType, bool hasFlow)
        {
            if (hasFlow)
            {
                return HasFlowBrush;
            }

            switch (substanceType)
            {
                case SubstanceType.Gas:
                    return GasBrush;
                case SubstanceType.Purge:
                    return PurgeBrush;
                case SubstanceType.Chemical:
                    return ChemicalBrush;
                default:
                    throw new ArgumentOutOfRangeException(nameof(substanceType));
            }
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pipe = (Pipe)d;
            pipe.InvalidateVisual();
        }

        #endregion
    }
}