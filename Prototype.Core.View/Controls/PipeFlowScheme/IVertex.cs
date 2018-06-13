using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    internal interface IVertex
    {
        IPipeConnector Connector { get; }

        IList<IPipeSegment> PipeSegments { get; }

        IEnumerable<IVertex> GetAdjacentVertices();

        IEnumerable<IVertex> GetAllAdjacentVertices();

        void AddAdjacentVertex(IVertex vertex);
    }

    internal abstract class VertexBase : IVertex
    {
        protected readonly List<IVertex> AdjacentVertices = new List<IVertex>();

        protected VertexBase(IPipeConnector connector)
        {
            Connector = connector;
            PipeSegments = new List<IPipeSegment>();
        }

        public IPipeConnector Connector { get; }

        public IList<IPipeSegment> PipeSegments { get; }

        public virtual IEnumerable<IVertex> GetAdjacentVertices()
        {
            return AdjacentVertices;
        }

        public IEnumerable<IVertex> GetAllAdjacentVertices()
        {
            return AdjacentVertices;
        }

        public void AddAdjacentVertex(IVertex vertex)
        {
            AdjacentVertices.Add(vertex);
        }

        public override string ToString()
        {
            return GetType().Name + " " + Connector.Rect;
        }
    }

    internal class SourceVertex : VertexBase
    {
        public SourceVertex(IPipeConnector connector) : base(connector)
        {
        }
    }

    internal class DestinationVertex : VertexBase
    {
        public DestinationVertex(IPipeConnector connector) : base(connector)
        {
        }
    }

    internal class Vertex : VertexBase
    {
        public Vertex(IPipeConnector connector) : base(connector)
        {
        }

        public IValve Valve { get; set; }

        public override IEnumerable<IVertex> GetAdjacentVertices()
        {
            if (Valve != null && !Valve.CanPassFlow(null, null))
            {
                return Enumerable.Empty<IVertex>();
            }

            return base.GetAdjacentVertices();
        }

        public override string ToString()
        {
            return GetType().Name + " " + Connector.Rect + (Valve == null ? "" : " Valve");
        }
    }

    internal class Edge
    {

        public Edge(IVertex startVertex, IVertex endVertex)
        {
            StartVertex = startVertex;
            EndVertex = endVertex;
            IsBidirectional = !(startVertex is SourceVertex || endVertex is DestinationVertex);
        }

        public IVertex StartVertex { get; }
        public IVertex EndVertex { get; }
        public bool IsBidirectional { get; }

        public IPipeSegment PipeSegment { get; set; }

        public bool Equals(IVertex startVertex, IVertex endVertex)
        {
            if (IsBidirectional)
            {
                return StartVertex == startVertex && EndVertex == endVertex ||
                       EndVertex == startVertex && StartVertex == endVertex;
            }

            return StartVertex == startVertex && EndVertex == endVertex;
        }


        public override string ToString()
        {
            return $"Start: {StartVertex} End: {EndVertex} Bi: {IsBidirectional}";
        }
    }
}