using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using MugenMvvmToolkit.Models;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    public interface IPipeSegment : INotifyPropertyChanged
    {
        Point StartPoint { get; }

        double Length { get; }

        Orientation Orientation { get; }

        FlowDirection FlowDirection { get; set; }
    }

    [Flags]
    public enum FlowDirection
    {
        None = 0,
        Forward = 1 << 0,
        Backward = 1 << 1,
        Both = Forward | Backward
    }

    internal enum FailType
    {
        None,
        WrongSize,
        IntersectionNotSupported,
        BridgeNotEnoughSpace
    }

    internal class ConnectorSegment : NotifyPropertyChangedBase, IPipeSegment
    {
        private FlowDirection _flowDirection;

        public ConnectorSegment(Point startPoint, Orientation orientation, Side side)
        {
            StartPoint = startPoint;
            Orientation = orientation;
            Side = side;
        }

        public Point StartPoint { get; }

        public double Length => Common.PipeWidth;

        public Orientation Orientation { get; }

        public FlowDirection FlowDirection
        {
            get => _flowDirection;
            set
            {
                if (value == _flowDirection)
                {
                    return;
                }

                _flowDirection = value;
                OnPropertyChanged();
            }
        }

        public Side Side { get; }
    }

    internal class BridgeSegment : NotifyPropertyChangedBase, IPipeSegment
    {
        private FlowDirection _flowDirection;

        public BridgeSegment(Point startPoint, Orientation orientation)
        {
            StartPoint = startPoint;
            Orientation = orientation;
        }

        public Point StartPoint { get; }

        public double Length => Common.BridgeLength;

        public Orientation Orientation { get; }

        public FlowDirection FlowDirection
        {
            get => _flowDirection;
            set
            {
                if (value == _flowDirection)
                {
                    return;
                }

                _flowDirection = value;
                OnPropertyChanged();
            }
        }
    }

    internal class LineSegment : NotifyPropertyChangedBase, IPipeSegment
    {
        private FlowDirection _flowDirection;

        public LineSegment(Point startPoint, double length, Orientation orientation)
        {
            StartPoint = startPoint;
            Length = length;
            Orientation = orientation;
        }

        public Point StartPoint { get; }

        public double Length { get; }

        public Orientation Orientation { get; }

        public FlowDirection FlowDirection
        {
            get => _flowDirection;
            set
            {
                if (value == _flowDirection)
                {
                    return;
                }

                _flowDirection = value;
                OnPropertyChanged();
            }
        }
    }

    internal class FailedSegment : NotifyPropertyChangedBase, IPipeSegment
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

        FlowDirection IPipeSegment.FlowDirection { get; set; }
    }
}