using System;
using System.Windows;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    internal static class PipeDrawing
    {
        public static bool IsIntersect(Rect a, Rect b)
        {
            return a.Left <= b.Right &&
                   b.Left <= a.Right &&
                   a.Top <= b.Bottom &&
                   b.Top <= a.Bottom;
        }

        public static bool IsBridgeConnection(ProcessPipe pipe1, ProcessPipe pipe2, Rect intersectionRect)
        {
            return pipe1.Rect.Left < intersectionRect.Left &&
                   pipe1.Rect.Right > intersectionRect.Right &&
                   pipe2.Rect.Top < intersectionRect.Top &&
                   pipe2.Rect.Bottom > intersectionRect.Bottom;
        }

        public static bool IsCornerConnection(ProcessPipe pipe1, ProcessPipe pipe2, Rect intersectionRect)
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

        public static bool IsSerialConnection(ProcessPipe pipe1, ProcessPipe pipe2, Rect intersectionRect)
        {
            bool IsVertical(ProcessPipe p1, ProcessPipe p2)
            {
                return p1.Rect.BottomRight == p2.Rect.TopLeft + Common.ConnectorVector &&
                       p1.Rect.BottomLeft == intersectionRect.BottomLeft;
            }

            bool IsHorizontal(ProcessPipe p1, ProcessPipe p2)
            {
                return p1.Rect.BottomRight == p2.Rect.TopLeft + Common.ConnectorVector &&
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