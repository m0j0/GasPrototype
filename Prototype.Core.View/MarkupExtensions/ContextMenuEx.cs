using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Prototype.Core.Interfaces;

namespace Prototype.Core.MarkupExtensions
{
    public static class ContextMenuEx
    {
        public static readonly DependencyProperty MenuProperty = DependencyProperty.RegisterAttached(
            "Menu", typeof(IMenu), typeof(ContextMenuEx), new PropertyMetadata(default(IMenu)));

        public static void SetMenu(DependencyObject element, IMenu value)
        {
            element.SetValue(MenuProperty, value);
        }

        public static IMenu GetMenu(DependencyObject element)
        {
            return (IMenu) element.GetValue(MenuProperty);
        }
    }
}