using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MugenMvvmToolkit.Binding;
using Prototype.Core.Interfaces;
using Prototype.Core.Interfaces.GasPanel;
using Prototype.Core.Models;
using Prototype.Core.Models.GasPanel;

namespace Prototype.Core.Controls
{
    public sealed class Pipe : Control
    {
        #region Constants

        internal const double PipeWidth = 5;
        private const double PipeBorderWidth = 1;
        private const double PipeSubstanceWidth = 3;
        
        #endregion

        #region Constructors

        public Pipe()
        {
            UpdateSizeConstraints();
        }

        #endregion

        #region Dependency properties

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation", typeof(Orientation), typeof(Pipe), new PropertyMetadata(Orientation.Horizontal, OrientationPropertyChangedCallback));

        public static readonly DependencyProperty HasFlowProperty = DependencyProperty.Register(
            "HasFlow", typeof(bool), typeof(Pipe), new PropertyMetadata(default(bool), VisualPropertyChangedCallback));

        public static readonly DependencyProperty SubstanceTypeProperty = DependencyProperty.Register(
            "SubstanceType", typeof(SubstanceType), typeof(Pipe), new PropertyMetadata(SubstanceType.Gas, VisualPropertyChangedCallback));

        internal static readonly DependencyProperty PipeModelProperty = DependencyProperty.Register(
            "PipeVm", typeof(IPipeVm), typeof(Pipe), new PropertyMetadata(default(IPipeVm), PipeVmPropertyChangedCallback));

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

        #endregion

        #region Methods

        protected override void OnRender(DrawingContext drawingContext)
        {
            var canvas = Parent as Canvas;
            if (canvas == null)
            {
                throw new NotSupportedException("Pipe should be located on Canvas");
            }

            var allPipes = canvas.Children
                .OfType<Pipe>()
                .Select(pipe => new PipeControlModel(pipe))
                .ToArray();
            PipeEx.FindAllIntersections(allPipes);
            
            var segments = PipeEx.SplitPipeToSegments(this, allPipes);

            var substanceBrush = GetSubstanceBrush(this);
            var borderBrush = GetBorderBrush(this);

            foreach (var segment in segments)
            {
                DrawPipeSegment(drawingContext, borderBrush, substanceBrush, segment);
            }
        }

        private static void DrawPipeSegment(DrawingContext drawingContext, Brush borderBrush, Brush substanceBrush, IPipeSegment pipeSegment)
        {
            //if (pipeSegment is PipeIntersectionSegment)
            //{
            //    borderBrush = new SolidColorBrush(Colors.Red);
            //    substanceBrush = new SolidColorBrush(Colors.Red);
            //}
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

        private void UpdateSizeConstraints()
        {
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    Width = 100;
                    MinWidth = 0;
                    MaxWidth = Double.PositiveInfinity;
                    Height = PipeWidth;
                    MinHeight = PipeWidth;
                    MaxHeight = PipeWidth;
                    break;

                case Orientation.Vertical:
                    Width = PipeWidth;
                    MinWidth = PipeWidth;
                    MaxWidth = PipeWidth;
                    Height = 100;
                    MinHeight = 0;
                    MaxHeight = Double.PositiveInfinity;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

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
        }

        private static void VisualPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var pipe = (Pipe)dependencyObject;
            pipe.InvalidateVisual();
        }

        private static void OrientationPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var pipe = (Pipe)dependencyObject;
            pipe.UpdateSizeConstraints();
        }

        private static SolidColorBrush GetBorderBrush(Pipe pipe)
        {
            return new SolidColorBrush(Color.FromRgb(77, 79, 83));
        }

        private static SolidColorBrush GetSubstanceBrush(Pipe pipe)
        {
            Color color;
            switch (pipe.SubstanceType)
            {
                case SubstanceType.Gas:
                    color = pipe.HasFlow ? Color.FromRgb(0, 0, 255) : Color.FromRgb(139, 141, 142);
                    break;

                case SubstanceType.Purge:
                    color = pipe.HasFlow ? Color.FromRgb(128, 128, 128) : Color.FromRgb(255, 255, 255);
                    break;

                case SubstanceType.Chemical:
                    color = pipe.HasFlow ? Color.FromRgb(168, 0, 168) : Color.FromRgb(255, 155, 255);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new SolidColorBrush(color);
        }

        #endregion
    }
}