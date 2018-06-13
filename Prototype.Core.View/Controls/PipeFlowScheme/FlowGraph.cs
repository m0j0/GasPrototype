using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    internal class FlowGraph
    {
        private IReadOnlyCollection<IVertex> _vertices;
        private IReadOnlyCollection<Edge> _edges;

        public FlowGraph(ISchemeContainer container, IEnumerable<IPipe> pipes,
            IEnumerable<IValve> valves)
        {
            SplitPipesToSegments(container, pipes, valves);
            
            InvalidateFlow();
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

                        int pipe1Index = pipes.IndexOf(pipe1);
                        int pipe2Index = pipes.IndexOf(pipe2);

                        var bridgeConnector = new BridgePipeConnector(intersectionRect,
                            pipe1Index > pipe2Index ? pipe1 : pipe2,
                            pipe1Index > pipe2Index ? pipe2 : pipe1);
                        connectors.Add(bridgeConnector);
                        cnnToVertex[bridgeConnector] = new Vertex(bridgeConnector);
                        continue;
                    }
                    
                    var connector = (PipeConnector) existingConnector ??
                                    new PipeConnector(intersectionRect);
                    connector.AddPipe(pipe1);
                    connector.AddPipe(pipe2);
                    if (existingConnector == null)
                    {
                        connectors.Add(connector);
                        cnnToVertex[connector] = new Vertex(connector);
                    }
                }

            }
            
            foreach (var pipe in pipes)
            {
                bool hasStartConnector = pipe.StartConnector != null;
                bool hasEndConnector = pipe.EndConnector != null;

                bool isSource = pipe.Type == PipeType.Source;
                bool isDestination = pipe.Type == PipeType.Destination;
                
                if (isSource && isDestination)
                {
                    pipe.FailType = FailType.BothSourceDestination;
                    continue;
                }

                if ((isSource || isDestination) && 
                    (!hasStartConnector && !hasEndConnector || hasStartConnector && hasEndConnector))
                {
                    pipe.FailType = FailType.BothSourceDestination;
                    continue;
                }

                if (!hasStartConnector)
                {
                    var connector = new PipeConnector(new Rect(pipe.Rect.TopLeft, Common.ConnectorVector));
                    connector.AddPipe(pipe);
                    
                    connectors.Add(connector);
                    cnnToVertex[connector] = isSource ? (IVertex) new SourceVertex(connector) : new Vertex(connector);
                }

                if (!hasEndConnector)
                {
                    var connector = new PipeConnector(new Rect(pipe.Rect.BottomRight - Common.ConnectorVector,
                        Common.ConnectorVector));
                    connector.AddPipe(pipe);
                    
                    connectors.Add(connector);
                    cnnToVertex[connector] = isDestination ? (IVertex)new DestinationVertex(connector) : new Vertex(connector);
                }
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

                        var vertex = (Vertex)cnnToVertex[pipeConnector];
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
                    cnnToVertex[connector] = new Vertex(connector) {Valve = valve.Valve};
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
                    if (edge.IsBidirectional)
                    {
                        endVertex.AddAdjacentVertex(startVertex);
                    }

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

            _vertices = cnnToVertex.Values.ToArray();
            _edges = edges.Where(edge => edge.PipeSegment != null).ToArray();
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

            var sourceConnectors = _vertices.OfType<SourceVertex>();
            var destinationConnectors = _vertices.OfType<DestinationVertex>().ToArray();
            foreach (var connector in sourceConnectors)
            {
                var algo = new DepthFirstDirectedPaths(connector, vertex => vertex.GetAdjacentVertices());

                foreach (var destinationVertex in destinationConnectors)
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
    }
}