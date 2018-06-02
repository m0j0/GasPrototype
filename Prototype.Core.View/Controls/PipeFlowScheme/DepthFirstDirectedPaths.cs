using System;
using System.Collections.Generic;
using System.Linq;
using Prototype.Core.Interfaces.GasPanel;
using Prototype.Core.Models.GasPanel;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    internal class DepthFirstDirectedPaths
    {
        private readonly Dictionary<PipeConnector, List<IReadOnlyList<PipeConnector>>> _paths;

        public DepthFirstDirectedPaths(PipeConnector sourceVertex)
        {
            _paths = new Dictionary<PipeConnector, List<IReadOnlyList<PipeConnector>>>();

            var vertices = new Stack<PipeConnector>();
            vertices.Push(sourceVertex);
            DepthFirstSearch(vertices);
        }

        public IReadOnlyCollection<IReadOnlyList<PipeConnector>> PathsTo(PipeConnector vertex)
        {
            return _paths.TryGetValue(vertex, out var result) ? result : null;
        }

        private void DepthFirstSearch(Stack<PipeConnector> visited)
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
                        _paths[vertex] = new List<IReadOnlyList<PipeConnector>>();
                    }

                    _paths[vertex].Add(visited.Reverse().ToArray());
                }

                DepthFirstSearch(visited);
                visited.Pop();
            }
        }
    }
}