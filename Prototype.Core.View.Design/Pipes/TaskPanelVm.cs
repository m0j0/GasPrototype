using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Metadata;
using MugenMvvmToolkit;
using MugenMvvmToolkit.Collections;
using MugenMvvmToolkit.Models;
using Prototype.Core.Controls;
using Prototype.Core.Interfaces.GasPanel;
using Prototype.Core.MarkupExtensions;
using Prototype.Core.Models.GasPanel;

namespace Prototype.Core.Design.Pipes
{
    public class TaskPanelVm : NotifyPropertyChangedBase
    {
        #region Fields

        private double _length;
        private ModelItem _modelItem;
        private Orientation _orientation;
        private VertexProjection _startVertex;
        private VertexProjection _endVertex;

        private Dictionary<IVertex, VertexProjection> _allVertices;

        #endregion

        #region Constructors

        public TaskPanelVm()
        {
            StartVertices = new SynchronizedNotifiableCollection<VertexProjection>();
            EndVertices = new SynchronizedNotifiableCollection<VertexProjection>();

            DuplicateControlCommand = new RelayCommand(DuplicateControl);
        }

        #endregion

        #region Properties

        public double Length
        {
            get => _length;
            set
            {
                if (value.Equals(_length))
                {
                    return;
                }

                _length = value;
                UpdateControlLength(value);
                OnPropertyChanged();
            }
        }

        public Orientation Orientation
        {
            get => _orientation;
            private set
            {
                if (value == _orientation)
                {
                    return;
                }

                _orientation = value;
                OnPropertyChanged();
            }
        }

        public VertexProjection StartVertex
        {
            get => _startVertex;
            set
            {
                if (Equals(value, _startVertex))
                {
                    return;
                }

                _startVertex = value;
                SetControlVertice(value, PipeSchemeEx.StartVertexPropertyName);
                OnPropertyChanged();


                if (EndVertex != null)
                {
                    return;
                }

                if (value == null)
                {
                    EndVertices.Clear();
                }
                else
                {
                    EndVertices.Update(SelectAdjacentVerticesProjections(value));
                }
            }
        }

        public SynchronizedNotifiableCollection<VertexProjection> StartVertices { get; }

        public VertexProjection EndVertex
        {
            get => _endVertex;
            set
            {
                if (Equals(value, _endVertex))
                {
                    return;
                }

                _endVertex = value;
                SetControlVertice(value, PipeSchemeEx.EndVertexPropertyName);
                OnPropertyChanged();
            }
        }

        public SynchronizedNotifiableCollection<VertexProjection> EndVertices { get; }

        #endregion

        #region Commands

        public ICommand DuplicateControlCommand { get; }

        private void DuplicateControl()
        {
            using (var scope = _modelItem.BeginEdit())
            {
                var pipe = ModelFactory.CreateItem(_modelItem.Context, typeof(Pipe));
                //pipe.Properties[nameof(Pipe.Width)].SetValue("textBox");
                ModelParent.Parent(_modelItem.Context, _modelItem.Parent, pipe);

                scope.Complete();
            }
        }

        #endregion

        #region Methods

        public void Activate(ModelItem modelItem)
        {
            _modelItem = modelItem;
            _modelItem.PropertyChanged += ModelItemOnPropertyChanged;

            Initialize();
        }

        public void Deactivate()
        {
            _modelItem.PropertyChanged -= ModelItemOnPropertyChanged;
            _modelItem = null;
        }

        private void ModelItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName) ||
                e.PropertyName == nameof(Pipe.Orientation) ||
                e.PropertyName == nameof(Pipe.Width) ||
                e.PropertyName == nameof(Pipe.Height))
            {
                SynchronizeValues();
            }
        }

        private void Initialize()
        {
            var vertices = new List<VertexProjection>();

            var dataContext = _modelItem.Properties[nameof(FrameworkElement.DataContext)].ComputedValue;
            if (dataContext == null)
            {
                _allVertices = new Dictionary<IVertex, VertexProjection>();
                SynchronizeValues();
                return;
            }

            var properties = dataContext.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var pi in properties)
            {
                if (typeof(IVertex).IsAssignableFrom(pi.PropertyType) && pi.CanRead)
                {
                    var vertex = (IVertex)pi.GetValue(dataContext);
                    if (vertex.Owner == null)
                    {
                        continue;
                    }

                    vertices.Add(new VertexProjection(vertex, pi.Name));
                }
            }

            _allVertices = vertices.OrderBy(vertex => vertex.Name).ToDictionary(projection => projection.Vertex, projection => projection);

            SynchronizeValues();
        }

        private void SynchronizeValues()
        {
            SynchronizeVertices();

            var pipe = (Pipe)_modelItem.View.PlatformObject;

            SetDisplayLength(pipe.Orientation == Orientation.Horizontal ? pipe.Width : pipe.Height);
            Orientation = pipe.Orientation;
        }

        private void SynchronizeVertices()
        {
            var startVertexProperty = _modelItem.Properties[new PropertyIdentifier(typeof(PipeSchemeEx), PipeSchemeEx.StartVertexPropertyName)];
            var startVertex = startVertexProperty.ComputedValue as IVertex;

            var endVertexProperty = _modelItem.Properties[new PropertyIdentifier(typeof(PipeSchemeEx), PipeSchemeEx.EndVertexPropertyName)];
            var endVertex = endVertexProperty.ComputedValue as IVertex;

            if (startVertex != null)
            {
                _allVertices.TryGetValue(startVertex, out var startVertexProjection);
                _startVertex = startVertexProjection;
                OnPropertyChanged(nameof(StartVertex));
            }
            StartVertices.Update(_allVertices.Values);

            if (endVertex != null)
            {
                _allVertices.TryGetValue(endVertex, out var endVertexProjection);
                _endVertex = endVertexProjection;
                OnPropertyChanged(nameof(EndVertex));
            }
            EndVertices.Update(_allVertices.Values);
        }

        private void SetDisplayLength(double newLength)
        {
            _length = newLength;
            OnPropertyChanged(nameof(Length));
        }

        private void UpdateControlLength(double newLength)
        {
            var pipe = _modelItem.View.PlatformObject as Pipe;
            if (pipe == null)
            {
                return;
            }

            if (pipe.Orientation == Orientation.Vertical)
            {
                _modelItem.Properties[nameof(Pipe.Height)].SetValue(newLength);
            }
            else
            {
                _modelItem.Properties[nameof(Pipe.Width)].SetValue(newLength);
            }
        }

        private void SetControlVertice(VertexProjection vertex, string property)
        {
            var propertyIdentifier = new PropertyIdentifier(typeof(PipeSchemeEx), property);
            if (vertex == null)
            {
                _modelItem.Properties[propertyIdentifier].ClearValue();
            }
            else
            {
                _modelItem.Properties[propertyIdentifier].SetValue(new Binding(vertex.PropertyName));
            }

        }

        private IEnumerable<VertexProjection> SelectAdjacentVerticesProjections(VertexProjection value)
        {
            return value.Vertex.GetAllAdjacentVertices().Select(vertex => _allVertices[vertex]);
        }

        #endregion
    }
}