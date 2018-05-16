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
        LeftRight = Left | Right,
        TopBottom = Top | Bottom,
        All = Left | Right | Top | Bottom
    }

    internal static class PipeEx
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

        public static IReadOnlyCollection<IPipeSegment> SplitPipeToSegments(Pipe pipe, IReadOnlyCollection<Pipe> allPipes)
        {

            //double IPipe.Top => Canvas.GetTop(this);

            //double IPipe.Left => Canvas.GetLeft(this);
            //result.Add(new LinePipeSegment(new Point(0, 0), pipe.Orientation == Orientation.Horizontal ? pipe.Width : pipe.Height, pipe.Orientation));

            var allPipeRects = allPipes.ToDictionary(p => new Rect(Canvas.GetLeft(p), Canvas.GetTop(p), p.Width, p.Height));
            var allPipeRects2 = allPipeRects.ToDictionary(pair => pair.Value, pair => pair.Key);

            var intersections = new Dictionary<Pipe, HashSet<Rect>>();

            foreach (var pipe1 in allPipeRects.Keys)
            {
                foreach (var pipe2 in allPipeRects.Keys)
                {
                    if (pipe1 == pipe2)
                    {
                        continue;
                    }

                    var intersectionRect = Intersect(
                        pipe1,
                        pipe2
                    );

                    if (intersectionRect.Width != Pipe.PipeWidth ||
                        intersectionRect.Height != Pipe.PipeWidth)
                    {
                        continue;
                    }

                    AddPipeIntersection(pipe1, intersectionRect);
                    AddPipeIntersection(pipe2, intersectionRect);

                    //if (!connections.ContainsKey(intersectionRect))
                }
            }

            var pipeRect = allPipeRects2[pipe];

            var result = new List<IPipeSegment>();

            if (intersections.TryGetValue(pipe, out var pipeIntersections))
            {
                var points = new HashSet<Point>();
                points.Add(pipeRect.TopLeft);
                points.Add(pipe.Orientation == Orientation.Horizontal ? pipeRect.TopRight : pipeRect.BottomLeft);
                foreach (var pipeIntersection in pipeIntersections)
                {
                    points.Add(pipeIntersection.TopLeft);
                    points.Add(pipe.Orientation == Orientation.Horizontal ? pipeIntersection.TopRight : pipeIntersection.BottomLeft);
                }

                var orderedPoint = points.OrderBy(rect => rect.X).ThenBy(rect => rect.Y).ToList();
                for (int i = 0; i < orderedPoint.Count - 1; i++)
                {
                    var point1 = orderedPoint[i];
                    var point2 = orderedPoint[i + 1];

                    result.Add(new LinePipeSegment(point1, Math.Max(point2.X - point1.X, point2.Y - point1.Y), pipe.Orientation));
                }
            }
            else
            {
                result.Add(new LinePipeSegment(new Point(0, 0),
                    pipe.Orientation == Orientation.Horizontal ? pipe.Width : pipe.Height,
                    pipe.Orientation));
            }

            return result;

            void AddPipeIntersection(Rect p, Rect intersectionRect)
            {
                var ctrlPipe = allPipeRects[p];
                if (!intersections.ContainsKey(ctrlPipe))
                {
                    intersections[ctrlPipe] = new HashSet<Rect>();
                }

                intersections[ctrlPipe].Add(intersectionRect);
            }
        }
    }

    internal interface IPipeSegment
    {
        Point StartPoint { get; }

        double Length { get; }

        Orientation Orientation { get; }
    }

    internal class LinePipeSegment : IPipeSegment
    {
        public LinePipeSegment(Point startPoint, double length, Orientation orientation)
        {
            StartPoint = startPoint;
            Length = length;
            Orientation = orientation;
        }

        public Point StartPoint { get; }
        public double Length { get; }
        public Orientation Orientation { get; }

        public override string ToString()
        {
            return $"StartPoint: {StartPoint}, lenght: {Length}";
        }
    }


}