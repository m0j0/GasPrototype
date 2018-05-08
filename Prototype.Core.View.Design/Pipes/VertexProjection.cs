using MugenMvvmToolkit.Models;
using Prototype.Core.Interfaces.GasPanel;

namespace Prototype.Core.Design.Pipes
{
    public class VertexProjection : NotifyPropertyChangedBase
    {
        public VertexProjection(IVertex vertex, string propertyName)
        {
            Vertex = vertex;
            PropertyName = propertyName;
        }

        public IVertex Vertex { get; }

        public string Name => !string.IsNullOrEmpty(Vertex.Name) ? Vertex.Name : PropertyName;

        public string PropertyName { get; }
    }
}