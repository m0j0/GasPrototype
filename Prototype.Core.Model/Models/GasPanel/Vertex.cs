using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MugenMvvmToolkit;
using MugenMvvmToolkit.Models;
using Prototype.Core.Interfaces.GasPanel;

namespace Prototype.Core.Models.GasPanel
{
    public abstract class VertexBase : NotifyPropertyChangedBase, IVertex
    {
        protected readonly List<IVertex> AdjacentVertices = new List<IVertex>();
        private bool _isPresent = true;

        public PipeScheme Owner { get; set; }

        public virtual bool IsPresent
        {
            get => _isPresent;
            set
            {
                if (value == _isPresent)
                {
                    return;
                }
                _isPresent = value;
                OnPropertyChanged();
            }
        }

        public virtual IReadOnlyList<IVertex> GetAdjacentVertices()
        {
            return AdjacentVertices.Where(vertex => vertex.IsPresent).ToArray();
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
        public override void Validate()
        {
            if (AdjacentVertices.Count != 1)
            {
                throw new InvalidOperationException("SourceVertex can have only one adjacent vertices");
            }
        }
    }

    public sealed class DestinationVertex : VertexBase
    {
        public override void Validate()
        {
            if (AdjacentVertices.Count > 0)
            {
                throw new InvalidOperationException("DestinationVertex can't have adjacent vertices");
            }
        }
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
                return base.GetAdjacentVertices();
            }

            return Empty.Array<IVertex>();
        }
    }
}
