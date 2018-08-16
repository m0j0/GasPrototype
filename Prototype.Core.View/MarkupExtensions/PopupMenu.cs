using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Prototype.Core.Interfaces;

namespace Prototype.Core.MarkupExtensions
{
    public static class PopupMenu
    {
        public static readonly DependencyProperty MenuProperty = DependencyProperty.RegisterAttached(
            "Menu", typeof(IMenu), typeof(PopupMenu), new PropertyMetadata(default(IMenu), PropertyChangedCallback));

        public static void SetMenu(DependencyObject element, IMenu value)
        {
            element.SetValue(MenuProperty, value);
        }

        public static IMenu GetMenu(DependencyObject element)
        {
            return (IMenu) element.GetValue(MenuProperty);
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null)
            {
                throw new ArgumentException("d", nameof(d));
            }

            var el = d as FrameworkElement;
            if (el == null)
            {
                throw new InvalidOperationException("Only FrameworkElement are supported");
            }

            el.MouseUp += ElementOnMouseUp;
        }

        private static void ElementOnMouseUp(object sender, MouseButtonEventArgs e)
        {
            var element = (FrameworkElement) sender;
            e.Handled = true;

            var cm = ContextMenuService.GetContextMenu(element);
            if (cm == null)
            {
                cm = new ContextMenu
                {
                    PlacementTarget = element,
                    Placement = PlacementMode.Bottom,
                    Style = (Style) element.FindResource("PopupMenuRegularStyle")
                };

                SetMenu(cm, GetMenu(element));
                element.ContextMenu = cm;
            }

            cm.IsOpen = true;
        }
    }
}