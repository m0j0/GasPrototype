using System.Windows;

namespace Prototype.Infrastructure
{
    internal static class AttachedProperties
    {
        public static readonly DependencyProperty WrapperWidthProperty = DependencyProperty.RegisterAttached(
            "WrapperWidth", typeof(double), typeof(AttachedProperties), new PropertyMetadata(double.NaN));

        public static void SetWrapperWidth(DependencyObject element, double value)
        {
            element.SetValue(WrapperWidthProperty, value);
        }

        public static double GetWrapperWidth(DependencyObject element)
        {
            return (double) element.GetValue(WrapperWidthProperty);
        }

        public static readonly DependencyProperty WrapperHeightProperty = DependencyProperty.RegisterAttached(
            "WrapperHeight", typeof(double), typeof(AttachedProperties), new PropertyMetadata(double.NaN));

        public static void SetWrapperHeight(DependencyObject element, double value)
        {
            element.SetValue(WrapperHeightProperty, value);
        }

        public static double GetWrapperHeight(DependencyObject element)
        {
            return (double) element.GetValue(WrapperHeightProperty);
        }
    }
}