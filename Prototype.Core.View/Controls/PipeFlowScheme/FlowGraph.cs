using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    internal class FlowGraph
    {
        private readonly ISchemeContainer _container;
        private readonly GraphPipe[] _pipes;
        private readonly GraphValve[] _valves;

        private readonly IReadOnlyCollection<IVertex> _vertices;
        private readonly IReadOnlyCollection<IPipeSegment> _segments;

        public FlowGraph(ISchemeContainer container, IReadOnlyCollection<IPipe> pipes,
            IReadOnlyCollection<IValve> valves)
        {
            _container = container;
            _pipes = pipes.Select(p => new GraphPipe(container, p)).ToArray();
            _valves = valves.Select(v => new GraphValve(container, v)).ToArray();
            var result = SplitPipesToSegments();
            _vertices = result.Item1;
            _segments = result.Item2;

            InvalidateFlow();
        }

        private Tuple<IReadOnlyCollection<IVertex>, IReadOnlyCollection<IPipeSegment>> SplitPipesToSegments()
        {
            var connectors = new List<IPipeConnector>();

            foreach (var pipe1 in _pipes)
            {
                if (!Common.IsSizeValid(pipe1))
                {
                    pipe1.FailType = FailType.WrongSize;
                    continue;
                }

                foreach (var pipe2 in _pipes)
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
                        if (intersectionRect != Rect.Empty)
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

                        int pipe1Index = Array.IndexOf(_pipes, pipe1);
                        int pipe2Index = Array.IndexOf(_pipes, pipe2);

                        var bridgeConnector = new BridgePipeConnector(intersectionRect,
                            pipe1Index > pipe2Index ? pipe1 : pipe2,
                            pipe1Index > pipe2Index ? pipe2 : pipe1);
                        connectors.Add(bridgeConnector);
                        continue;
                    }

                    if (!Common.IsCornerConnection(pipe1, pipe2, intersectionRect) &&
                        !Common.IsSerialConnection(pipe1, pipe2, intersectionRect))
                    {
                        pipe1.FailType = FailType.IntersectionIsNotSupported;
                        pipe2.FailType = FailType.IntersectionIsNotSupported;

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

                bool hasStartConnector = pipe1.StartConnector != null;
                bool hasEndConnector = pipe1.EndConnector != null;

                if (pipe1.StartConnector == null)
                {
                    var connector = new PipeConnector(new Rect(pipe1.Rect.TopLeft, Common.ConnectorVector));
                    connector.AddPipe(pipe1);

                    pipe1.StartConnector = connector;
                    connectors.Add(connector);

                    if (_container.IsSource(pipe1.Pipe))
                    {
                        connector.IsSource = true;
                    }

                    if (_container.IsDestination(pipe1.Pipe))
                    {
                        connector.IsDestination = true;
                    }

                    if (connector.IsSource && connector.IsDestination)
                    {
                        throw new Exception("!!!");
                    }

                    if ((connector.IsSource || connector.IsDestination) && !hasEndConnector)
                    {
                        throw new Exception("!!!");
                    }
                }

                if (pipe1.EndConnector == null)
                {
                    var connector = new PipeConnector(new Rect(pipe1.Rect.BottomRight - Common.ConnectorVector,
                        Common.ConnectorVector));
                    connector.AddPipe(pipe1);

                    pipe1.EndConnector = connector;
                    connectors.Add(connector);

                    if (_container.IsSource(pipe1.Pipe))
                    {
                        connector.IsSource = true;
                    }

                    if (_container.IsDestination(pipe1.Pipe))
                    {
                        connector.IsDestination = true;
                    }

                    if (connector.IsSource && connector.IsDestination)
                    {
                        throw new Exception("!!!");
                    }

                    if ((connector.IsSource || connector.IsDestination) && !hasStartConnector)
                    {
                        throw new Exception("!!!");
                    }
                }
            }

            // just check
            for (int i = 0; i < connectors.Count; i++)
            {
                for (int j = 0; j < connectors.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    if (connectors[i].Rect == connectors[j].Rect)
                    {
                        throw new Exception("!!!");
                    }
                }
            }

            foreach (var connector in connectors.OfType<PipeConnector>())
            {
                foreach (var valve in _valves)
                {
                    if (Common.IsIntersect(connector.Rect, valve.Rect))
                    {
                        connector.Valve = valve.Valve;
                    }
                }
            }

            var vertices = new List<IVertex>();
            var segments = new List<IPipeSegment>();

            //foreach (var connector in connectors.OfType<PipeConnector>())
            //{
            //    if (connector.IsSource)
            //    {
            //        vertices.Add(new SourceVertex());
            //    }
            //}

            foreach (var currentProcessPipe in _pipes)
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

                    allSegments.Add(s1);
                    allSegments.Add(Common.CreateSegmentBetweenSegments(currentProcessPipe, s1, s2));
                    allSegments.Add(s2);
                }

                currentProcessPipe.Pipe.Segments = allSegments;
            }

            return Tuple.Create<IReadOnlyCollection<IVertex>, IReadOnlyCollection<IPipeSegment>>(vertices, segments);
        }

        public void InvalidateFlow()
        {
            foreach (var edge in _pipes)
            {
                foreach (var segment in edge.Pipe.Segments)
                {
                    segment.FlowDirection = FlowDirection.None;
                }
            }

            var _connectors = _pipes.SelectMany(pipe => pipe.Connectors).Distinct().OfType<PipeConnector>().ToArray(); // TODO
            var destinationConnectors = _connectors.Where(c => c.IsDestination).ToArray();
            var sourceConnectors = _connectors.Where(c => c.IsSource);
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
                            var edge = _pipes.Single(pipe =>
                                pipe.Connectors.Contains(path[i]) && pipe.Connectors.Contains(path[i + 1]));
                            foreach (var segment in edge.Pipe.Segments)
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