using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    public sealed class PipeFlowCanvas : Canvas, IContainer
    {
        private readonly PipeFlowScheme _scheme;

        public PipeFlowCanvas()
        {
            _scheme = new PipeFlowScheme(this);

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        public double GetTop(IFlowControl control)
        {
            return GetTop((UIElement) control);
        }

        public double GetLeft(IFlowControl control)
        {
            return GetLeft((UIElement) control);
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            if (!IsLoaded)
            {
                return;
            }

            if (visualAdded is IFlowControl addedControl)
            {
                _scheme.Add(addedControl);
            }

            if (visualRemoved is IFlowControl removedControl)
            {
                _scheme.Remove(removedControl);
            }
        }
        
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _scheme.Add(Children.OfType<IFlowControl>());
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // TODO Dispose
        }
    }
}