using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

                    default:
                        throw new NotSupportedException("Invalid segment type");
                }
            }
        }

        private static void DrawLineSegment(DrawingContext drawingContext, LineSegment segment)
        {
            var substanceBrush = segment.FailType != FailType.None
                ? new SolidColorBrush(Colors.GreenYellow)
                : GetSubstanceBrush(segment.SubstanceType, segment.HasFlow);

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

        private static Geometry CreateGeometryByPoints(string points)
        {
            var collection = PointCollection.Parse(points);
            return new PathGeometry
            {
                FillRule = FillRule.EvenOdd,
                Figures =
                {
                    new PathFigure(
                        collection[0],
                        new[]
                        {
                            new PolyLineSegment(collection.Skip(1), true)
                        }, false)
                }
            };
        }

        private static void DrawConnectorSegment(DrawingContext drawingContext, ConnectorSegment segment)
        {
            var substanceBrush = GetSubstanceBrush(segment.SubstanceType, segment.HasFlow);

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

            var substanceBrush = GetSubstanceBrush(segment.SubstanceType, segment.HasFlow);

            //
            var leftPart1 = CreateGeometryByPoints(".521 0 .521 1.84 4.28 5.923 4.5 7 8.6 7 8.485 5.456 4.561 .869 4.561 0");
            var leftPart2 = Geometry.Parse("M5,0.00165509581 C5,0.419788971 5.15803526,0.820797172 5.43933983,1.11646247 L8.26776695,4.08928214 C8.7366079,4.58205764 9,5.25040464 9,5.94729443 L9,7 L8,7 L8,5.94729443 C8,5.52916056 7.84196474,5.12815235 7.56066017,4.83248706 L4.73223305,1.85966739 C4.2633921,1.36689189 4,0.696889793 4,0 L5,0.00165509581 Z");
            var leftPart3 = Geometry.Parse("M1,0 L1,1.05270557 C1,1.47083944 1.15803526,1.87184765 1.43933983,2.16751294 L4.26776695,5.14033261 C4.7366079,5.63310811 5,6.30311021 5,7 L4,7 C4,6.58186612 3.84196474,6.17920283 3.56066017,5.88353753 L0.732233047,2.91071786 C0.263392101,2.41794236 1.83186799e-15,1.74959536 1.77635684e-15,1.05270557 L0,0 L1,0 Z");
            drawingContext.DrawGeometry(substanceBrush, null, leftPart1);
            drawingContext.DrawGeometry(PipeBorderBrush, null, leftPart2);
            drawingContext.DrawGeometry(PipeBorderBrush, null, leftPart3);
            //

            //
            drawingContext.PushTransform(new TranslateTransform { X = 4, Y = 7 });
            drawingContext.DrawRectangle(PipeBorderBrush, null, new Rect(0, 0, 1, 3 + segment.ExtraLength));
            drawingContext.DrawRectangle(substanceBrush, null, new Rect(1, 0, 3, 3 + segment.ExtraLength));
            drawingContext.DrawRectangle(PipeBorderBrush, null, new Rect(4, 0, 1, 3 + segment.ExtraLength));
            drawingContext.Pop();
            //

            //
            drawingContext.PushTransform(new TranslateTransform { X = 0, Y = 10 + segment.ExtraLength });
            var rightPart1 = CreateGeometryByPoints("4.469 0 4.469 .814 .447 5.491 .447 7 4.661 7 4.661 5.983 8.429 1.668 8.429 0");
            var rightPart2 = Geometry.Parse("M9,0 L9,1.05270557 C9,1.74959536 8.7366079,2.41794236 8.26776695,2.91071786 L5.43933983,5.88353753 C5.15803526,6.17920283 5,6.58186612 5,7 L4,7 C4,6.30311021 4.2633921,5.63310811 4.73223305,5.14033261 L7.56066017,2.16751294 C7.84196474,1.87184765 8,1.47083944 8,1.05270557 L8,0 L9,0 Z");
            var rightPart3 = Geometry.Parse("M5,0 C5,0.696889793 4.7366079,1.36689189 4.26776695,1.85966739 L1.43933983,4.83248706 C1.15803526,5.12815235 1,5.52916056 1,5.94729443 L1,7 L0,7 L0,5.94729443 C-1.11022302e-16,5.25040464 0.263392101,4.58205764 0.732233047,4.08928214 L3.56066017,1.11646247 C3.84196474,0.820797172 4,0.418133876 4,0 L5,0 Z");
            drawingContext.DrawGeometry(substanceBrush, null, rightPart1);
            drawingContext.DrawGeometry(PipeBorderBrush, null, rightPart2);
            drawingContext.DrawGeometry(PipeBorderBrush, null, rightPart3);
            drawingContext.Pop();
            //

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

        private static Brush GetSubstanceBrush(SubstanceType substanceType, bool hasFlow)
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