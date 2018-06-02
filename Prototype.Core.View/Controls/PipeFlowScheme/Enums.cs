using System;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    internal enum PipeType
    {
        Regular,
        Source,
        Destination
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
}
