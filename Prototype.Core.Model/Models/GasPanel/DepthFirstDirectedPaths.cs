using System.Collections.Generic;
using System.Linq;
using Prototype.Core.Interfaces.GasPanel;

namespace Prototype.Core.Models.GasPanel
{
    internal class DepthFirstDirectedPaths
    {
        private readonly Dictionary<IVertex, List<IReadOnlyList<IVertex>>> _paths;

        public DepthFirstDirectedPaths(SourceVertex sourceVertex)
        {
            _paths = new Dictionary<IVertex, List<IReadOnlyList<IVertex>>>();

            var vertices = new Stack<IVertex>();
            vertices.Push(sourceVertex);
            DepthFirstSearch(vertices);
        }

        public IReadOnlyCollection<IReadOnlyList<IVertex>> PathsTo(IVertex vertex)
        {
            return _paths.TryGetValue(vertex, out var result) ? result : null;
        }

        private void DepthFirstSearch(Stack<IVertex> visited)
        {
            var vertices = visited.Peek().GetAdjacentVertices();

            foreach (var vertex in vertices)
            {
                if (visited.Contains(vertex))
                {
                    continue;
                }

                visited.Push(vertex);

                if (vertex is DestinationVertex)
                {
                    if (!_paths.ContainsKey(vertex))
                    {
                        _paths[vertex] = new List<IReadOnlyList<IVertex>>();
                    }

                    _paths[vertex].Add(visited.Reverse().ToArray());
                }

                DepthFirstSearch(visited);
                visited.Pop();
            }
        }
    }
}