using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Prototype.Core.Controls.PipeFlowScheme;

namespace Prototype.Test.PipeFlowScheme
{
    internal class TestContainer : ISchemeContainer
    {
        private readonly List<IFlowControl> _controls = new List<IFlowControl>();
        
        public void Add(IFlowControl flowControl)
        {
            _controls.Add(flowControl);
        }

        public void Remove(IFlowControl flowControl)
        {
            _controls.Remove(flowControl);
        }

        public void InvalidateScheme()
        {
            throw new NotImplementedException();
        }

        public void InvalidateSchemeFlow()
        {
            throw new NotImplementedException();
        }

        public FlowGraph CreateGraph()
        {
            return new FlowGraph(_controls.OfType<IPipe>(), _controls.OfType<IValve>(), true);
        }

        public IEnumerable<TestPipe> GetPipes()
        {
            return _controls.OfType<TestPipe>();
        }

        public IEnumerable<TestValve> GetValves()
        {
            return _controls.OfType<TestValve>();
        }
    }

    internal abstract class FlowControlBase : IFlowControl
    {
        public abstract double Left { get; set; }

        public abstract double Top { get; set; }

        public abstract double Width { get; set; }

        public abstract double Height { get; set; }

        public Rect LayoutRect => new Rect(Left, Top, Width, Height);
        
        public Vector Offset { get; set; }

        public bool IsVisible { get; set; } = true;

        public ISchemeContainer SchemeContainer { get; set; }

        public void SetContrainer(ISchemeContainer container, Vector offset)
        {
            SchemeContainer = container;
            Offset = offset;
        }
    }

    internal class TestPipe : FlowControlBase, IPipe
    {
        private double? _width;
        private double? _height;

        public TestPipe(ISchemeContainer container)
        {
            SchemeContainer = container;
            Orientation = Orientation.Horizontal;
        }

        public override double Width
        {
            get
            {
                switch (Orientation)
                {
                    case Orientation.Horizontal:
                        return _width ?? Common.DefaultPipeLength;
                    case Orientation.Vertical:
                        return Common.PipeWidth;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set => _width = value;
        }

        public override double Height
        {
            get
            {
                switch (Orientation)
                {
                    case Orientation.Horizontal:
                        return Common.PipeWidth;
                    case Orientation.Vertical:
                        return _height ?? Common.DefaultPipeHeight;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set => _height = value;
        }

        public override double Top { get; set; }

        public override double Left { get; set; }

        public Orientation Orientation { get; set; }

        SubstanceType IPipe.SubstanceType { get; }

        public PipeType Type { get; set; }

        public IReadOnlyList<IPipeSegment> Segments { get; set; } = new List<IPipeSegment>();
    }

    internal class TestValve : FlowControlBase, IValve
    {
        public TestValve(ISchemeContainer container)
        {
            SchemeContainer = container;
        }

        public override double Width
        {
            get
            {
                switch (Orientation)
                {
                    case Orientation.Horizontal:
                        return 43;
                    case Orientation.Vertical:
                        return 37;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set { throw new NotImplementedException(); }
        }

        public override double Height
        {
            get
            {
                switch (Orientation)
                {
                    case Orientation.Horizontal:
                        return 37;
                    case Orientation.Vertical:
                        return 43;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set { throw new NotImplementedException(); }
        }

        public Orientation Orientation { get; set; }

        public override double Top { get; set; }

        public override double Left { get; set; }

        public bool CanPassFlow { get; set; }

        bool IValve.CanPassFlow(IFlowGraph graph, IPipeSegment pipeSegment)
        {
            return CanPassFlow;
        }
    }

    internal class TestValve3Way : FlowControlBase, IValve3Way
    {
        private readonly Valve3WayModel _model;

        public TestValve3Way(ISchemeContainer container)
        {
            SchemeContainer = container;
            _model = new Valve3WayModel(this);
        }

        public override double Width
        {
            get
            {
                switch (Rotation)
                {
                    case Rotation.Rotate0:
                    case Rotation.Rotate180:
                        return 40;
                    case Rotation.Rotate90:
                    case Rotation.Rotate270:
                        return 43;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set { throw new NotImplementedException(); }
        }

        public override double Height
        {
            get
            {
                switch (Rotation)
                {
                    case Rotation.Rotate0:
                    case Rotation.Rotate180:
                        return 43;
                    case Rotation.Rotate90:
                    case Rotation.Rotate270:
                        return 40;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set { throw new NotImplementedException(); }
        }

        public override double Top { get; set; }

        public override double Left { get; set; }

        public Valve3WayFlowPath PathWhenOpen { get; set; }

        public Valve3WayFlowPath PathWhenClosed { get; set; }

        public Rotation Rotation { get; set; }

        public bool IsOpen { get; set; }

        bool IValve.CanPassFlow(IFlowGraph graph, IPipeSegment pipeSegment)
        {
            return _model.CanPassFlow(graph, pipeSegment);
        }
    }
}