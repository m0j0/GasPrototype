using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    public interface IFlowGraph
    {
        IPipe FindPipe(IPipeSegment segment);

        Rect GetAbsoluteRect(IPipe pipe);

        Rect GetAbsoluteRect(IValve valve);
    }

    internal class FlowGraph : IFlowGraph
    {
        private bool _isSchemeFailed;
        private IReadOnlyCollection<IVertex> _vertices;
        private IReadOnlyCollection<Edge> _edges;

        private IReadOnlyCollection<GraphPipe> _pipes;
        private IReadOnlyCollection<GraphValve> _valves;

        public FlowGraph(ISchemeContainer container, IEnumerable<IPipe> pipes,
            IEnumerable<IValve> valves)
        {
            SplitPipesToSegments(container, pipes, valves);
            
            InvalidateFlow();
        }

        public IPipe FindPipe(IPipeSegment segment)
        {
            foreach (var pipe in _pipes)
            {
                if (pipe.Pipe.Segments.Contains(segment))
                {
                    return pipe.Pipe;
                }
            }

            return null;
        }

        public Rect GetAbsoluteRect(IPipe pipe)
        {
            return _pipes.Single(p => p.Pipe == pipe).Rect;
        }

        public Rect GetAbsoluteRect(IValve valve)
        {
            return _valves.Single(v => v.Valve == valve).Rect;
        }

        private void SplitPipesToSegments(ISchemeContainer container, IEnumerable<IPipe> pipeControls,
            IEnumerable<IValve> valveControls)
        {
            var pipes = new List<GraphPipe>();
            foreach (var pipeControl in pipeControls)
            {
                pipeControl.Segments?.Clear();
                if (!pipeControl.IsVisible)
                {
                    continue;
                }

                pipes.Add(new GraphPipe(container, pipeControl));
            }
            var valves = valveControls.Where(v => v.IsVisible).Select(v => new GraphValve(container, v)).ToArray();

            _pipes = pipes.ToArray();
            _valves = valves.ToArray();

            var connectors = new List<IPipeConnector>();
            var cnnToVertex = new Dictionary<IPipeConnector, IVertex>();
            var edges = new List<Edge>();

            foreach (var pipe1 in pipes)
            {
                if (!Common.IsSizeValid(pipe1))
                {
                    pipe1.FailType = FailType.WrongSize;
                    continue;
                }

                foreach (var pipe2 in pipes)
                {
                    if (pipe1 == pipe2 ||
                        pipe2.IsFailed)
                    {
                        continue;
                    }

                    var intersectionRect = Common.FindIntersection(
                        pipe1.Rect,
                        pipe2.Rect
                    );

                    if (!Common.IsIntersectionSizeValid(intersectionRect))
                    {
                        if (!intersectionRect.IsEmpty)
                        {
                            pipe1.FailType = FailType.IntersectionIsNotSupported;
                            pipe2.FailType = FailType.IntersectionIsNotSupported;
                        }

                        continue;
                    }

                    var existingConnector = connectors.FirstOrDefault(c => c.Rect == intersectionRect);

                    if (Common.IsBridgeConnection(pipe1, pipe2, intersectionRect) ||
                        Common.IsBridgeConnection(pipe2, pipe1, intersectionRect))
                    {
                        if (existingConnector != null)
                        {
                            continue;
                        }

                        var bridgeConnector = new BridgePipeConnector(intersectionRect, pipe2);
                        connectors.Add(bridgeConnector);
                        cnnToVertex[bridgeConnector] = new Vertex(this, bridgeConnector);

                        continue;
                    }
                    
                    var connector = (PipeConnector) existingConnector ??
                                    new PipeConnector(intersectionRect);
                    connector.AddPipe(pipe1);
                    connector.AddPipe(pipe2);
                    if (existingConnector == null)
                    {
                        connectors.Add(connector);
                        cnnToVertex[connector] = new Vertex(this, connector);
                    }
                }

            }
            
            foreach (var pipe in pipes)
            {
                if (pipe.IsFailed)
                {
                    continue;
                }

                pipe.CreateEndsConnectors();
                
                if (pipe.IsFailed)
                {
                    continue;
                }

                AddEndVertex(pipe, pipe.StartConnector, connectors, cnnToVertex);
                AddEndVertex(pipe, pipe.EndConnector, connectors, cnnToVertex);
            }

            foreach (var pipe in pipes)
            {
                if (pipe.IsFailed)
                {
                    continue;
                }
                
                foreach (var valve in valves)
                {
                    if (valve.Connector != null)
                    {
                        continue;
                    }

                    foreach (var pipeConnector in pipe.Connectors)
                    {
                        if (!Common.IsIntersect(pipeConnector.Rect, valve.Rect))
                        {
                            continue;
                        }

                        var vertex = cnnToVertex[pipeConnector] as Vertex;
                        if (vertex == null)
                        {
                            continue; // TODO !! !!
                        }
                        if (vertex.Valve != null)
                        {
                            throw new InvalidOperationException();
                        }
                        vertex.Valve = valve.Valve;
                        valve.Connector = pipeConnector;
                        break;
                    }

                    if (valve.Connector != null)
                    {
                        continue;
                    }
                    
                    var intersectionRect = Common.FindIntersection(
                        pipe.Rect,
                        valve.Rect
                    );

                    if (intersectionRect.IsEmpty)
                    {
                        continue;
                    }

                    var connector = new PipeConnector(intersectionRect);
                    connector.AddPipe(pipe);
                    cnnToVertex[connector] = new Vertex(this, connector) {Valve = valve.Valve};
                }
            }

            foreach (var pipe in pipes)
            {
                if (pipe.IsFailed)
                {
                    continue;
                }
                
                if (pipe.Connectors.Count < 2)
                {
                    throw new Exception("!!!");
                }

                for (var i = 0; i < pipe.Connectors.Count - 1; i++)
                {
                    var startVertex = cnnToVertex[pipe.Connectors[i]];
                    var endVertex = cnnToVertex[pipe.Connectors[i + 1]];

                    var edge = new Edge(startVertex, endVertex);

                    startVertex.AddAdjacentVertex(endVertex);
                    endVertex.AddAdjacentVertex(startVertex);

                    edges.Add(edge);
                }
            }

            ValidateVerticesAccessibility(pipes, cnnToVertex);
            
            foreach (var pipe in pipes)
            {
                if (pipe.IsFailed)
                {
                    var result = new List<IPipeSegment>();

                    result.Add(
                        new FailedSegment(
                            new Point(0, 0),
                            Common.GetLength(pipe.Rect, pipe.Orientation),
                            pipe.Orientation,
                            pipe.FailType)
                    );

                    pipe.SetPipeSegments(result);
                }
            }

            foreach (var pipe in pipes)
            {
                if (pipe.IsFailed)
                {
                    continue;
                }

                List<IPipeSegment> allSegments = new List<IPipeSegment>();

                for (var i = 0; i < pipe.Connectors.Count - 1; i++)
                {
                    var c1 = pipe.Connectors[i];
                    var c2 = pipe.Connectors[i + 1];

                    var s1 = c1.CreateSegment(pipe);
                    var s2 = c2.CreateSegment(pipe);

                    var v1 = cnnToVertex[c1];
                    var v2 = cnnToVertex[c2];

                    if (i == 0)
                    {
                        allSegments.Add(s1);
                        cnnToVertex[c1].PipeSegments.Add(s1);
                    }

                    var edge = edges.Single(e => e.Equals(v1, v2));

                    var lineSegment = Common.CreateSegmentBetweenSegments(pipe, s1, s2);
                    edge.PipeSegment = lineSegment;
                    allSegments.Add(lineSegment);

                    allSegments.Add(s2);
                    cnnToVertex[c2].PipeSegments.Add(s2);
                }
                
                pipe.SetPipeSegments(allSegments);
            }

            _isSchemeFailed = pipes.Any(pipe => pipe.IsFailed);
            _vertices = cnnToVertex.Values.ToArray();
            _edges = edges.Where(edge => edge.PipeSegment != null).ToArray();
        }

        private void AddEndVertex(GraphPipe pipe, IPipeConnector connector, List<IPipeConnector> connectors, Dictionary<IPipeConnector, IVertex> cnnToVertex)
        {
            bool isStartVertex = pipe.StartConnector == connector;
            if (!isStartVertex && pipe.EndConnector != connector)
            {
                throw new Exception("!! !!");
            }

            if (connectors.Contains(connector))
            {
                return;
            }

            connectors.Add(connector);
            IVertex vertex;
            switch (pipe.Direction)
            {
                case PipeDirection.None:
                    vertex = new Vertex(this, connector);
                    break;
                case PipeDirection.Forward:
                    vertex = isStartVertex ? (IVertex) new SourceVertex(this, connector) : new DestinationVertex(this, connector);
                    break;
                case PipeDirection.Backward:
                    vertex = isStartVertex ? new DestinationVertex(this, connector) : (IVertex)new SourceVertex(this, connector);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            cnnToVertex[connector] = vertex;
        }

        private void ValidateVerticesAccessibility(IReadOnlyList<GraphPipe> pipes, Dictionary<IPipeConnector, IVertex> cnnToVertexDictionary)
        {
            var sourceVertices = cnnToVertexDictionary.Values.OfType<SourceVertex>();
            var destinationVertices = cnnToVertexDictionary.Values.OfType<DestinationVertex>().ToArray();
            var visitedVertices = new HashSet<IVertex>();

            foreach (var connector in sourceVertices)
            {
                var algo = new DepthFirstDirectedPaths(connector, vertex => vertex.GetAllAdjacentVertices());

                foreach (var destinationVertex in destinationVertices)
                {
                    var paths = algo.PathsTo(destinationVertex);
                    if (paths == null || paths.Count == 0)
                    {
                        continue;
                    }

                    foreach (var path in paths)
                    {
                        foreach (var vertex in path)
                        {
                            visitedVertices.Add(vertex);
                        }
                    }
                }
            }

            foreach (var pipe in pipes)
            {
                foreach (var connector in pipe.Connectors)
                {
                    if (!visitedVertices.Contains(cnnToVertexDictionary[connector]))
                    {
                        pipe.FailType = FailType.DeadPath;
                    }
                }
            }
        }

        public void InvalidateFlow()
        {
            if (_isSchemeFailed)
            {
                return;
            }

            foreach (var edge in _edges)
            {
                edge.PipeSegment.HasFlow = false;
            }
            foreach (var vertex in _vertices)
            {
                foreach (var pipeSegment in vertex.PipeSegments)
                {
                    pipeSegment.HasFlow = false;
                }
            }

            var sourceVertices = _vertices.OfType<SourceVertex>();
            var destinationVertices = _vertices.OfType<DestinationVertex>().ToArray();
            foreach (var connector in sourceVertices)
            {
                var algo = new DepthFirstDirectedPaths(connector, vertex => vertex.GetAdjacentVertices());

                foreach (var destinationVertex in destinationVertices)
                {
                    var paths = algo.PathsTo(destinationVertex);
                    if (paths == null || paths.Count == 0)
                    {
                        continue;
                    }

                    foreach (var path in paths)
                    {
                        foreach (var vertex in path)
                        {
                            foreach (var pipeSegment in vertex.PipeSegments)
                            {
                                pipeSegment.HasFlow = true;
                            }
                        }

                        for (var i = 0; i < path.Count - 1; i++)
                        {
                            var edge = _edges.Single(e => e.Equals(path[i], path[i + 1]));
                            edge.PipeSegment.HasFlow = true;
                        }
                    }
                }
            }
        }

        public Edge FindEdge(IVertex startVertex, IVertex endVertex)
        {
            return _edges.Single(e => e.Equals(startVertex, endVertex));
        }
    }
}