using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    public interface ISchemeContainer
    {
        double GetTop(IFlowControl control);

        double GetLeft(IFlowControl control);
    }

    public interface IFlowControl
    {
        double Width { get; }

        double Height { get; }

        bool IsVisible { get; }

        ISchemeContainer GetContainer();

        event EventHandler SchemeChanged;
    }

    public interface IPipe : IFlowControl
    {
        Orientation Orientation { get; }

        PipeType Type { get; }

        IList<IPipeSegment> Segments { get; set; }
    }

    public interface IValve : IFlowControl
    {
        bool CanPassFlow(IFlowGraph graph, IPipeSegment pipeSegment);

        event EventHandler StateChanged;
    }
}