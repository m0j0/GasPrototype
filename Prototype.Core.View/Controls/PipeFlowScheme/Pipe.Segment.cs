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

    internal class ConnectorSegment : IPipeSegment
    {
        public ConnectorSegment(Point startPoint, Orientation orientation, Side side)
        {
            StartPoint = startPoint;
            Orientation = orientation;
            Side = side;
        }

        public Point StartPoint { get; }

        public double Length => Pipe.PipeWidth;

        public Orientation Orientation { get; }

        public Side Side { get; }
        public bool IsFailed { get; } // TODO delete
    }

    internal class BridgeSegment : IPipeSegment
    {
        public BridgeSegment(Point startPoint, Orientation orientation)
        {
            StartPoint = startPoint;
            Orientation = orientation;
        }

        public Point StartPoint { get; }

        public double Length => Pipe.PipeWidth * 3;

        public Orientation Orientation { get; }
    }

    internal class LinePipeSegment : IPipeSegment
    {
        public LinePipeSegment(Point startPoint, double length, Orientation orientation, bool isFailed)
        {
            StartPoint = startPoint;
            Length = length;
            Orientation = orientation;
            IsFailed = isFailed;
        }

        public Point StartPoint { get; }
        public double Length { get; }
        public Orientation Orientation { get; }
        public bool IsFailed { get; }

        public override string ToString()
        {
            return $"Segment StartPoint: {StartPoint}, lenght: {Length}";
        }
    }
}