using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using Prototype.Core.Controls;
using Prototype.Core.Design.Pipes;

namespace Prototype.Core.Design
{
    public class VisualStudioMetadata : IProvideAttributeTable
    {
        public VisualStudioMetadata()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();

            builder.AddCustomAttributes(typeof(Pipe), new FeatureAttribute(typeof(PipeTaskPanelAdornerProvider)));

            AttributeTable = builder.CreateTable();
        }

        public AttributeTable AttributeTable { get; }
    }
}