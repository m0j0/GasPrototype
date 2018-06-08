using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    public interface ISchemeContainer
    {
        double GetTop(IFlowControl control);

        double GetLeft(IFlowControl control);

        event EventHandler SchemeChanged;
    }

    public interface IFlowControl
    {
        double Width { get; }

        double Height { get; }

        ISchemeContainer GetContainer();

        event EventHandler SchemeChanged;
    }

    public interface IPipe : IFlowControl
    {
        Orientation Orientation { get; }

        PipeType Type { get; }

        IList<IPipeSegment> Segments { get; set; } // TODO Observable / ReadOnly
    }

    public interface IValve : IFlowControl
    {
        bool CanPassFlow(IPipeSegment startPipeSegment, IPipeSegment endPipeSegment);

        event EventHandler StateChanged;
    }
}