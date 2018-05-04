using System.Collections.Generic;
using System.ComponentModel;
using Prototype.Core.Models.GasPanel;

namespace Prototype.Core.Interfaces.GasPanel
{
    public interface IVertex : INotifyPropertyChanged
    {
        string Name { get; set; }

        PipeScheme Owner { get; set; }

        bool IsPresent { get; set; }

        IReadOnlyList<IVertex> GetAdjacentVertices();

        IReadOnlyList<IVertex> GetAllAdjacentVertices();

        void AddAdjacentVertex(IVertex vertex);

        void Validate();
    }
}
