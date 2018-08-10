using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Imaging;
using Prototype.Core.Controls.PipeFlowScheme;
using Prototype.Core.Models;

namespace Prototype.Core.Controls
{
    public sealed class Valve3Way : ValveBase, IValve3Way
    {
        #region Fields

        private readonly Valve3WayModel _model;

        #endregion

        #region Constructors

        static Valve3Way()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Valve3Way), new FrameworkPropertyMetadata(typeof(Valve3Way)));
            SetSubscribedProperties(typeof(Valve3Way), RotationProperty);
        }

        public Valve3Way()
        {
            _model = new Valve3WayModel(this);
        }

        #endregion

        #region Dependency properties

        public static readonly DependencyProperty RotationProperty = DependencyProperty.Register(
            "Rotation", typeof(Rotation), typeof(Valve3Way), new PropertyMetadata(Rotation.Rotate0));

        public static readonly DependencyProperty PathWhenOpenProperty = DependencyProperty.Register(
            "PathWhenOpen", typeof(Valve3WayFlowPath), typeof(Valve3Way), new PropertyMetadata(default(Valve3WayFlowPath), OnStatePropertyChangedCallback));

        public static readonly DependencyProperty PathWhenClosedProperty = DependencyProperty.Register(
            "PathWhenClosed", typeof(Valve3WayFlowPath), typeof(Valve3Way), new PropertyMetadata(default(Valve3WayFlowPath), OnStatePropertyChangedCallback));

        #endregion

        #region Properties

        [Category("Layout")]
        public Rotation Rotation
        {
            get { return (Rotation) GetValue(RotationProperty); }
            set { SetValue(RotationProperty, value); }
        }

        [Category("Model")]
        public Valve3WayFlowPath PathWhenOpen
        {
            get { return (Valve3WayFlowPath) GetValue(PathWhenOpenProperty); }
            set { SetValue(PathWhenOpenProperty, value); }
        }

        [Category("Model")]
        public Valve3WayFlowPath PathWhenClosed
        {
            get { return (Valve3WayFlowPath) GetValue(PathWhenClosedProperty); }
            set { SetValue(PathWhenClosedProperty, value); }
        }

        bool IValve3Way.IsOpen => State == ValveState.Open;

        #endregion

        #region Methods

        public override bool CanPassFlow(IFlowGraph graph, IPipeSegment pipeSegment)
        {
            return _model.CanPassFlow(graph, pipeSegment);
        }

        #endregion
    }
}