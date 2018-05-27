using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    public interface IContainer
    {
        double GetTop(IFlowControl control);

        double GetLeft(IFlowControl control);

        event EventHandler InvalidateRequired;
    }

    public interface IFlowControl
    {
        double Width { get; }

        double Height { get; }

        event EventHandler InvalidateRequired;
    }

    public interface IPipe : IFlowControl
    {
        Orientation Orientation { get; }

        IList<IPipeSegment> Segments { get; set; }

        bool HasFlow { get; set; }
    }

    public interface IValve : IFlowControl
    {
        bool CanPassFlow(IPipe pipe1, IPipe pipe2);
    }
}