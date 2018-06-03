using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    internal interface IVertex
    {
        PipeConnector Connector { get; }

        IEnumerable<IVertex> GetAdjacentVertices();
    }

    internal abstract class VertexBase : IVertex
    {
        protected VertexBase(PipeConnector connector)
        {
            if (connector.Vertex != null)
            {
                throw new Exception("!!!");
            }

            Connector = connector;
            connector.Vertex = this;
        }

        public PipeConnector Connector { get; }

        public virtual IEnumerable<IVertex> GetAdjacentVertices()
        {
            foreach (var pipe in Connector.GetPipes())
            {
                foreach (var connector in pipe.Connectors.OfType<PipeConnector>())
                {
                    yield return connector.Vertex;
                }
            }
        }
    }

    internal class SourceVertex : VertexBase
    {
        public SourceVertex(PipeConnector connector) : base(connector)
        {
        }
    }

    internal class DestinationVertex : VertexBase
    {
        public DestinationVertex(PipeConnector connector) : base(connector)
        {
        }
    }

    internal class Vertex : VertexBase
    {
        public Vertex(PipeConnector connector) : base(connector)
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
    }

    internal class Edge
    {
        private readonly IVertex _startVertex;
        private readonly IVertex _endVertex;
        private readonly bool _bidirectional;

        public Edge(IVertex startVertex, IVertex endVertex, bool bidirectional)
        {
            _startVertex = startVertex;
            _endVertex = endVertex;
            _bidirectional = bidirectional;
        }
        
        public bool Equals(IVertex startVertex, IVertex endVertex)
        {
            if (_bidirectional)
            {
                return _startVertex == startVertex && _endVertex == endVertex ||
                       _endVertex == startVertex && _startVertex == endVertex;
            }

            return _startVertex == startVertex && _endVertex == endVertex;
        }
    }
}