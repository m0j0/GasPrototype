using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MugenMvvmToolkit;
using Prototype.Core.Interfaces.GasPanel;

namespace Prototype.Core.Models.GasPanel
{
    public abstract class VertexBase : IVertex
    {
        protected readonly List<IVertex> AdjacentVertices = new List<IVertex>();

        public PipeScheme Owner { get; set; }

        public virtual IReadOnlyList<IVertex> GetAdjacentVertices()
        {
            return AdjacentVertices;
        }

        public IReadOnlyList<IVertex> GetAllAdjacentVertices()
        {
            return AdjacentVertices;
        }

        public void AddAdjacentVertex(IVertex vertex)
        {
            AdjacentVertices.Add(vertex);
        }

        public virtual void Validate()
        {
        }
    }

    public sealed class SourceVertex : VertexBase
    {
    }

    public sealed class DestinationVertex : VertexBase
    {
    }

    public sealed class Vertex : VertexBase
    {
    }

    [DebuggerDisplay("ValveVm = {ValveVm.Name}")]
    public class ValveVertex : VertexBase
    {
        public ValveVertex(IValveVm valveVm)
        {
            ValveVm = valveVm;
        }

        public IValveVm ValveVm { get; }

        public override IReadOnlyList<IVertex> GetAdjacentVertices()
        {
            if (ValveVm.State == ValveState.Opened)
            {
                return AdjacentVertices;
            }

            return Empty.Array<IVertex>();
        }
    }
}
