using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            }

            return null;
        }
    }
}