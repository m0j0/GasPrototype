using Prototype.Core.Interfaces.GasPanel;

namespace Prototype.Core.Models.GasPanel
{
    public struct VerticesPair
    {
        public VerticesPair(IVertex startVertex, IVertex endVertex)
        {
            StartVertex = startVertex;
            EndVertex = endVertex;
        }

        public IVertex StartVertex { get; }

        public IVertex EndVertex { get; }
    }
}