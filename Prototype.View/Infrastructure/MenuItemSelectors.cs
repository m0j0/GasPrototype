using System.Windows;
using System.Windows.Controls;
using Prototype.Core.Interfaces;

namespace Prototype.Infrastructure
{
    public class MenuItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate HeaderTemplate { get; set; }

        public DataTemplate ItemTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            switch (item)
            {
                case IMenu _:
                    return HeaderTemplate;
                case IMenuItem _:
                    return ItemTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }

    public class MenuItemContainerStyleSelector : StyleSelector
    {
        public Style HeaderStyle { get; set; }

        public Style ItemStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            switch (item)
            {
                case IMenu _:
                    return HeaderStyle;
                case IMenuItem _:
                    return ItemStyle;
            }

            return base.SelectStyle(item, container);
        }
    }
}