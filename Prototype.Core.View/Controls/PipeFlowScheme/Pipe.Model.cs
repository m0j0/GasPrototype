using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    internal class FlowGraph
    {
        private ProcessPipe[] _processPipes;

        public FlowGraph(ISchemeContainer container, IReadOnlyCollection<IPipe> pipes, IReadOnlyCollection<IValve> valves)
        {
            _processPipes = SplitPipeToSegments(container, pipes, valves);
        }


        private static ProcessPipe[] SplitPipeToSegments(ISchemeContainer container,
            IReadOnlyCollection<IPipe> allPipes, IReadOnlyCollection<IValve> allValves)
        {
            var processPipes = allPipes.Select(p => new ProcessPipe(container, p)).ToArray();
            var processValves = allValves.Select(v => new ProcessValve(container, v)).ToArray();
            var connectors = new List<IConnector>();

            foreach (var pipe1 in processPipes)
            {
                if (!Common.IsSizeValid(pipe1))
                {
                    pipe1.FailType = FailType.WrongSize;
                    continue;
                }

                foreach (var pipe2 in processPipes)
                {
                    if (pipe1 == pipe2 ||
                        pipe2.IsFailed)
                    {
                        continue;
                    }

                    var intersectionRect = PipeDrawing.FindIntersection(
                        pipe1.Rect,
                        pipe2.Rect
                    );

                    if (!Common.IsIntersectionSizeValid(intersectionRect))
                    {
                        if (intersectionRect != Rect.Empty)
                        {
                            pipe1.FailType = FailType.IntersectionNotSupported;
                            pipe2.FailType = FailType.IntersectionNotSupported;
                        }

                        continue;
                    }

                    var existingConnector = connectors.FirstOrDefault(c => c.Rect == intersectionRect);

                    if (PipeDrawing.IsBridgeConnection(pipe1, pipe2, intersectionRect) ||
                        PipeDrawing.IsBridgeConnection(pipe2, pipe1, intersectionRect))
                    {
                        if (existingConnector != null)
                        {
                            continue;
                        }

                        int pipe1Index = Array.IndexOf(processPipes, pipe1);
                        int pipe2Index = Array.IndexOf(processPipes, pipe2);

                        var bridgeConnector = new BridgeConnector(intersectionRect,
                            pipe1Index > pipe2Index ? pipe1 : pipe2,
                            pipe1Index > pipe2Index ? pipe2 : pipe1);
                        connectors.Add(bridgeConnector);
                        continue;
                    }

                    if (!PipeDrawing.IsCornerConnection(pipe1, pipe2, intersectionRect) &&
                        !PipeDrawing.IsSerialConnection(pipe1, pipe2, intersectionRect))
                    {
                        pipe1.FailType = FailType.IntersectionNotSupported;
                        pipe2.FailType = FailType.IntersectionNotSupported;

                        continue;
                    }

                    var cornerConnector = (CornerConnector)existingConnector ??
                                          new CornerConnector(intersectionRect);
                    cornerConnector.AddPipe(pipe1);
                    cornerConnector.AddPipe(pipe2);
                    if (existingConnector == null)
                    {
                        connectors.Add(cornerConnector);
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

            foreach (var connector in connectors.OfType<CornerConnector>())
            {
                foreach (var valve in processValves)
                {
                    if (PipeDrawing.IsIntersect(connector.Rect, valve.Rect))
                    {
                        connector.Valve = valve.Valve;
                    }
                }
            }

            foreach (var currentProcessPipe in processPipes)
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

                var orderedConnectors = currentProcessPipe.Connectors.OrderBy(c => c.Rect.Top).ThenBy(c => c.Rect.Left).ToList();
                var firstOrDefault = orderedConnectors.FirstOrDefault();
                var lastOrDefault = orderedConnectors.LastOrDefault();

                if (firstOrDefault == null ||
                    firstOrDefault.Rect.TopLeft != currentProcessPipe.Rect.TopLeft)
                {
                    var cornerConnector = new CornerConnector(new Rect(currentProcessPipe.Rect.TopLeft, Common.ConnectorVector));
                    cornerConnector.AddPipe(currentProcessPipe);
                    orderedConnectors.Insert(0, cornerConnector);

                    if (PipeFlowScheme.GetIsSource((DependencyObject)currentProcessPipe.Pipe))
                    {
                        cornerConnector.IsSource = true;
                    }
                    if (PipeFlowScheme.GetIsDestination((DependencyObject)currentProcessPipe.Pipe))
                    {
                        cornerConnector.IsDestination = true;
                    }
                }

                if (lastOrDefault == null ||
                    lastOrDefault.Rect.BottomRight != currentProcessPipe.Rect.BottomRight)
                {
                    var cornerConnector = new CornerConnector(new Rect(currentProcessPipe.Rect.BottomRight - Common.ConnectorVector, Common.ConnectorVector));
                    cornerConnector.AddPipe(currentProcessPipe);
                    orderedConnectors.Add(cornerConnector);

                    if (PipeFlowScheme.GetIsSource((DependencyObject)currentProcessPipe.Pipe))
                    {
                        cornerConnector.IsSource = true;
                    }
                    if (PipeFlowScheme.GetIsDestination((DependencyObject)currentProcessPipe.Pipe))
                    {
                        cornerConnector.IsDestination = true;
                    }
                }

                foreach (var connector in orderedConnectors)
                {
                    currentProcessPipe.Segments.Add(connector.CreateSegment(currentProcessPipe));
                }

                List<IPipeSegment> allSegments = new List<IPipeSegment>();
                for (int i = 0; i < currentProcessPipe.Segments.Count - 1; i++)
                {
                    var s1 = currentProcessPipe.Segments[i];
                    var s2 = currentProcessPipe.Segments[i + 1];

                    allSegments.Add(s1);

                    if (currentProcessPipe.Orientation == Orientation.Horizontal)
                    {
                        allSegments.Add(new LineSegment(new Point(s1.StartPoint.X + s1.Length, 0),
                            s2.StartPoint.X - (s1.StartPoint.X + s1.Length),
                            currentProcessPipe.Orientation));
                    }
                    else
                    {
                        allSegments.Add(new LineSegment(new Point(0, s1.StartPoint.Y + s1.Length),
                            s2.StartPoint.Y - (s1.StartPoint.Y + s1.Length),
                            currentProcessPipe.Orientation));
                    }

                    allSegments.Add(s2);
                }

                currentProcessPipe.Segments.Clear();
                currentProcessPipe.Segments.AddRange(allSegments);
                currentProcessPipe.Pipe.Segments = allSegments;
            }

            return processPipes;
        }

        public void InvalidateFlow()
        {
            var connectors = _processPipes.SelectMany(pipe => pipe.Connectors).OfType<CornerConnector>().Distinct().ToArray();

            foreach (var edge in _processPipes)
            {
                foreach (var segment in edge.Pipe.Segments)
                {
                    segment.FlowDirection = FlowDirection.None;
                }
            }

            var destinationConnectors = connectors.Where(c => c.IsDestination).ToArray();
            foreach (var connector in connectors)
            {
                if (!connector.IsSource)
                {
                    continue;
                }

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
                            var edge = _processPipes.Single(pipe => pipe.Connectors.Contains(path[i]) && pipe.Connectors.Contains(path[i + 1]));
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

    internal class ProcessPipe
    {
        public ProcessPipe(ISchemeContainer container, IPipe pipe)
        {
            Rect = Common.GetAbsoluteRect(container, pipe);
            Pipe = pipe;
            Segments = new List<IPipeSegment>();
            Connectors = new List<IConnector>();
        }

        public IPipe Pipe { get; }

        public Rect Rect { get; }

        public Orientation Orientation => Pipe.Orientation;

        public FailType FailType { get; set; }

        public bool IsFailed => FailType != FailType.None;

        public List<IPipeSegment> Segments { get; }

        public List<IConnector> Connectors { get; }
    }

    internal class ProcessValve
    {
        public ProcessValve(ISchemeContainer container, IValve valve)
        {
            Rect = Common.GetAbsoluteRect(container, valve);
            Valve = valve;
        }

        public IValve Valve { get; }

        public Rect Rect { get; }
    }
}