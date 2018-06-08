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
                if (_graphPipe.Direction == PipeDirection.None ||
                    _graphPipe.Direction == PipeDirection.Forward)
                {
                    var topCompare = x.Rect.Top.CompareTo(y.Rect.Top);
                    return topCompare == 0 ? x.Rect.Left.CompareTo(y.Rect.Left) : topCompare;
                }

                var leftCompare = x.Rect.Left.CompareTo(y.Rect.Left);
                return leftCompare == 0 ? x.Rect.Top.CompareTo(y.Rect.Top) : leftCompare;
            }

            #endregion
        }

        #endregion

        #region Fields

        private readonly SortedSet<IPipeConnector> _connectors;

        #endregion

        #region Constructors

        public GraphPipe(ISchemeContainer container, IPipe pipe)
        {
            Rect = Common.GetAbsoluteRect(container, pipe);
            Pipe = pipe;
            _connectors = new SortedSet<IPipeConnector>(new ConnectorComparer(this));
        }

        #endregion

        #region Properties

        public IPipe Pipe { get; }

        public Rect Rect { get; }

        public Orientation Orientation => Pipe.Orientation;

        public FailType FailType { get; set; }

        public bool IsFailed => FailType != FailType.None;

        public PipeDirection Direction { get; }

        public IEnumerable<IPipeConnector> Connectors => _connectors;

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
            
        }

        #endregion
    }
}