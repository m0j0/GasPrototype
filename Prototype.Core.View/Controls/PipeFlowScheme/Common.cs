using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    internal static class Common
    {
        #region Constants

        internal const double PipeWidth = 5;
        internal const double PipeBorderWidth = 1;
        internal const double PipeSubstanceWidth = 3;
        internal const double DefaultPipeLength = 5;
        internal const double DefaultPipeHeight = 100;

        internal const double BridgeLength = 27;
        internal const double BridgeOffset = (BridgeLength - PipeWidth) / 2;
        internal static Vector ConnectorVector = new Vector(PipeWidth, PipeWidth);

        #endregion

        #region Size methods

        internal static double GetBridgeHorizontalConnectorOffset(Orientation pipeOrientation)
        {
            return pipeOrientation == Orientation.Horizontal ? BridgeOffset : 0;
        }

        internal static double GetBridgeVerticalConnectorOffset(Orientation pipeOrientation)
        {
            return pipeOrientation == Orientation.Horizontal ? 0 : BridgeOffset;
        }

        internal static bool IsSizeValid(GraphPipe pipe)
        {
            return IsSizeValid(pipe.Rect, pipe.Orientation);
        }

        internal static bool IsSizeValid(Rect rect, Orientation orientation)
        {
            if (double.IsInfinity(rect.Top) || double.IsInfinity(rect.Left) ||
                double.IsNaN(rect.Top) || double.IsNaN(rect.Left))
            {
                return false;
            }

            switch (orientation)
            {
                case Orientation.Horizontal:
                    return rect.Height == PipeWidth && rect.Width > 10;
                case Orientation.Vertical:
                    return rect.Width == PipeWidth && rect.Height > 10;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
        }

        internal static bool IsIntersectionSizeValid(Rect intersectionRect)
        {
            return intersectionRect.Width == Common.PipeWidth &&
                   intersectionRect.Height == Common.PipeWidth;
        }

        internal static double GetLength(Rect rect, Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.Horizontal:
                    return rect.Width;
                case Orientation.Vertical:
                    return rect.Height;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
        }

        internal static LineSegment CreateSegmentBetweenSegments(GraphPipe pipe, IPipeSegment s1, IPipeSegment s2)
        {
            if (pipe.Direction == PipeDirection.Backward)
            {
                var tmp = s1;
                s1 = s2;
                s2 = tmp;
            }

            if (pipe.Orientation == Orientation.Horizontal)
            {
                return new LineSegment(new Point(s1.StartPoint.X + s1.Length, 0),
                    s2.StartPoint.X - (s1.StartPoint.X + s1.Length),
                    pipe.Orientation,
                    pipe.Pipe.SubstanceType);
            }

            return new LineSegment(new Point(0, s1.StartPoint.Y + s1.Length),
                s2.StartPoint.Y - (s1.StartPoint.Y + s1.Length),
                pipe.Orientation,
                pipe.Pipe.SubstanceType);
        }

        #endregion

        #region Intersections

        public static bool IsIntersect(Rect a, Rect b)
        {
            return a.Left <= b.Right &&
                   b.Left <= a.Right &&
                   a.Top <= b.Bottom &&
                   b.Top <= a.Bottom;
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

        public static bool IsBridgeConnection(GraphPipe pipe1, GraphPipe pipe2, Rect intersectionRect)
        {
            return pipe1.Rect.Left < intersectionRect.Left &&
                   pipe1.Rect.Right > intersectionRect.Right &&
                   pipe2.Rect.Top < intersectionRect.Top &&
                   pipe2.Rect.Bottom > intersectionRect.Bottom;
        }

        public static bool IsCornerConnection(GraphPipe pipe1, GraphPipe pipe2, Rect intersectionRect)
        {
            bool IsUpperLeft(GraphPipe p1, GraphPipe p2)
            {
                return p1.Rect.TopLeft == p2.Rect.TopLeft &&
                       p1.Rect.TopLeft == intersectionRect.TopLeft;
            }

            bool IsUpperRight(GraphPipe p1, GraphPipe p2)
            {
                return p1.Rect.TopRight == p2.Rect.TopRight &&
                       p1.Rect.TopRight == intersectionRect.TopRight;
            }

            bool IsLowerLeft(GraphPipe p1, GraphPipe p2)
            {
                return p1.Rect.BottomLeft == p2.Rect.BottomLeft &&
                       p1.Rect.BottomLeft == intersectionRect.BottomLeft;
            }

            bool IsLowerRight(GraphPipe p1, GraphPipe p2)
            {
                return p1.Rect.BottomRight == p2.Rect.BottomRight &&
                       p1.Rect.BottomRight == intersectionRect.BottomRight;
            }

            return IsUpperLeft(pipe1, pipe2) || IsUpperLeft(pipe2, pipe1) ||
                   IsUpperRight(pipe1, pipe2) || IsUpperRight(pipe2, pipe1) ||
                   IsLowerLeft(pipe1, pipe2) || IsLowerLeft(pipe2, pipe1) ||
                   IsLowerRight(pipe1, pipe2) || IsLowerRight(pipe2, pipe1);
        }

        public static bool IsSerialConnection(GraphPipe pipe1, GraphPipe pipe2, Rect intersectionRect)
        {
            bool IsVertical(GraphPipe p1, GraphPipe p2)
            {
                return p1.Rect.BottomRight == p2.Rect.TopLeft + Common.ConnectorVector &&
                       p1.Rect.BottomLeft == intersectionRect.BottomLeft;
            }

            bool IsHorizontal(GraphPipe p1, GraphPipe p2)
            {
                return p1.Rect.BottomRight == p2.Rect.TopLeft + Common.ConnectorVector &&
                       p1.Rect.BottomRight == intersectionRect.BottomRight;
            }

            return IsVertical(pipe1, pipe2) || IsVertical(pipe2, pipe1) ||
                   IsHorizontal(pipe1, pipe2) || IsHorizontal(pipe2, pipe1);
        }

        #endregion

        #region Extensions

        public static bool HasFlagEx(this Side side, Side flag)
        {
            return (side & flag) != 0;
        }

        #endregion
    }
}