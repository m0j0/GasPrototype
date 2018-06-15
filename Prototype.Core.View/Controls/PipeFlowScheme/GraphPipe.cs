﻿using System;
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

        private readonly List<IPipeConnector> _connectors;
        private readonly ConnectorComparer _comparer;

        #endregion

        #region Constructors

        public GraphPipe(ISchemeContainer container, IPipe pipe)
        {
            Rect = Common.GetAbsoluteRect(container, pipe);
            Pipe = pipe;
            _connectors = new List<IPipeConnector>();
            _comparer = new ConnectorComparer(this);
        }

        #endregion

        #region Properties

        private IPipe Pipe { get; }

        public Rect Rect { get; }

        public Orientation Orientation => Pipe.Orientation;

        public PipeType Type => Pipe.Type;

        public FailType FailType { get; set; }

        public bool IsFailed => FailType != FailType.None;

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

        public void SetPipeSegments(IList<IPipeSegment> segments)
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

            _connectors.Sort(_comparer);
        }

        #endregion
    }
}