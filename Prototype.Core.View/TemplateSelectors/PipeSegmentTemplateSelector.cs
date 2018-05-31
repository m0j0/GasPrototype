using System;
using System.Windows;
using System.Windows.Controls;
using Prototype.Core.Controls.PipeFlowScheme;

namespace Prototype.Core.TemplateSelectors
{
    internal class PipeSegmentTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ConnectorTemplate { get; set; }

        public DataTemplate BridgeTemplate { get; set; }

        public DataTemplate LineTemplate { get; set; }

        public DataTemplate FailedTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            switch (item)
            {
                case ConnectorSegment _:
                    return ConnectorTemplate;

                case BridgeSegment _:
                    return BridgeTemplate;

                case LineSegment _:
                    return LineTemplate;

                case FailedSegment _:
                    return FailedTemplate;

                case null:
                    return null;

                default:
                    throw new Exception("!!!");
            }
        }
    }
}