using System;
using System.Windows;
using System.Windows.Controls;
using Prototype.Core.Controls;

namespace Prototype.Core.TemplateSelectors
{
    internal class PipeItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ConnectorTemplate { get; set; }

        public DataTemplate BridgeTemplate { get; set; }

        public DataTemplate LineTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            switch (item)
            {
                case ConnectorSegment _:
                    return ConnectorTemplate;

                case BridgeSegment _:
                    return BridgeTemplate;

                case LinePipeSegment _:
                    return LineTemplate;

                case null:
                    return null;

                default:
                    throw new Exception("!!!");
            }
        }
    }
}