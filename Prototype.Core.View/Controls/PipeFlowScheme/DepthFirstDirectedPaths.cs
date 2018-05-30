using System;
using System.Collections.Generic;
using System.Linq;
using Prototype.Core.Interfaces.GasPanel;
using Prototype.Core.Models.GasPanel;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    internal class DepthFirstDirectedPaths
    {
        private readonly Dictionary<CornerConnector, List<IReadOnlyList<CornerConnector>>> _paths;

        public DepthFirstDirectedPaths(CornerConnector sourceVertex)
        {
            _paths = new Dictionary<CornerConnector, List<IReadOnlyList<CornerConnector>>>();

            var vertices = new Stack<CornerConnector>();
            vertices.Push(sourceVertex);
            DepthFirstSearch(vertices);
        }

        public IReadOnlyCollection<IReadOnlyList<CornerConnector>> PathsTo(CornerConnector vertex)
        {
            return _paths.TryGetValue(vertex, out var result) ? result : null;
        }

        private void DepthFirstSearch(Stack<CornerConnector> visited)
        {
            var vertices = visited.Peek().GetAdjacentConnectors();

            foreach (var vertex in vertices)
            {
                if (visited.Contains(vertex))
                {
                    continue;
                }

                visited.Push(vertex);

                if (vertex.IsDestination)
                {
                    if (!_paths.ContainsKey(vertex))
                    {
                        _paths[vertex] = new List<IReadOnlyList<CornerConnector>>();
                    }

                    _paths[vertex].Add(visited.Reverse().ToArray());
                }

                DepthFirstSearch(visited);
                visited.Pop();
            }
        }
    }
}