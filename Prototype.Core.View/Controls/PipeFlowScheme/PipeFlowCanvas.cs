using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using MugenMvvmToolkit;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    public sealed class PipeFlowCanvas : Canvas, ISchemeContainerOwner
    {
        private SchemeContainer _schemeContainer;

        public PipeFlowCanvas()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        public IEnumerable ChildrenControls => Children;
        
        Rect ISchemeContainerOwner.LayoutRect => LayoutInformation.GetLayoutSlot(this);

        public bool IsChildContainer { get; set; }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            if (_schemeContainer == null)
            {
                return;
            }
            
            var addedFlowControl = visualAdded as IFlowControl;
            var removedFlowControl = visualRemoved as IFlowControl;

            if (addedFlowControl != null || removedFlowControl != null)
            {
                _schemeContainer.OnVisualChildrenChanged(addedFlowControl, removedFlowControl);
            }
        }
        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
        {
            base.MeasureOverride(constraint);
            double width = base
                .InternalChildren
                .OfType<UIElement>()
                .Max(i => i.DesiredSize.Width + (double)i.GetValue(Canvas.LeftProperty));

            double height = base
                .InternalChildren
                .OfType<UIElement>()
                .Max(i => i.DesiredSize.Height + (double)i.GetValue(Canvas.TopProperty));

            return new Size(width, height);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (IsChildContainer)
            {
                var parentContainer = FindParentContainer();
                if (parentContainer == null)
                {
                    if (ServiceProvider.IsDesignMode)
                    {
                        _schemeContainer = new SchemeContainer(this);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    var parentContainerOwner = (PipeFlowCanvas) parentContainer.Owner;
                    parentContainerOwner._schemeContainer.AddChildContainer(new OwnerOffsetPair(this, parentContainer.Offset));
                    _schemeContainer = parentContainerOwner._schemeContainer;
                    return;
                }
            }
            else
            {

                _schemeContainer = new SchemeContainer(this);
            }
            _schemeContainer.OnLoaded();
        }

        private OwnerOffsetPair FindParentContainer()
        {
            var parent = Parent as FrameworkElement;
            var offset = new Vector();
            while (parent != null)
            {
                if (parent is ISchemeContainerOwner owner)
                {
                    if (!owner.IsChildContainer)
                    {
                        return new OwnerOffsetPair(owner, offset);
                    }
                }

                var layout = LayoutInformation.GetLayoutSlot(parent);
                offset.X += layout.X;
                offset.Y += layout.Y;
                parent = parent.Parent as FrameworkElement;
            }

            return null;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _schemeContainer?.OnUnloaded();
        }
    }

    public class OwnerOffsetPair
    {
        public OwnerOffsetPair(ISchemeContainerOwner owner, Vector offset)
        {
            Owner = owner;
            Offset = offset;
        }

        public ISchemeContainerOwner Owner { get; }
        public Vector Offset { get; }

    }

    public sealed class SchemeContainer : ISchemeContainer
    {
        private static readonly EventHandler OnFlowControlSchemeChangedEventHandler = OnFlowControlSchemeChanged;
        private static readonly EventHandler OnValveStateChangedEventHandler = OnValveStateChanged;

        private readonly List<OwnerOffsetPair> _children = new List<OwnerOffsetPair>();
        private readonly ISchemeContainerOwner _owner;
        private FlowGraph _scheme;
        private bool _isInvalidateCalled;

        public SchemeContainer(ISchemeContainerOwner owner)
        {
            Should.NotBeNull(owner, nameof(owner));
            _owner = owner;
        }

        public void OnVisualChildrenChanged(IFlowControl visualAdded, IFlowControl visualRemoved)
        {
            if (!_owner.IsLoaded)
            {
                return;
            }

            if (visualAdded != null)
            {
                SubscribePositionChangedEvents(visualAdded);
            }

            if (visualRemoved != null)
            {
                UnsubscribePositionChangedEvents(visualRemoved);
            }

            InvalidateScheme(false);
        }

        public void OnLoaded()
        {
            foreach (var child in _owner.ChildrenControls)
            {
                // TODO
                if (child is IFlowControl flowControl)
                {
                    SubscribePositionChangedEvents(flowControl);
                }
            }

            InvalidateScheme(true);
        }

        public void OnUnloaded()
        {
            foreach (var child in _owner.ChildrenControls)
            {
                // TODO
                if (child is IFlowControl flowControl)
                {
                    UnsubscribePositionChangedEvents(flowControl);
                }
            }
        }

        public void AddChildContainer(OwnerOffsetPair pair)
        {
            _children.Add(pair);
            foreach (var child in pair.Owner.ChildrenControls)
            {
                // TODO
                if (child is IFlowControl flowControl)
                {
                    SubscribePositionChangedEvents(flowControl);
                }
            }
            InvalidateScheme(false);
        }

        private void SubscribePositionChangedEvents(IFlowControl element)
        {
            switch (element)
            {
                case IPipe pipe:
                    pipe.SchemeChanged += OnFlowControlSchemeChangedEventHandler;
                    break;

                case IValve valve:
                    valve.SchemeChanged += OnFlowControlSchemeChangedEventHandler;
                    valve.StateChanged += OnValveStateChangedEventHandler;
                    break;

                default:
                    throw new ArgumentException(nameof(element));
            }
        }

        private void UnsubscribePositionChangedEvents(IFlowControl element)
        {
            switch (element)
            {
                case IPipe pipe:
                    pipe.SchemeChanged -= OnFlowControlSchemeChangedEventHandler;
                    break;

                case IValve valve:
                    valve.SchemeChanged -= OnFlowControlSchemeChangedEventHandler;
                    valve.StateChanged -= OnValveStateChangedEventHandler;
                    break;

                default:
                    throw new ArgumentException(nameof(element));
            }
        }

        private static void OnFlowControlSchemeChanged(object sender, EventArgs e)
        {
            var flowControl = (IFlowControl) sender;
            var schemeContainer = (SchemeContainer) flowControl.SchemeContainer;
            schemeContainer?.InvalidateScheme(false);
        }

        private static void OnValveStateChanged(object sender, EventArgs e)
        {
            var valve = (IValve)sender;
            var schemeContainer = (SchemeContainer)valve.SchemeContainer;
            schemeContainer.InvalidateSchemeFlow();
        }

        private void InvalidateScheme(bool isFirstLoad)
        {
            if (isFirstLoad)
            {
                InvalidateSchemeImpl();
                return;
            }

            if (_isInvalidateCalled)
            {
                return;
            }

            // hack to avoid massive pack of calls to one
            _isInvalidateCalled = true;
            Dispatcher.CurrentDispatcher.InvokeAsync(InvalidateSchemeImpl, DispatcherPriority.Background);
        }

        private void InvalidateSchemeImpl()
        {
            _isInvalidateCalled = false;

            var pipes = new List<IPipe>();
            var valves = new List<IValve>();

            GatherControls(this, pipes, valves, _owner, new Vector());
            foreach (var pair in _children)
            {
                GatherControls(this, pipes, valves, pair.Owner, pair.Offset);
            }

            if (_scheme != null && _scheme.Equals(pipes, valves))
            {
                return;
            }
            _scheme = new FlowGraph(this, pipes, valves);
        }

        private void GatherControls(SchemeContainer schemeContainer, List<IPipe> pipes, List<IValve> valves, ISchemeContainerOwner owner, Vector offset)
        {
            foreach (var child in owner.ChildrenControls)
            {
                if (child is IPipe pipe)
                {
                    pipe.SchemeContainer = schemeContainer;
                    //pipe.Offset = offset;
                    pipes.Add(pipe);
                }

                if (child is IValve valve)
                {
                    valve.SchemeContainer = schemeContainer;
                    //valve.Offset = offset;
                    valves.Add(valve);
                }
            }
        }

        private void InvalidateSchemeFlow()
        {
            _scheme.InvalidateFlow();
        }
    }
}