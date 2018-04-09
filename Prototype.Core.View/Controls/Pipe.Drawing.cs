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
        #region Extensions

        

        #endregion

        #region Methods

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

        public static void FindAllIntersections(IReadOnlyCollection<PipeControlModel> allPipes)
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

                    }*/

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
        }

        public static List<IPipeSegment> SplitPipeToSegments(Pipe pipeControl, IReadOnlyCollection<PipeControlModel> allPipes)
        {
            var segments = new List<IPipeSegment>();

            var currentPipe = allPipes.Single(pipe => ReferenceEquals(pipe.Pipe, pipeControl));

            var visibleIntersections = currentPipe
                .Intersections
                .Where(intersection => intersection.IsNeedToDrawIntersection)
                .OrderBy(intersection => intersection.GetComparePoint())
                .ToList();
            
            if (visibleIntersections.Count > 0)
            {
                var firstPoint = visibleIntersections.First();

                if (GetRectPoint(currentPipe.Pipe, currentPipe.RelativeRectangle) != firstPoint.GetComparePoint())
                {
                    segments.Add(currentPipe.CreateSegmentFromStart(firstPoint));
                }

                for (int i = 0; i < visibleIntersections.Count - 1; i++)
                {
                    segments.Add(visibleIntersections[i]);
                    segments.Add(currentPipe.CreateMiddleSegment(visibleIntersections[i], visibleIntersections[i + 1]));
                }


                var lastPoint = visibleIntersections.Last();
                segments.Add(lastPoint);

                if (GetRectLastPoint(currentPipe.Pipe, currentPipe.RelativeRectangle) != lastPoint.GetComparePoint())
                {
                    segments.Add(currentPipe.CreateSegmentToEnd(lastPoint));
                }
            }
            else
            {
                segments.Add(new PipeSegment(
                    GetSegmentLength(currentPipe.Pipe.Orientation, currentPipe.RelativeRectangle), 
                    currentPipe.Pipe.Orientation,
                    currentPipe.RelativeRectangle.Location));
            }

            return segments;
        }

        public static Rect GetPipeAbsoluteRectangle(Pipe pipe)
        {
            return new Rect(Canvas.GetLeft(pipe), Canvas.GetTop(pipe), pipe.Width, pipe.Height);
        }

        private static double GetRectPoint(Pipe pipe, Rect rect)
        {
            switch (pipe.Orientation)
            {
                case Orientation.Horizontal:
                    return rect.X;

                case Orientation.Vertical:
                    return rect.Y;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static double GetRectLastPoint(Pipe pipe, Rect rect)
        {
            switch (pipe.Orientation)
            {
                case Orientation.Horizontal:
                    return rect.Right - 5;

                case Orientation.Vertical:
                    return rect.Bottom - 5;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static double GetSegmentLength(Orientation orientation, Rect rect)
        {
            switch (orientation)
            {
                case Orientation.Horizontal:
                    return rect.Width;

                case Orientation.Vertical:
                    return rect.Height;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }     
        
        
        private static Point ConverPoint(Orientation orientation, Point point)
        {
            switch (orientation)
            {
                case Orientation.Horizontal:
                    return new Point(point.X + 5, point.Y);

                case Orientation.Vertical:
                    return new Point(point.X, point.Y + 5);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        public static Side OrientationToSide(Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.Horizontal:
                    return Side.TopBottom;
                    
                case Orientation.Vertical:
                    return Side.LeftRight;

                default:
                    throw new ArgumentOutOfRangeException("orientation", orientation, null);
            }
        }
    }

    internal class PipeControlModel
    {
        private readonly Pipe _pipe;
        private readonly Rect _absoluteRectangle;
        private readonly Rect _relativeRectangle;
        private readonly IList<PipeIntersectionSegment> _intersections;

        public PipeControlModel(Pipe pipe)
        {
            _pipe = pipe;
            _absoluteRectangle = PipeEx.GetPipeAbsoluteRectangle(pipe);
            _relativeRectangle = new Rect(new Point(0, 0), new Point(Pipe.Width, Pipe.Height));
            _intersections = new List<PipeIntersectionSegment>();
        }

        public Pipe Pipe
        {
            get { return _pipe; }
        }

        public Rect AbsoluteRectangle
        {
            get { return _absoluteRectangle; }
        }

        public Rect RelativeRectangle
        {
            get { return _relativeRectangle; }
        }

        public IList<PipeIntersectionSegment> Intersections
        {
            get { return _intersections; }
        }

        public Orientation Orientation
        {
            get { return Pipe.Orientation; }
        }

        public IPipeSegment CreateSegmentFromStart(PipeIntersectionSegment intersectionSegment)
        {
            return new PipeSegment(GetFirstSegmentLength(this, intersectionSegment),
                Orientation, new Point(0, 0));
        }

        public IPipeSegment CreateSegmentToEnd(PipeIntersectionSegment intersectionSegment)
        {
            return new PipeSegment(GetLastSegmentLength(this, intersectionSegment),
                Orientation,
                ConvertPoint(Orientation, intersectionSegment)
            );
        }

        public IPipeSegment CreateMiddleSegment(PipeIntersectionSegment leftIntersectionSegment, PipeIntersectionSegment rightIntersectionSegment)
        {
            return new PipeSegment(GetSegmentLengthBetweenPoints(Orientation, leftIntersectionSegment.StartPoint, rightIntersectionSegment.StartPoint),
                Orientation,
                ConvertPoint(Orientation, leftIntersectionSegment)
                );
        }

        //////////////

        private static Point ConvertPoint(Orientation orientation, PipeIntersectionSegment intersectionSegment)
        {
            switch (orientation)
            {
                case Orientation.Horizontal:
                    return new Point(intersectionSegment.StartPoint.X + intersectionSegment.Length, intersectionSegment.StartPoint.Y);

                case Orientation.Vertical:
                    return new Point(intersectionSegment.StartPoint.X, intersectionSegment.StartPoint.Y + intersectionSegment.Length);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static double GetSegmentLengthBetweenPoints(Orientation orientation, Point startPoint, Point endPoint)
        {
            switch (orientation)
            {
                case Orientation.Horizontal:
                    return endPoint.X - startPoint.X - 5;

                case Orientation.Vertical:
                    return endPoint.Y - startPoint.Y - 5;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static double GetLastSegmentLength(PipeControlModel pipe, PipeIntersectionSegment intersectionSegment)
        {
            switch (pipe.Pipe.Orientation)
            {
                case Orientation.Horizontal:
                    return pipe.RelativeRectangle.Width - intersectionSegment.StartPoint.X;

                case Orientation.Vertical:
                    return pipe.RelativeRectangle.Height - intersectionSegment.StartPoint.Y;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }



        private static double GetFirstSegmentLength(PipeControlModel pipe, PipeIntersectionSegment intersectionSegment)
        {
            switch (pipe.Pipe.Orientation)
            {
                case Orientation.Horizontal:
                    return intersectionSegment.StartPoint.X - pipe.RelativeRectangle.X;

                case Orientation.Vertical:
                    return intersectionSegment.StartPoint.Y - pipe.RelativeRectangle.Y;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    internal interface IPipeSegment
    {
        Point StartPoint { get; }

        double Length { get; }

        Orientation Orientation { get; }
    }

    internal class PipeIntersectionSegment : IPipeSegment
    {
        private readonly Point _startPoint;
        private readonly Rect _absoluteRectangle;
        private readonly PipeControlModel _pipeOwner;
        private Side _side;

        public PipeIntersectionSegment(Point startPoint, Rect absoluteRectangle, Side side, PipeControlModel pipeOwner, PipeControlModel secondPipe)
        {
            _startPoint = startPoint;
            _absoluteRectangle = absoluteRectangle;
            _side = side;
            _pipeOwner = pipeOwner;
        }
        
        public Side Side
        {
            get { return _side; }
            set { _side = value; }
        }

        public bool IsDrawn { get; set; }

        public Point StartPoint
        {
            get { return _startPoint; }
        }

        public double Length
        {
            get { return Math.Min(_absoluteRectangle.Width, _absoluteRectangle.Height); }
        }

        Orientation IPipeSegment.Orientation
        {
            get { return _absoluteRectangle.Width > _absoluteRectangle.Height ? Orientation.Vertical : Orientation.Horizontal; }
        }

        public override string ToString()
        {
            return _startPoint.ToString();
        }

        public bool IsNeedToDrawIntersection
        {
            get { return Length > 0; }
        }

        public double GetComparePoint()
        {
            switch (_pipeOwner.Orientation)
            {
                case Orientation.Horizontal:
                    return _startPoint.X;

                case Orientation.Vertical:
                    return _startPoint.Y;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    internal class PipeSegment : IPipeSegment
    {
        private readonly Point _startPoint;
        private readonly double _length;
        private readonly Orientation _orientation;

        public PipeSegment(double length, Orientation orientation, Point startPoint)
        {
            _length = length;
            _orientation = orientation;
            _startPoint = startPoint;
        }

        public Point StartPoint
        {
            get { return _startPoint; }
        }

        public double Length
        {
            get { return _length; }
        }

        public Side Side
        {
            get { return PipeEx.OrientationToSide(_orientation); }
        }

        public Orientation Orientation
        {
            get { return _orientation; }
        }
    }

    internal enum IntersectionType
    {
        Crossing,
        Bridge,
        Turn
    }

    internal class Intersection
    {
        private readonly List<PipeIntersectionSegment> _segments;

        public Intersection()
        {
            _segments = new List<PipeIntersectionSegment>();
        }

        public IList<PipeIntersectionSegment> Segments
        {
            get { return _segments; }
        }

/*        public Rect AbsoluteRectangle { get; }

        public Side Side { get; }

        public IntersectionType IntersectionType { get; }*/
    }
}