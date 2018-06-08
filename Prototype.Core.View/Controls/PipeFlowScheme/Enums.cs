using System;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    public enum PipeType
    {
        Regular,
        Source,
        Destination
    }

    public enum SubstanceType
    {
        Gas = 0,
        Purge = 1,
        Chemical = 2
    }

    internal enum PipeDirection
    {
        None,
        Forward,
        Backward
    }

    [Flags]
    internal enum Side
    {
        None = 0,
        Left = 1 << 0,
        Top = 1 << 1,
        Right = 1 << 2,
        Bottom = 1 << 3,
        All = Left | Right | Top | Bottom
    }

    internal enum FailType
    {
        None,
        WrongSize,
        IntersectionIsNotSupported,
        BridgeNotEnoughSpace,
        DeadPath,
        BothSourceDestination
    }
}