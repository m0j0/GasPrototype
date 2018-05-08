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

        /*public static void FindAllIntersections(IReadOnlyCollection<PipeControlModel> allPipes)
        {
            var allIntersections = new Dictionary<Rect, Intersection>();
            foreach (var pipe1 in allPipes)
            {
                foreach (var pipe2 in allPipes)
                {
                    if (pipe1 == pipe2)
                    {
                        continue;
                    }

                    var intersectionRect = Intersect(
                        GetPipeAbsoluteRectangle(pipe1.Pipe),
                        GetPipeAbsoluteRectangle(pipe2.Pipe)
                    );

                    if (intersectionRect == Rect.Empty)
                    {
                        continue;
                    }

                    if (intersectionRect.Width < Pipe.PipeWidth &&
                        intersectionRect.Height < Pipe.PipeWidth)
                    {
                        continue;
                    }

/*                    if (intersectionRect.Height == 5 && intersectionRect.Width == 5)
                    {

                    }#1#

                    Intersection intersection;
                    if (!allIntersections.TryGetValue(intersectionRect, out intersection))
                    {
                        intersection = new Intersection();
                        allIntersections[intersectionRect] = intersection;
                    }
                    
                    var pipeIntersectionProjection =
                        new PipeIntersectionSegment(
                            new Point(intersectionRect.X - pipe1.AbsoluteRectangle.X, intersectionRect.Y - pipe1.AbsoluteRectangle.Y),
                            intersectionRect,
                            Side.All,
                            pipe1,
                            pipe2);

                    pipe1.Intersections.Add(pipeIntersectionProjection);

                    intersection.Segments.Add(pipeIntersectionProjection);
                }
            }
        }*/
    }
    

    internal interface IPipeSegment
    {
        Point StartPoint { get; }

        double Length { get; }

        Orientation Orientation { get; }
    }
    
}