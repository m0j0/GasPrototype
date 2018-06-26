using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Prototype.Core.Controls.PipeFlowScheme;

namespace Prototype.Test.PipeFlowScheme
{
    internal class TestContainer : ISchemeContainer
    {
        private readonly List<IFlowControl> _controls = new List<IFlowControl>();

        public double GetTop(IFlowControl control)
        {
            switch (control)
            {
                case TestPipe pipe:
                    return pipe.Top;
                case TestValve valve:
                    return valve.Top;
                default:
                    throw new ArgumentException();
            }
        }

        public double GetLeft(IFlowControl control)
        {
            switch (control)
            {
                case TestPipe pipe:
                    return pipe.Left;
                case TestValve valve:
                    return valve.Left;
                default:
                    throw new ArgumentException();
            }
        }

        public event EventHandler SchemeChanged;

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
        private readonly ISchemeContainer _container;
        private double? _width;
        private double? _height;

        public TestPipe(ISchemeContainer container)
        {
            _container = container;
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

        public bool IsVisible { get; set; } = true;

        public double Top { get; set; }

        public double Left { get; set; }

        public Orientation Orientation { get; set; }

        public PipeType Type { get; set; }

        public IList<IPipeSegment> Segments { get; set; }

        public event EventHandler SchemeChanged;

        public ISchemeContainer GetContainer()
        {
            return _container;
        }
    }

    internal class TestValve : IValve
    {
        private readonly ISchemeContainer _container;

        public TestValve(ISchemeContainer container)
        {
            _container = container;
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

        public bool IsVisible { get; set; } = true;

        public Orientation Orientation { get; set; }

        public double Top { get; set; }

        public double Left { get; set; }

        public bool CanPassFlow { get; set; }

        public event EventHandler SchemeChanged;

        public event EventHandler StateChanged;

        public ISchemeContainer GetContainer()
        {
            return _container;
        }

        bool IValve.CanPassFlow(IFlowGraph graph, IPipeSegment pipeSegment)
        {
            return CanPassFlow;
        }
    }
}