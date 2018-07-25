using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    public interface ISchemeContainer
    {
        void Add(IFlowControl flowControl);

        void Remove(IFlowControl flowControl);

        void InvalidateScheme();

        void InvalidateSchemeFlow();
    }

    public interface IFlowGraph
    {
        IPipe FindPipe(IPipeSegment segment);

        Rect GetAbsoluteRect(IPipe pipe);

        Rect GetAbsoluteRect(IValve valve);
    }

    public interface IFlowControl
    {
        Rect LayoutRect { get; }

        Vector Offset { get; }

        bool IsVisible { get; }

        void SetContrainer(ISchemeContainer container, Vector offset);
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
    }
}