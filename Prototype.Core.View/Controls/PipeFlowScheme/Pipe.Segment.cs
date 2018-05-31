using System.Windows;
using System.Windows.Controls;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    public interface IPipeSegment
    {
        Point StartPoint { get; }

        double Length { get; }

        Orientation Orientation { get; }
    }

    internal enum FailType
    {
        WrongSize,
        IntersectionNotSupported,
        BridgeNotEnoughSpace
    }

    internal class ConnectorSegment : IPipeSegment
    {
        public ConnectorSegment(Point startPoint, Orientation orientation, Side side)
        {
            StartPoint = startPoint;
            Orientation = orientation;
            Side = side;
        }

        public Point StartPoint { get; }

        public double Length => Common.PipeWidth;

        public Orientation Orientation { get; }

        public Side Side { get; }
    }

    internal class BridgeSegment : IPipeSegment
    {
        public BridgeSegment(Point startPoint, Orientation orientation)
        {
            StartPoint = startPoint;
            Orientation = orientation;
        }

        public Point StartPoint { get; }

        public double Length => Common.BridgeLength;

        public Orientation Orientation { get; }
    }

    internal class LineSegment : IPipeSegment
    {
        public LineSegment(Point startPoint, double length, Orientation orientation)
        {
            StartPoint = startPoint;
            Length = length;
            Orientation = orientation;
        }

        public Point StartPoint { get; }

        public double Length { get; }

        public Orientation Orientation { get; }
    }

    internal class FailedSegment : IPipeSegment
    {
        public FailedSegment(Point startPoint, double length, Orientation orientation, FailType failType)
        {
            StartPoint = startPoint;
            Length = length;
            Orientation = orientation;
            FailType = failType;
        }

        public Point StartPoint { get; }

        public double Length { get; }

        public Orientation Orientation { get; }

        public FailType FailType { get; }
    }
}