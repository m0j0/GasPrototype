﻿using System;
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

        public static ProcessPipe[] SplitPipeToSegments(IContainer container, 
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
                        if (intersectionRect != Rect.Empty)
                        {
                            pipe1.IsFailed = true;
                            pipe2.IsFailed = true;
                        }

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

                    if (!IsCornerConnection(pipe1, pipe2, intersectionRect) &&
                        !IsSerialConnection(pipe1, pipe2, intersectionRect))
                    {
                        pipe1.IsFailed = true;
                        pipe2.IsFailed = true;

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
                if (currentProcessPipe.IsFailed)
                {
                    var result = new List<IPipeSegment>();

                    result.Add(new LinePipeSegment(new Point(0, 0), 
                        currentProcessPipe.Orientation == Orientation.Horizontal
                            ? currentProcessPipe.Rect.Width
                            : currentProcessPipe.Rect.Height,
                        currentProcessPipe.Orientation,
                        true));

                    currentProcessPipe.Pipe.Segments = result;

                    continue;
                }

                var orderedConnectors = currentProcessPipe.Connectors.OrderBy(c => c.Rect.Top).ThenBy(c => c.Rect.Left).ToList();
                var firstOrDefault = orderedConnectors.FirstOrDefault();
                var lastOrDefault = orderedConnectors.LastOrDefault();

                if (firstOrDefault == null ||
                    firstOrDefault.Rect.TopLeft != currentProcessPipe.Rect.TopLeft)
                {
                    var cornerConnector = new CornerConnector(new Rect(currentProcessPipe.Rect.TopLeft, Pipe.ConnectorVector));
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
                    var cornerConnector = new CornerConnector(new Rect(currentProcessPipe.Rect.BottomRight - Pipe.ConnectorVector, Pipe.ConnectorVector));
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
                        allSegments.Add(new LinePipeSegment(new Point(s1.StartPoint.X + s1.Length, 0),
                            s2.StartPoint.X - (s1.StartPoint.X + s1.Length),
                            currentProcessPipe.Orientation,
                            false));
                    }
                    else
                    {
                        allSegments.Add(new LinePipeSegment(new Point(0, s1.StartPoint.Y + s1.Length),
                            s2.StartPoint.Y - (s1.StartPoint.Y + s1.Length),
                            currentProcessPipe.Orientation,
                            false));
                    }

                    allSegments.Add(s2);
                }

                currentProcessPipe.Pipe.Segments = allSegments;
            }

            return processPipes;
        }

        private static bool IsBridgeConnection(ProcessPipe pipe1, ProcessPipe pipe2, Rect intersectionRect)
        {
            return pipe1.Rect.Left < intersectionRect.Left &&
                   pipe1.Rect.Right > intersectionRect.Right &&
                   pipe2.Rect.Top < intersectionRect.Top &&
                   pipe2.Rect.Bottom > intersectionRect.Bottom;
        }

        private static bool IsCornerConnection(ProcessPipe pipe1, ProcessPipe pipe2, Rect intersectionRect)
        {
            bool IsUpperLeft(ProcessPipe p1, ProcessPipe p2)
            {
                return p1.Rect.TopLeft == p2.Rect.TopLeft &&
                       p1.Rect.TopLeft == intersectionRect.TopLeft;
            }
            bool IsUpperRight(ProcessPipe p1, ProcessPipe p2)
            {
                return p1.Rect.TopRight == p2.Rect.TopRight &&
                       p1.Rect.TopRight == intersectionRect.TopRight;
            }
            bool IsLowerLeft(ProcessPipe p1, ProcessPipe p2)
            {
                return p1.Rect.BottomLeft == p2.Rect.BottomLeft &&
                       p1.Rect.BottomLeft == intersectionRect.BottomLeft;
            }
            bool IsLowerRight(ProcessPipe p1, ProcessPipe p2)
            {
                return p1.Rect.BottomRight == p2.Rect.BottomRight &&
                       p1.Rect.BottomRight == intersectionRect.BottomRight;
            }

            return IsUpperLeft(pipe1, pipe2) || IsUpperLeft(pipe2, pipe1) ||
                   IsUpperRight(pipe1, pipe2) || IsUpperRight(pipe2, pipe1) ||
                   IsLowerLeft(pipe1, pipe2) || IsLowerLeft(pipe2, pipe1) ||
                   IsLowerRight(pipe1, pipe2) || IsLowerRight(pipe2, pipe1);
        }

        private static bool IsSerialConnection(ProcessPipe pipe1, ProcessPipe pipe2, Rect intersectionRect)
        {
            bool IsVertical(ProcessPipe p1, ProcessPipe p2)
            {
                return p1.Rect.BottomRight == p2.Rect.TopLeft + Pipe.ConnectorVector &&
                       p1.Rect.BottomLeft == intersectionRect.BottomLeft;
            }
            bool IsHorizontal(ProcessPipe p1, ProcessPipe p2)
            {
                return p1.Rect.BottomRight == p2.Rect.TopLeft + Pipe.ConnectorVector &&
                       p1.Rect.BottomRight == intersectionRect.BottomRight;
            }

            return IsVertical(pipe1, pipe2) || IsVertical(pipe2, pipe1) ||
                   IsHorizontal(pipe1, pipe2) || IsHorizontal(pipe2, pipe1);
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