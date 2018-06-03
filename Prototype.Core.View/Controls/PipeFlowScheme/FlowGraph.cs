using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    internal class FlowGraph
    {
        private IReadOnlyCollection<IVertex> _vertices;
        private IReadOnlyCollection<IPipeSegment> _segments;

        public FlowGraph(ISchemeContainer container, IReadOnlyCollection<IPipe> pipes,
            IReadOnlyCollection<IValve> valves)
        {
            SplitPipesToSegments(container, pipes, valves);
            
            InvalidateFlow();
        }

        private void SplitPipesToSegments(ISchemeContainer container, IReadOnlyCollection<IPipe> pipeControls,
            IReadOnlyCollection<IValve> valveControls)
        {
            var pipes = pipeControls.Select(p => new GraphPipe(container, p)).ToArray();
            var valves = valveControls.Select(v => new GraphValve(container, v)).ToArray();

            var connectors = new List<IPipeConnector>();

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

                        int pipe1Index = Array.IndexOf(pipes, pipe1);
                        int pipe2Index = Array.IndexOf(pipes, pipe2);

                        var bridgeConnector = new BridgePipeConnector(intersectionRect,
                            pipe1Index > pipe2Index ? pipe1 : pipe2,
                            pipe1Index > pipe2Index ? pipe2 : pipe1);
                        connectors.Add(bridgeConnector);
                        continue;
                    }
                    
                    var connector = (PipeConnector) existingConnector ??
                                    new PipeConnector(intersectionRect);
                    connector.AddPipe(pipe1);
                    connector.AddPipe(pipe2);
                    if (existingConnector == null)
                    {
                        connectors.Add(connector);
                    }

                    if (pipe1.Rect.TopLeft == connector.Rect.TopLeft)
                    {
                        pipe1.StartConnector = connector;
                    }

                    if (pipe1.Rect.BottomRight == connector.Rect.BottomRight)
                    {
                        pipe1.EndConnector = connector;
                    }
                }

            }

            //// move to tests
            //for (int i = 0; i < connectors.Count; i++)
            //{
            //    for (int j = 0; j < connectors.Count; j++)
            //    {
            //        if (i == j)
            //        {
            //            continue;
            //        }

            //        if (connectors[i].Rect == connectors[j].Rect)
            //        {
            //            throw new Exception("!!!");
            //        }
            //    }
            //}


            var vertices = new List<IVertex>();
            var segments = new List<IPipeSegment>();

            foreach (var pipe in pipes)
            {
                bool hasStartConnector = pipe.StartConnector != null;
                bool hasEndConnector = pipe.EndConnector != null;

                bool isSource = container.IsSource(pipe.Pipe);
                bool isDestination = container.IsDestination(pipe.Pipe);
                
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

                    pipe.StartConnector = connector;
                    connectors.Add(connector);

                    IVertex vertex;
                    if (isSource)
                    {
                        vertex = new SourceVertex(connector);
                    }
                    else if (isDestination)
                    {
                        vertex = new DestinationVertex(connector);
                    }
                    else
                    {
                        vertex = new Vertex(connector);
                    }

                    vertices.Add(vertex);
                }

                if (!hasEndConnector)
                {
                    var connector = new PipeConnector(new Rect(pipe.Rect.BottomRight - Common.ConnectorVector,
                        Common.ConnectorVector));
                    connector.AddPipe(pipe);

                    pipe.EndConnector = connector;
                    connectors.Add(connector);
                    
                    IVertex vertex;
                    if (isSource)
                    {
                        vertex = new SourceVertex(connector);
                    }
                    else if (isDestination)
                    {
                        vertex = new DestinationVertex(connector);
                    }
                    else
                    {
                        vertex = new Vertex(connector);
                    }
                    vertices.Add(vertex);
                }
                
                foreach (var connector in pipe.Connectors.OfType<PipeConnector>())
                {
                    bool hasIntersectionWithValve = false;
                    foreach (var valve in valves)
                    {
                        if (!Common.IsIntersect(connector.Rect, valve.Rect))
                        {
                            continue;
                        }

                        if (hasIntersectionWithValve)
                        {
                            throw new Exception("!!!");
                        }

                        hasIntersectionWithValve = true;
                        if (connector.Vertex == null)
                        {
                            connector.Vertex = new Vertex(connector);

                            vertices.Add(connector.Vertex);
                        }

                        switch (connector.Vertex)
                        {
                            case Vertex vertex:
                                vertex.Valve = valve.Valve;
                                break;

                            case SourceVertex _:
                            case DestinationVertex _:
                                throw new Exception("!!!");

                            default:
                                throw new Exception("!!! !!!");
                        }
                    }

                    if (connector.Vertex == null)
                    {
                        connector.Vertex = new Vertex(connector);

                        vertices.Add(connector.Vertex);
                    }
                }
            }

            foreach (var currentProcessPipe in pipes)
            {
                if (currentProcessPipe.IsFailed)
                {
                    var result = new List<IPipeSegment>();

                    result.Add(
                        new FailedSegment(
                            new Point(0, 0),
                            Common.GetLength(currentProcessPipe.Rect, currentProcessPipe.Orientation),
                            currentProcessPipe.Orientation,
                            currentProcessPipe.FailType)
                    );

                    currentProcessPipe.Pipe.Segments = result;

                    continue;
                }

                var orderedConnectors = currentProcessPipe.Connectors.OrderBy(c => c.Rect.Top).ThenBy(c => c.Rect.Left)
                    .ToList();

                var connectorSegments = new List<IPipeSegment>();
                foreach (var connector in orderedConnectors)
                {
                    connectorSegments.Add(connector.CreateSegment(currentProcessPipe));
                }

                List<IPipeSegment> allSegments = new List<IPipeSegment>();
                for (int i = 0; i < connectorSegments.Count - 1; i++)
                {
                    var s1 = connectorSegments[i];
                    var s2 = connectorSegments[i + 1];

                    var lineSegment = Common.CreateSegmentBetweenSegments(currentProcessPipe, s1, s2);
                    allSegments.Add(s1);
                    allSegments.Add(lineSegment);
                    allSegments.Add(s2);
                }

                segments.AddRange(allSegments);
                currentProcessPipe.Pipe.Segments = allSegments;
            }

            _vertices = vertices;
            _segments = segments;
        }

        public void InvalidateFlow()
        {
            foreach (var segment in _segments)
            {
                segment.FlowDirection = FlowDirection.None;
            }

            var sourceConnectors = _vertices.OfType<SourceVertex>();
            var destinationConnectors = _vertices.OfType<DestinationVertex>().ToArray();
            foreach (var connector in sourceConnectors)
            {
                var algo = new DepthFirstDirectedPaths(connector);

                foreach (var destinationVertex in destinationConnectors)
                {
                    var paths = algo.PathsTo(destinationVertex);
                    if (paths == null || paths.Count == 0)
                    {
                        continue;
                    }

                    foreach (var path in paths)
                    {
                        for (var i = 0; i < path.Count - 1; i++)
                        {
                            var pipe = path[i].Connector.GetPipes().Intersect(path[i + 1].Connector.GetPipes()).Single();
                            
                            foreach (var segment in pipe.Pipe.Segments)
                            {
                                segment.FlowDirection = FlowDirection.Both;
                            }
                        }
                    }
                }
            }
        }
    }
}