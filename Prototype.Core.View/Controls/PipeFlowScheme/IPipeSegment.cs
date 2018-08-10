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

        SubstanceType SubstanceType { get; }

        bool HasFlow { get; set; }
    }

    internal class ConnectorSegment : NotifyPropertyChangedBase, IPipeSegment
    {
        private bool _hasFlow;

        public ConnectorSegment(Point startPoint, Orientation orientation, SubstanceType substanceType, Side side)
        {
            StartPoint = startPoint;
            Orientation = orientation;
            SubstanceType = substanceType;
            Side = side;
        }

        public Point StartPoint { get; }

        public double Length => Common.PipeWidth;

        public Orientation Orientation { get; }

        public SubstanceType SubstanceType { get; }

        public bool HasFlow
        {
            get => _hasFlow;
            set
            {
                if (value == _hasFlow)
                {
                    return;
                }

                _hasFlow = value;
                OnPropertyChanged();
            }
        }

        public Side Side { get; }
    }

    internal class BridgeSegment : NotifyPropertyChangedBase, IPipeSegment
    {
        private bool _hasFlow;

        public BridgeSegment(Point startPoint, Orientation orientation, SubstanceType substanceType)
        {
            StartPoint = startPoint;
            Orientation = orientation;
            SubstanceType = substanceType;
        }

        public Point StartPoint { get; }

        public double Length => Common.BridgeLength;

        public Orientation Orientation { get; }

        public SubstanceType SubstanceType { get; }

        public bool HasFlow
        {
            get => _hasFlow;
            set
            {
                if (value == _hasFlow)
                {
                    return;
                }

                _hasFlow = value;
                OnPropertyChanged();
            }
        }
    }

    internal class LineSegment : NotifyPropertyChangedBase, IPipeSegment
    {
        private bool _hasFlow;

        public LineSegment(Point startPoint, double length, Orientation orientation, SubstanceType substanceType, FailType failType = FailType.None)
        {
            if (length < 0)
            {
                throw new ArgumentException("Length should be positive.");
            }

            StartPoint = startPoint;
            Length = length;
            Orientation = orientation;
            SubstanceType = substanceType;
            FailType = failType;
        }

        public Point StartPoint { get; }

        public double Length { get; }

        public Orientation Orientation { get; }

        public SubstanceType SubstanceType { get; }

        public FailType FailType { get; }

        public bool HasFlow
        {
            get => _hasFlow;
            set
            {
                if (value == _hasFlow)
                {
                    return;
                }

                _hasFlow = value;
                OnPropertyChanged();
            }
        }
    }
}