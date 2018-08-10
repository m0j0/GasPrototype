using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MugenMvvmToolkit;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    internal class GraphPipe
    {
        #region Nested types

        private class ConnectorComparer : IComparer<IPipeConnector>
        {
            #region Fields

            private readonly GraphPipe _graphPipe;

            #endregion

            #region Constructors

            public ConnectorComparer(GraphPipe graphPipe)
            {
                _graphPipe = graphPipe;
            }

            #endregion

            #region Methods

            public int Compare(IPipeConnector x, IPipeConnector y)
            {
                if (x == null || y == null)
                {
                    throw new ArgumentException();
                }

                var topCompare = x.Rect.Top.CompareTo(y.Rect.Top);
                var leftCompare = x.Rect.Left.CompareTo(y.Rect.Left);
                var result = topCompare == 0 ? leftCompare : topCompare;

                return _graphPipe.Direction == PipeDirection.None ||
                       _graphPipe.Direction == PipeDirection.Forward
                    ? result
                    : -result;
            }

            #endregion
        }

        #endregion

        #region Fields

        private readonly List<IPipeConnector> _connectors;
        private readonly ConnectorComparer _comparer;

        #endregion

        #region Constructors

        public GraphPipe(IPipe pipe)
        {
            Rect = new Rect(pipe.LayoutRect.Location + pipe.Offset, pipe.LayoutRect.Size);
            IsVisible = pipe.IsVisible;
            Type = pipe.Type;
            Pipe = pipe;
            _connectors = new List<IPipeConnector>();
            _comparer = new ConnectorComparer(this);
        }

        #endregion

        #region Properties

        public IPipe Pipe { get; }

        public Rect Rect { get; }

        public Orientation Orientation => Pipe.Orientation;

        public PipeType Type { get; }

        public FailType FailType { get; set; }

        public bool IsFailed => FailType != FailType.None;

        public bool IsVisible { get; }

        public PipeDirection Direction { get; private set; }

        public IReadOnlyList<IPipeConnector> Connectors => _connectors;

        public PipeConnector StartConnector { get; private set; }

        public PipeConnector EndConnector { get; private set; }

        #endregion

        #region Methods

        public void AddConnector(IPipeConnector connector)
        {
            Should.NotBeNull(connector, nameof(connector));
            _connectors.Add(connector);

            if (Rect.TopLeft == connector.Rect.TopLeft)
            {
                StartConnector = (PipeConnector)connector;
            }
            if (Rect.BottomRight == connector.Rect.BottomRight)
            {
                EndConnector = (PipeConnector)connector;
            }

            UpdateDirection();
        }

        public void SetPipeSegments(IReadOnlyList<IPipeSegment> segments)
        {
            Pipe.Segments = segments;
        }

        public void CreateEndsConnectors()
        {
            bool hasStartConnector = StartConnector != null;
            bool hasEndConnector = EndConnector != null;

            bool isSource = Type == PipeType.Source;
            bool isDestination = Type == PipeType.Destination;

            if ((isSource || isDestination) &&
                (!hasStartConnector && !hasEndConnector || hasStartConnector && hasEndConnector))
            {
                FailType = FailType.BothSourceDestination;
                return;
            }

            if (!hasStartConnector)
            {
                var connector = new PipeConnector(new Rect(Rect.TopLeft, Common.ConnectorVector));
                connector.AddPipe(this);
                StartConnector = connector;
            }

            if (!hasEndConnector)
            {
                var connector = new PipeConnector(new Rect(Rect.BottomRight - Common.ConnectorVector, Common.ConnectorVector));
                connector.AddPipe(this);
                EndConnector = connector;
            }

            UpdateDirection();
        }

        public bool Equals(IPipe pipe)
        {
            if (Pipe != pipe)
            {
                return false;
            }

            if (IsVisible != pipe.IsVisible)
            {
                return false;
            }

            if (Type != pipe.Type)
            {
                return false;
            }

            if (Rect != pipe.LayoutRect)
            {
                return false;
            }

            return true;
        }

        private void UpdateDirection()
        {
            if (StartConnector == null || 
                EndConnector == null)
            {
                return;
            }

            if (Type == PipeType.Source)
            {
                if (StartConnector.IsEndConnector())
                {
                    Direction = PipeDirection.Forward;
                }
                else if (EndConnector.IsEndConnector())
                {
                    Direction = PipeDirection.Backward;
                }
                else
                {
                    throw new Exception("! ! !");
                }
            }
            else if (Type == PipeType.Destination)
            {
                if (StartConnector.IsEndConnector())
                {
                    Direction = PipeDirection.Backward;
                }
                else if (EndConnector.IsEndConnector())
                {
                    Direction = PipeDirection.Forward;
                }
                else
                {
                    throw new Exception("! ! !");
                }
            }
            
            if (Direction == PipeDirection.Backward)
            {
                var tmp = StartConnector;
                StartConnector = EndConnector;
                EndConnector = tmp;
            }

            _connectors.Sort(_comparer);
        }

        #endregion
    }
}