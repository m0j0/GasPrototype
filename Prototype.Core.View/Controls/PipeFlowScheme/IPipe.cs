using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    public interface ISchemeContainer
    {
    }

    public interface ISchemeContainerOwner
    {
        IEnumerable ChildrenControls { get; }

        Rect LayoutRect { get; }

        bool IsLoaded { get; }

        bool IsChildContainer { get; }
    }

    public interface IFlowControl
    {
        Rect LayoutRect { get; }

        Vector Offset { get; set; }

        bool IsVisible { get; }

        ISchemeContainer SchemeContainer { get; set; }

        event EventHandler SchemeChanged;
    }

    public interface IPipe : IFlowControl
    {
        Orientation Orientation { get; }

        SubstanceType SubstanceType { get; }

        PipeType Type { get; }

        IList<IPipeSegment> Segments { get; set; }
    }

    public interface IValve : IFlowControl
    {
        bool CanPassFlow(IFlowGraph graph, IPipeSegment pipeSegment);

        event EventHandler StateChanged;
    }
}