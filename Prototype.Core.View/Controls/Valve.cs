using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Prototype.Core.Controls.PipeFlowScheme;
using Prototype.Core.Models;

namespace Prototype.Core.Controls
{
    public sealed class Valve : ValveBase, IValve
    {
        #region Constructors

        static Valve()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Valve), new FrameworkPropertyMetadata(typeof(Valve)));
            AddSubscribedProperties(typeof(Valve), OrientationProperty, VisibilityProperty);
        }

        #endregion

        #region Dependency properties

        internal static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation", typeof(Orientation), typeof(Valve), new PropertyMetadata(Orientation.Horizontal));

        #endregion

        #region Properties

        [Category("Layout")]
        public Orientation Orientation
        {
            get { return (Orientation) GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        #endregion

        #region Methods

        public override bool CanPassFlow(IFlowGraph graph, IPipeSegment pipeSegment)
        {
            return State == ValveState.Open;
        }

        #endregion
    }
}