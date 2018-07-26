using System.Windows;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    public static class FlowSchemeSettings
    {
        #region IsFlowSchemeContainer

        public static readonly DependencyProperty IsFlowSchemeContainerProperty = DependencyProperty.RegisterAttached(
            "IsFlowSchemeContainer", typeof(bool), typeof(FlowSchemeSettings), new PropertyMetadata(default(bool)));

        public static void SetIsFlowSchemeContainer(DependencyObject element, bool value)
        {
            element.SetValue(IsFlowSchemeContainerProperty, value);
        }

        public static bool GetIsFlowSchemeContainer(DependencyObject element)
        {
            return (bool) element.GetValue(IsFlowSchemeContainerProperty);
        }

        #endregion

        #region Container

        internal static readonly DependencyProperty ContainerProperty = DependencyProperty.RegisterAttached(
            "Container", typeof(ISchemeContainer), typeof(FlowSchemeSettings), new PropertyMetadata(default(ISchemeContainer)));

        internal static void SetContainer(DependencyObject element, ISchemeContainer value)
        {
            element.SetValue(ContainerProperty, value);
        }

        internal static ISchemeContainer GetContainer(DependencyObject element)
        {
            return (ISchemeContainer) element.GetValue(ContainerProperty);
        }

        #endregion
    }
}