using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MugenMvvmToolkit;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    public sealed class PipeFlowScheme
    {
        #region Attached properties

        internal const string IsSourcePropertyName = "IsSource";
        internal const string IsDestinationPropertyName = "IsDestination";

        public static readonly DependencyProperty IsSourceProperty = DependencyProperty.RegisterAttached(
            IsSourcePropertyName, typeof(bool), typeof(PipeFlowScheme), new PropertyMetadata(false, PropertyChangedCallback));

        public static void SetIsSource(DependencyObject element, bool value)
        {
            element.SetValue(IsSourceProperty, value);
        }

        public static bool GetIsSource(DependencyObject element)
        {
            return (bool)element.GetValue(IsSourceProperty);
        }

        public static readonly DependencyProperty IsDestinationProperty = DependencyProperty.RegisterAttached(
            IsDestinationPropertyName, typeof(bool), typeof(PipeFlowScheme), new PropertyMetadata(false, PropertyChangedCallback));

        public static void SetIsDestination(DependencyObject element, bool value)
        {
            element.SetValue(IsDestinationProperty, value);
        }

        public static bool GetIsDestination(DependencyObject element)
        {
            return (bool)element.GetValue(IsDestinationProperty);
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (GetIsDestination(d) && GetIsSource(d))
            {
                throw new Exception("!!!");
            }
        }

        #endregion

        private readonly IContainer _container;
        private readonly List<IPipe> _pipes;
        private readonly List<IValve> _valves;
        private ProcessPipe[] _processPipes;

        public PipeFlowScheme(IContainer container)
        {
            _container = container;
            _container.SchemeChanged += OnSchemeChanged;
            _pipes = new List<IPipe>();
            _valves = new List<IValve>();
        }

        public void Add(IFlowControl control)
        {
            AddInternal(control);

            InvalidateScheme();
        }

        public void Add(IReadOnlyCollection<IFlowControl> controls)
        {
            Should.NotBeNull(controls, nameof(controls));

            if (controls.Count == 0)
            {
                return;
            }

            foreach (var flowControl in controls)
            {
                AddInternal(flowControl);
            }

            InvalidateScheme();
        }

        public bool Contains(IFlowControl flowControl)
        {
            return _pipes.Contains(flowControl) || _valves.Contains(flowControl);
        }

        public void Remove(IFlowControl control)
        {
            Should.NotBeNull(control, nameof(control));
            control.SchemeChanged -= OnSchemeChanged;

            switch (control)
            {
                case IPipe pipe:
                    _pipes.Remove(pipe);
                    break;

                case IValve valve:
                    valve.StateChanged -= OnValveStateChanged;
                    _valves.Remove(valve);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        private void InvalidateScheme()
        {
            _processPipes = PipeDrawing.SplitPipeToSegments(_container, _pipes, _valves);
            InvalidateFlow();
        }

        private void InvalidateFlow()
        {
            var connectors = _processPipes.SelectMany(pipe => pipe.Connectors).OfType<CornerConnector>().Distinct().ToArray();

            foreach (var edge in _pipes)
            {
                edge.HasFlow = false;
            }

            var destinationConnectors = connectors.Where(c => c.IsDestination).ToArray();
            foreach (var connector in connectors)
            {
                if (!connector.IsSource)
                {
                    continue;
                }

                var algo = new DepthFirstDirectedPaths(connector);

                foreach (var destinationVertex in destinationConnectors)
                {
                    var paths = algo.PathsTo(destinationVertex);
                    if (paths == null || paths.Count == 0)
                    {
                        continue;
                    }

                    foreach (var path in paths)
                    {
                        for (var i = 0; i < path.Count - 1; i++)
                        {
                            var edge = _processPipes.Single(pipe => pipe.Connectors.Contains(path[i]) && pipe.Connectors.Contains(path[i + 1]));
                            edge.Pipe.HasFlow = true;
                        }
                    }
                }
            }
        }

        private void AddInternal(IFlowControl control)
        {
            Should.NotBeNull(control, nameof(control));
            control.SchemeChanged += OnSchemeChanged;
            switch (control)
            {
                case IPipe pipe:
                    _pipes.Add(pipe);
                    break;

                case IValve valve:
                    valve.StateChanged += OnValveStateChanged;
                    _valves.Add(valve);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        private void OnValveStateChanged(object sender, EventArgs e)
        {
            InvalidateFlow();
        }

        private void OnSchemeChanged(object sender, EventArgs e)
        {
            InvalidateScheme();
        }
    }
}