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

        public FlowGraph CreateGraph()
        {
            return new FlowGraph(this, _controls.OfType<IPipe>(), _controls.OfType<IValve>());
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

    internal class TestPipe : IPipe
    {
        private double? _width;
        private double? _height;

        public TestPipe(ISchemeContainer container)
        {
            SchemeContainer = container;
            Orientation = Orientation.Horizontal;
        }

        public double Width
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

        public double Height
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

        public Rect LayoutRect => new Rect(Left, Top, Width, Height);

        public bool IsVisible { get; set; } = true;

        public ISchemeContainer SchemeContainer { get; set; }

        public double Top { get; set; }

        public double Left { get; set; }

        public Orientation Orientation { get; set; }

        SubstanceType IPipe.SubstanceType { get; }

        public PipeType Type { get; set; }

        public IList<IPipeSegment> Segments { get; set; } = new List<IPipeSegment>();

        public event EventHandler SchemeChanged;
    }

    internal class TestValve : IValve
    {
        public TestValve(ISchemeContainer container)
        {
            SchemeContainer = container;
        }

        public double Width
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
        }

        public double Height
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
        }

        public Rect LayoutRect => new Rect(Left, Top, Width, Height);

        public bool IsVisible { get; set; } = true;

        public ISchemeContainer SchemeContainer { get; set; }

        public Orientation Orientation { get; set; }

        public double Top { get; set; }

        public double Left { get; set; }

        public bool CanPassFlow { get; set; }

        public event EventHandler SchemeChanged;

        public event EventHandler StateChanged;

        bool IValve.CanPassFlow(IFlowGraph graph, IPipeSegment pipeSegment)
        {
            return CanPassFlow;
        }
    }

    internal class TestValve3Way : IValve3Way
    {
        private readonly Valve3WayModel _model;

        public TestValve3Way(ISchemeContainer container)
        {
            SchemeContainer = container;
            _model = new Valve3WayModel(this);
        }

        public double Width
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
        }

        public double Height
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
        }

        public Rect LayoutRect => new Rect(Left, Top, Width, Height);

        public bool IsVisible { get; set; } = true;

        public ISchemeContainer SchemeContainer { get; set; }

        public double Top { get; set; }

        public double Left { get; set; }

        public Valve3WayFlowPath PathWhenOpen { get; set; }

        public Valve3WayFlowPath PathWhenClosed { get; set; }

        public Rotation Rotation { get; set; }

        public bool IsOpen { get; set; }

        public event EventHandler SchemeChanged;

        public event EventHandler StateChanged;

        bool IValve.CanPassFlow(IFlowGraph graph, IPipeSegment pipeSegment)
        {
            return _model.CanPassFlow(graph, pipeSegment);
        }
    }
}