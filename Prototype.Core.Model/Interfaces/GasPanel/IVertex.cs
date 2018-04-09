using System.Collections.Generic;
using Prototype.Core.Models.GasPanel;

namespace Prototype.Core.Interfaces.GasPanel
{
    public interface IVertex
    {
        PipeScheme Owner { get; set; }

        IReadOnlyList<IVertex> GetAdjacentVertices();

        IReadOnlyList<IVertex> GetAllAdjacentVertices();

        void AddAdjacentVertex(IVertex vertex);

        void Validate();
    }
}
