using System;
using System.Windows;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    public static class PipeFlowScheme
    {
        #region Attached properties

        internal const string IsSourcePropertyName = "IsSource";
        internal const string IsDestinationPropertyName = "IsDestination";

        public static readonly DependencyProperty IsSourceProperty = DependencyProperty.RegisterAttached(
            IsSourcePropertyName, typeof(bool), typeof(PipeFlowScheme),
            new PropertyMetadata(false, PropertyChangedCallback));

        public static void SetIsSource(DependencyObject element, bool value)
        {
            element.SetValue(IsSourceProperty, value);
        }

        public static bool GetIsSource(DependencyObject element)
        {
            return (bool) element.GetValue(IsSourceProperty);
        }

        public static readonly DependencyProperty IsDestinationProperty = DependencyProperty.RegisterAttached(
            IsDestinationPropertyName, typeof(bool), typeof(PipeFlowScheme),
            new PropertyMetadata(false, PropertyChangedCallback));

        public static void SetIsDestination(DependencyObject element, bool value)
        {
            element.SetValue(IsDestinationProperty, value);
        }

        public static bool GetIsDestination(DependencyObject element)
        {
            return (bool) element.GetValue(IsDestinationProperty);
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (GetIsDestination(d) && GetIsSource(d))
            {
                throw new InvalidOperationException("Pipe can't be Source and Destionation at the same time.");
            }
        }

        #endregion
    }
}