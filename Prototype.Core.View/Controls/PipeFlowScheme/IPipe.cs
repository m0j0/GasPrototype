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

        IList<IPipeSegment> Segments { get; set; }
    }

    public interface IValve : IFlowControl
    {
        bool CanPassFlow(IPipe pipe1, IPipe pipe2);

        event EventHandler StateChanged;
    }
}