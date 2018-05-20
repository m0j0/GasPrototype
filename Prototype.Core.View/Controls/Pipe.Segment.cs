using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Prototype.Core.Controls
{
    internal interface IPipeSegment
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
    }

    internal class BridgeSegment : IPipeSegment
    {
        public BridgeSegment(Point startPoint, Orientation orientation)
        {
            StartPoint = startPoint;
            Orientation = orientation;
        }

        public Point StartPoint { get; }

        public double Length => Pipe.PipeWidth;

        public Orientation Orientation { get; }
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
            return $"Segment StartPoint: {StartPoint}, lenght: {Length}";
        }
    }

    internal class EmptySegment : IPipeSegment
    {
        public EmptySegment(Point startPoint, Orientation orientation)
        {
            StartPoint = startPoint;
            Orientation = orientation;
        }

        public Point StartPoint { get; }
        public double Length => 0;
        public Orientation Orientation { get; }

        public override string ToString()
        {
            return $"Segment StartPoint: {StartPoint}, lenght: {Length}";
        }
    }
}