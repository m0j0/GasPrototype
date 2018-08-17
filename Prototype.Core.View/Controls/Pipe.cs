using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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

        private static readonly Geometry BridgeSegmentLeftGeometry1;
        private static readonly Geometry BridgeSegmentLeftGeometry2;
        private static readonly Geometry BridgeSegmentLeftGeometry3;
        private static readonly Geometry BridgeSegmentRightGeometry1;
        private static readonly Geometry BridgeSegmentRightGeometry2;
        private static readonly Geometry BridgeSegmentRightGeometry3;
        private static readonly Transform HorizontalBridgeTransform;

        private readonly PropertyChangedEventHandler _segmentPropertyChangedHandler;
        private IReadOnlyList<IPipeSegment> _segments;

        #endregion

        #region Constructors

        static Pipe()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Pipe), new FrameworkPropertyMetadata(typeof(Pipe)));
            SetSubscribedProperties(typeof(Pipe), OrientationProperty, SubstanceTypeProperty, TypeProperty);

            BridgeSegmentLeftGeometry1 = CreateGeometryByPoints(".521 0 .521 1.84 4.28 5.923 4.5 7 8.6 7 8.485 5.456 4.561 .869 4.561 0");
            BridgeSegmentLeftGeometry2 = Geometry.Parse(
                "M5,0.00165509581 C5,0.419788971 5.15803526,0.820797172 5.43933983,1.11646247 L8.26776695,4.08928214 C8.7366079,4.58205764 9,5.25040464 9,5.94729443 L9,7 L8,7 L8,5.94729443 C8,5.52916056 7.84196474,5.12815235 7.56066017,4.83248706 L4.73223305,1.85966739 C4.2633921,1.36689189 4,0.696889793 4,0 L5,0.00165509581 Z");
            BridgeSegmentLeftGeometry3 = Geometry.Parse(
                "M1,0 L1,1.05270557 C1,1.47083944 1.15803526,1.87184765 1.43933983,2.16751294 L4.26776695,5.14033261 C4.7366079,5.63310811 5,6.30311021 5,7 L4,7 C4,6.58186612 3.84196474,6.17920283 3.56066017,5.88353753 L0.732233047,2.91071786 C0.263392101,2.41794236 1.83186799e-15,1.74959536 1.77635684e-15,1.05270557 L0,0 L1,0 Z");

            BridgeSegmentRightGeometry1 = CreateGeometryByPoints("4.469 0 4.469 .814 .447 5.491 .447 7 4.661 7 4.661 5.983 8.429 1.668 8.429 0");
            BridgeSegmentRightGeometry2 = Geometry.Parse(
                "M9,0 L9,1.05270557 C9,1.74959536 8.7366079,2.41794236 8.26776695,2.91071786 L5.43933983,5.88353753 C5.15803526,6.17920283 5,6.58186612 5,7 L4,7 C4,6.30311021 4.2633921,5.63310811 4.73223305,5.14033261 L7.56066017,2.16751294 C7.84196474,1.87184765 8,1.47083944 8,1.05270557 L8,0 L9,0 Z");
            BridgeSegmentRightGeometry3 = Geometry.Parse(
                "M5,0 C5,0.696889793 4.7366079,1.36689189 4.26776695,1.85966739 L1.43933983,4.83248706 C1.15803526,5.12815235 1,5.52916056 1,5.94729443 L1,7 L0,7 L0,5.94729443 C-1.11022302e-16,5.25040464 0.263392101,4.58205764 0.732233047,4.08928214 L3.56066017,1.11646247 C3.84196474,0.820797172 4,0.418133876 4,0 L5,0 Z");

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

            var isFailed = segment.FailType != FailType.None;
            var substanceBrush = isFailed
                ? new SolidColorBrush(Colors.GreenYellow)
                : GetSubstanceBrush(segment.SubstanceType, segment.HasFlow);

            if (isFailed)
            {
                DrawFailTypeText();
            }

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

            void DrawFailTypeText()
            {
                switch (segment.Orientation)
                {
                    case Orientation.Horizontal:
                        drawingContext.PushTransform(new TranslateTransform { Y = -15 });
                        break;
                    case Orientation.Vertical:
                        drawingContext.PushTransform(new TransformGroup
                        {
                            Children =
                            {
                                new RotateTransform {Angle = 90},
                                new TranslateTransform {X = 20}
                            }
                        });
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                drawingContext.DrawText(
                    new FormattedText(segment.FailType.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                        new Typeface(SystemFonts.MessageFontFamily, FontStyles.Normal, FontWeights.SemiBold, FontStretches.Normal), 11,
                        new SolidColorBrush(Colors.Black)), segment.StartPoint);

                drawingContext.Pop();
            }
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

            // left (upper) part
            drawingContext.DrawGeometry(substanceBrush, null, BridgeSegmentLeftGeometry1);
            drawingContext.DrawGeometry(PipeBorderBrush, null, BridgeSegmentLeftGeometry2);
            drawingContext.DrawGeometry(PipeBorderBrush, null, BridgeSegmentLeftGeometry3);
            //

            // center part
            drawingContext.PushTransform(new TranslateTransform {X = 4, Y = 7});
            drawingContext.DrawRectangle(PipeBorderBrush, null, new Rect(0, 0, 1, 3 + segment.ExtraLength));
            drawingContext.DrawRectangle(substanceBrush, null, new Rect(1, 0, 3, 3 + segment.ExtraLength));
            drawingContext.DrawRectangle(PipeBorderBrush, null, new Rect(4, 0, 1, 3 + segment.ExtraLength));
            drawingContext.Pop();
            //

            // right (bottom) part
            drawingContext.PushTransform(new TranslateTransform {X = 0, Y = 10 + segment.ExtraLength});
            drawingContext.DrawGeometry(substanceBrush, null, BridgeSegmentRightGeometry1);
            drawingContext.DrawGeometry(PipeBorderBrush, null, BridgeSegmentRightGeometry2);
            drawingContext.DrawGeometry(PipeBorderBrush, null, BridgeSegmentRightGeometry3);
            drawingContext.Pop();
            //

            if (segment.Orientation == Orientation.Horizontal)
            {
                drawingContext.Pop();
            }

            drawingContext.Pop();
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
                        },
                        false)
                }
            };
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pipe = (Pipe) d;
            pipe.InvalidateVisual();
        }

        #endregion
    }
}