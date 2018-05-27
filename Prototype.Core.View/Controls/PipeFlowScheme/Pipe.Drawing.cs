using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    [Flags]
    internal enum Side
    {
        None = 0,
        Left = 1 << 0,
        Top = 1 << 1,
        Right = 1 << 2,
        Bottom = 1 << 3,
        All = Left | Right | Top | Bottom
    }

    internal static class PipeDrawing
    {

        private static bool IsIntersect(Rect a, Rect b)
        {
            return a.Left <= b.Right &&
                   b.Left <= a.Right &&
                   a.Top <= b.Bottom &&
                   b.Top <= a.Bottom;
        }

        public static void SplitPipeToSegments(IContainer container, 
            IReadOnlyCollection<IPipe> allPipes)
        {
            var processPipes = allPipes.Select(p => new ProcessPipe(container, p)).ToArray();
            var connectors = new List<IConnector>();

            foreach (var pipe1 in processPipes)
            {
                foreach (var pipe2 in processPipes)
                {
                    if (pipe1 == pipe2)
                    {
                        continue;
                    }

                    var intersectionRect = FindIntersection(
                        pipe1.Rect,
                        pipe2.Rect
                    );

                    if (intersectionRect.Width != Pipe.PipeWidth ||
                        intersectionRect.Height != Pipe.PipeWidth)
                    {
                        continue;
                    }

                    var existingConnector = connectors.FirstOrDefault(c => c.Rect == intersectionRect);
                    
                    if (IsBridgeConnection(pipe1, pipe2, intersectionRect) ||
                        IsBridgeConnection(pipe2, pipe1, intersectionRect))
                    {
                        if (existingConnector != null)
                        {
                            continue;
                        }

                        var bridgeConnector = new BridgeConnector(intersectionRect, pipe1, pipe2);
                        connectors.Add(bridgeConnector);
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

            foreach (var currentProcessPipe in processPipes)
            {
                foreach (var connector in currentProcessPipe.Connectors)
                {
                    currentProcessPipe.Segments.Add(connector.CreateSegment(currentProcessPipe));
                }

                var orderedSegments = currentProcessPipe.Segments.OrderBy(s => s.StartPoint.X)
                    .ThenBy(s => s.StartPoint.Y)
                    .ToList();

                var first = orderedSegments.FirstOrDefault();
                if (first == null ||
                    first.StartPoint.X != 0 ||
                    first.StartPoint.Y != 0)
                {
                    orderedSegments.Insert(0,
                        new EmptySegment(new Point(0, 0),
                            currentProcessPipe.Orientation)
                    );
                }

                var last = orderedSegments.LastOrDefault();
                if (last == null ||
                    last.StartPoint.X + Pipe.PipeWidth != currentProcessPipe.Rect.Right ||
                    last.StartPoint.Y + Pipe.PipeWidth != currentProcessPipe.Rect.Bottom)
                {
                    if (currentProcessPipe.Orientation == Orientation.Horizontal)
                    {
                        orderedSegments.Add(new EmptySegment(new Point(currentProcessPipe.Rect.Width, 0),
                            currentProcessPipe.Orientation));
                    }
                    else
                    {
                        orderedSegments.Add(new EmptySegment(new Point(0, currentProcessPipe.Rect.Height),
                            currentProcessPipe.Orientation));
                    }
                }


                List<IPipeSegment> allSegments = new List<IPipeSegment>();
                for (int i = 0; i < orderedSegments.Count - 1; i++)
                {
                    var s1 = orderedSegments[i];
                    var s2 = orderedSegments[i + 1];

                    if (!(s1 is EmptySegment))
                    {
                        allSegments.Add(s1);
                    }

                    if (currentProcessPipe.Orientation == Orientation.Horizontal)
                    {
                        allSegments.Add(new LinePipeSegment(new Point(s1.StartPoint.X + s1.Length, 0),
                            s2.StartPoint.X - (s1.StartPoint.X + s1.Length),
                            currentProcessPipe.Orientation));
                    }
                    else
                    {
                        allSegments.Add(new LinePipeSegment(new Point(0, s1.StartPoint.Y + s1.Length),
                            s2.StartPoint.Y - (s1.StartPoint.Y + s1.Length),
                            currentProcessPipe.Orientation));
                    }

                    if (!(s2 is EmptySegment))
                    {
                        allSegments.Add(s2);
                    }
                }

                currentProcessPipe.Pipe.Segments = allSegments;
            }
        }

        private static bool IsBridgeConnection(ProcessPipe pipe1, ProcessPipe pipe2, Rect intersectionRect)
        {
            return pipe1.Rect.Left < intersectionRect.Left &&
                   pipe1.Rect.Right > intersectionRect.Right &&
                   pipe2.Rect.Top < intersectionRect.Top &&
                   pipe2.Rect.Bottom > intersectionRect.Bottom;
        }

        public static Rect FindIntersection(Rect a, Rect b)
        {
            var x = Math.Max(a.X, b.X);
            var num1 = Math.Min(a.X + a.Width, b.X + b.Width);
            var y = Math.Max(a.Y, b.Y);
            var num2 = Math.Min(a.Y + a.Height, b.Y + b.Height);
            if (num1 >= x && num2 >= y)
            {
                return new Rect(x, y, num1 - x, num2 - y);
            }

            return Rect.Empty;
        }

        public static bool HasFlagEx(this Side side, Side flag)
        {
            return (side & flag) != 0;
        }
    }
}