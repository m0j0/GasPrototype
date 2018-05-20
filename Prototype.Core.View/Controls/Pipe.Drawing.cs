using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Prototype.Core.Controls
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
        public static Rect Intersect(Rect a, Rect b)
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

        private static bool IsIntersect(Rect a, Rect b)
        {
            return a.Left <= b.Right &&
                   b.Left <= a.Right &&
                   a.Top <= b.Bottom &&
                   b.Top <= a.Bottom;
        }

        public static IReadOnlyCollection<IPipeSegment> SplitPipeToSegments(Pipe pipe,
            IReadOnlyCollection<Pipe> allPipes)
        {
            var processPipes = allPipes.Select(p => new ProcessPipe(p)).ToArray();
            var currentProcessPipe = processPipes.First(processPipe => processPipe.Pipe == pipe);
            var connectors = new HashSet<IConnector>();

            foreach (var pipe1 in processPipes)
            {
                foreach (var pipe2 in processPipes)
                {
                    if (pipe1 == pipe2)
                    {
                        continue;
                    }

                    var intersectionRect = Intersect(
                        pipe1.Rect,
                        pipe2.Rect
                    );

                    if (intersectionRect.Width != Pipe.PipeWidth ||
                        intersectionRect.Height != Pipe.PipeWidth)
                    {
                        continue;
                    }

                    if (pipe1.Rect.Left < intersectionRect.Left &&
                        pipe1.Rect.Right > intersectionRect.Right &&
                        pipe2.Rect.Top < intersectionRect.Top &&
                        pipe2.Rect.Bottom > intersectionRect.Bottom)
                    {
                        var bridgeConnector = connectors.OfType<BridgeConnector>()
                                                  .FirstOrDefault(cnn => cnn.Rect == intersectionRect) ??
                                              new BridgeConnector(intersectionRect);
                        connectors.Add(bridgeConnector);
                        bridgeConnector.AddPipes(pipe1, pipe2);
                        continue;
                    }

                    var cornerConnector = connectors.OfType<CornerConnector>()
                                              .FirstOrDefault(cnn => cnn.Rect == intersectionRect) ??
                                          new CornerConnector(intersectionRect);
                    cornerConnector.AddPipe(pipe1);
                    cornerConnector.AddPipe(pipe2);
                    connectors.Add(cornerConnector);
                }
            }

            foreach (var connector in currentProcessPipe.Connectors)
            {
                currentProcessPipe.Segments.Add(connector.CreateSegment(currentProcessPipe));
            }

            var orderedSegments = currentProcessPipe.Segments.OrderBy(s => s.StartPoint.X).ThenBy(s => s.StartPoint.Y)
                .ToList();

            var first = orderedSegments.FirstOrDefault();
            if (first == null ||
                first.StartPoint.X != 0 ||
                first.StartPoint.Y != 0)
            {
                orderedSegments.Insert(0,
                    new LinePipeSegment(new Point(0, 0),
                        0,
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
                    orderedSegments.Add(new LinePipeSegment(new Point(currentProcessPipe.Rect.Width, 0),
                        0,
                        currentProcessPipe.Orientation));
                }
                else
                {
                    orderedSegments.Add(new LinePipeSegment(new Point(0, currentProcessPipe.Rect.Height),
                        0,
                        currentProcessPipe.Orientation));
                }
            }


            List<IPipeSegment> allSegments = new List<IPipeSegment>();
            for (int i = 0; i < orderedSegments.Count - 1; i++)
            {
                var s1 = orderedSegments[i];
                var s2 = orderedSegments[i + 1];

                allSegments.Add(s1);

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

                allSegments.Add(s2);
            }

            return allSegments;
        }

        public static bool HasFlagEx(this Side side, Side flag)
        {
            return (side & flag) != 0;
        }
    }
}