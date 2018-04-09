using Prototype.Core.Interfaces.GasPanel;

namespace Prototype.Core.Models.GasPanel
{
    internal class Edge
    {
        private readonly IVertex _startVertex;
        private readonly IVertex _endVertex;
        private readonly bool _bidirectional;

        public Edge(PipeVm pipeVm, IVertex startVertex, IVertex endVertex, bool bidirectional)
        {
            PipeVm = pipeVm;
            _startVertex = startVertex;
            _endVertex = endVertex;
            _bidirectional = bidirectional;
        }

        public PipeVm PipeVm { get; }

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