using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MugenMvvmToolkit;
using Prototype.Core.Controls.PipeFlowScheme;
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

        private static readonly Dictionary<Brush, Pen> BridgeSubstancePens = new Dictionary<Brush, Pen>();
        private static readonly Geometry BridgeSegmentGeometry1;
        private static readonly Geometry BridgeSegmentGeometry2;
        private static readonly Geometry BridgeSegmentGeometry3;
        private static readonly Transform HorizontalBridgeTransform;

        private readonly PropertyChangedEventHandler _segmentPropertyChangedHandler;
        private IReadOnlyList<IPipeSegment> _segments;

        #endregion

        #region Constructors

        static Pipe()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Pipe), new FrameworkPropertyMetadata(typeof(Pipe)));
            SetSubscribedProperties(typeof(Pipe), OrientationProperty, SubstanceTypeProperty, TypeProperty);

            BridgeSegmentGeometry1 = new PathGeometry
            {
                FillRule = FillRule.EvenOdd,
                Figures =
                {
                    new PathFigure(
                        new Point(2.5, 0),
                        new[]
                        {
                            new PolyLineSegment(new[]
                            {
                                new Point(2.5, 6),
                                new Point(6.5, 11),
                                new Point(6.5, 16),
                                new Point(2.5, 21),
                                new Point(2.5, 27),
                            }, true)
                        }, false)
                }
            };
            BridgeSegmentGeometry2 =
                Geometry.Parse(
                    "M4 0 L5 0 L5 5.17157288 C5 5.56939761 5.15803526 5.95092848 5.43933983 6.23223305 L8.26776695 9.06066017 " +
                    "C8.7366079 9.52950112 9 10.1653859 9 10.8284271 L9 16.1715729 C9 16.8346141 8.7366079 17.4704989 8.26776695 17.9393398 " +
                    "L5.43933983 20.767767 C5.15803526 21.0490715 5 21.4306024 5 21.8284271 L5 27 L4 27 L4 21.8284271 " +
                    "C4 21.1653859 4.2633921 20.5295011 4.73223305 20.0606602 L7.56066017 17.232233 C7.84196474 16.9509285 8 16.5693976 8 16.1715729 " +
                    "L8 10.8284271 C8 10.4306024 7.84196474 10.0490715 7.56066017 9.76776695 L4.73223305 6.93933983 C4.2633921 6.47049888 4 5.8346141 4 5.17157288 L4 0 Z");
            BridgeSegmentGeometry3 =
                Geometry.Parse(
                    "M0 0 L1 0 L1 6.17157288 C1 6.56939761 1.15803526 6.95092848 1.43933983 7.23223305 L4.26776695 10.0606602 " +
                    "C4.7366079 10.5295011 5 11.1653859 5 11.8284271 L5 15.1715729 C5 15.8346141 4.7366079 16.4704989 4.26776695 16.9393398 " +
                    "L1.43933983 19.767767 C1.15803526 20.0490715 1 20.4306024 1 20.8284271 L1 27 L0 27 L0 20.8284271 " +
                    "C-1.11022302e-16 20.1653859 0.263392101 19.5295011 0.732233047 19.0606602 L3.56066017 16.232233 " +
                    "C3.84196474 15.9509285 4 15.5693976 4 15.1715729 L4 11.8284271 C4 11.4306024 3.84196474 11.0490715 3.56066017 10.767767 " +
                    "L0.732233047 7.93933983 C0.263392101 7.47049888 1.83186799e-15 6.8346141 1.77635684e-15 6.17157288 L0 0 Z");
            HorizontalBridgeTransform = new TransformGroup
            {
                Children =
                {
                    new RotateTransform {Angle = -90},
                    new TranslateTransform {Y = Common.PipeWidth}
                }
            };
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
            set { SetValue(OrientationProperty, value); }
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
            var substanceBrush = segment is FailedSegment ? new SolidColorBrush(Colors.GreenYellow) : GetSubstanceColor(segment.SubstanceType, segment.HasFlow);

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

        private static void DrawConnectorSegment(DrawingContext drawingContext, ConnectorSegment segment)
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

        private static void DrawBridgeSegment(DrawingContext drawingContext, BridgeSegment segment)
        {
            drawingContext.PushTransform(new TranslateTransform {X = segment.StartPoint.X, Y = segment.StartPoint.Y});

            if (segment.Orientation == Orientation.Horizontal)
            {
                drawingContext.PushTransform(HorizontalBridgeTransform);
            }

            drawingContext.DrawGeometry(null, GetBridgeSegmentSubstancePen(GetSubstanceColor(segment.SubstanceType, segment.HasFlow)), BridgeSegmentGeometry1);
            drawingContext.DrawGeometry(PipeBorderBrush, null, BridgeSegmentGeometry2);
            drawingContext.DrawGeometry(PipeBorderBrush, null, BridgeSegmentGeometry3);

            drawingContext.Pop();

            if (segment.Orientation == Orientation.Horizontal)
            {
                drawingContext.Pop();
            }
        }

        private static void DrawHorizontalLine(DrawingContext drawingContext, Brush brush, Point startPoint, double width, double height, double heightOffset)
        {
            drawingContext.DrawRectangle(brush, null, new Rect(startPoint.X, startPoint.Y + heightOffset, width, height));
        }

        private static void DrawVerticalLine(DrawingContext drawingContext, Brush brush, Point startPoint, double height, double width, double widthOffset)
        {
            drawingContext.DrawRectangle(brush, null, new Rect(startPoint.X + widthOffset, startPoint.Y, width, height));
        }

        private static Pen GetBridgeSegmentSubstancePen(Brush brush)
        {
            if (!BridgeSubstancePens.TryGetValue(brush, out Pen pen))
            {
                pen = new Pen(brush, 4);
                BridgeSubstancePens[brush] = pen;
            }

            return pen;
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
            var pipe = (Pipe) d;
            pipe.InvalidateVisual();
        }

        #endregion
    }
}