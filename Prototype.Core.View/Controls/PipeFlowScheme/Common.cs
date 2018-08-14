﻿using System;
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
        internal const double PipeEndBorderOffset = PipeBorderWidth + PipeSubstanceWidth;
        internal const double DefaultPipeLength = 5;
        internal const double DefaultPipeHeight = 100;

        internal const double BridgeLength = 17;
        internal const double BridgeOffset = (BridgeLength - PipeWidth) / 2;
        internal static Vector ConnectorVector = new Vector(PipeWidth, PipeWidth);

        #endregion

        #region Common

        private static bool AreClose(double value1, double value2)
        {
            const double DBL_EPSILON = 2.2204460492503131e-016;

            //in case they are Infinities (then epsilon check does not work)
            if (value1 == value2)
            {
                return true;
            }

            // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < DBL_EPSILON
            double eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * DBL_EPSILON;
            double delta = value1 - value2;
            return (-eps < delta) && (eps > delta);
        }

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
                    return AreClose(rect.Height, PipeWidth) && rect.Width > 10;
                case Orientation.Vertical:
                    return AreClose(rect.Width, PipeWidth) && rect.Height > 10;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
        }

        internal static bool IsIntersectionSizeValid(Rect intersectionRect)
        {
            return AreClose(intersectionRect.Width, PipeWidth) &&
                   AreClose(intersectionRect.Height, PipeWidth);
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

        internal static IReadOnlyList<IPipeSegment> CreateFailedSegments(GraphPipe pipe)
        {
            var result = new List<IPipeSegment>
            {
                new LineSegment(
                    new Point(0, 0),
                    GetLength(pipe.Rect, pipe.Orientation),
                    pipe.Orientation,
                    pipe.Pipe.SubstanceType,
                    pipe.FailType)
            };

            return result;
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

        public static bool IsEnoughSpaceForBridgeConnection(GraphPipe pipe, Rect intersectionRect)
        {
            const double bridgeSpace = BridgeOffset + PipeWidth; // PipeWidth for start/end connectors

            switch (pipe.Orientation)
            {
                case Orientation.Horizontal:
                    return intersectionRect.Left - pipe.Rect.Left >= bridgeSpace &&
                           pipe.Rect.Right - intersectionRect.Right >= bridgeSpace;
                case Orientation.Vertical:
                    return intersectionRect.Top - pipe.Rect.Top >= bridgeSpace &&
                           pipe.Rect.Bottom - intersectionRect.Bottom >= bridgeSpace;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static bool IsEnoughSpaceBetweenBridgeConnectors(IPipeConnector c1, IPipeConnector c2)
        {
            // TODO
            var hor = c2.Rect.Left - c1.Rect.Left;
            var ver = c2.Rect.Top - c1.Rect.Top;
            return (hor == 0 || hor >= 27) && (ver == 0 || ver >= 27);
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