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
        private IVertex _startVertex;
        private IVertex _endVertex;

        #endregion

        #region Constructors

        public TaskPanelVm()
        {
            StartVertices = new SynchronizedNotifiableCollection<IVertex>();
            EndVertices = new SynchronizedNotifiableCollection<IVertex>();

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

        public IVertex StartVertex
        {
            get => _startVertex;
            set
            {
                if (Equals(value, _startVertex))
                {
                    return;
                }

                _startVertex = value;
                EndVertices.Update(value.GetAllAdjacentVertices());
                SetControlVertice(value, "StartVertex");
                OnPropertyChanged();
            }
        }

        public SynchronizedNotifiableCollection<IVertex> StartVertices { get; }

        public IVertex EndVertex
        {
            get => _endVertex;
            set
            {
                if (Equals(value, _endVertex))
                {
                    return;
                }

                _endVertex = value;
                SetControlVertice(value, "EndVertex");
                OnPropertyChanged();
            }
        }

        public SynchronizedNotifiableCollection<IVertex> EndVertices { get; }

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

            SynchronizeValues();
        }

        public void Deactivate()
        {
            _modelItem.PropertyChanged -= ModelItemOnPropertyChanged;
            _modelItem = null;
        }

        private void ModelItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Pipe.Orientation) ||
                e.PropertyName == nameof(Pipe.Width) ||
                e.PropertyName == nameof(Pipe.Height))
            {
                SynchronizeValues();
            }
        }

        private void SynchronizeValues()
        {
            if (_modelItem == null)
            {
                return;
            }

            var dataContext = _modelItem.Properties[nameof(FrameworkElement.DataContext)].ComputedValue;
            var properties = dataContext.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var schemes = new List<PipeScheme>();
            foreach (var pi in properties)
            {
                if (typeof(PipeScheme).IsAssignableFrom(pi.PropertyType) && pi.CanRead)
                {
                    var value = (PipeScheme)pi.GetValue(dataContext);
                    schemes.Add(value);
                }

                if (typeof(IVertex).IsAssignableFrom(pi.PropertyType) && pi.CanRead)
                {
                    var value = (IVertex)pi.GetValue(dataContext);
                    if (string.IsNullOrEmpty(value.Name))
                    {
                        value.Name = pi.Name;
                    }
                }
            }

            var pipeScheme = schemes.FirstOrDefault();
            if (pipeScheme != null)
            {
                StartVertices.Update(pipeScheme.Vertices);
                EndVertices.Update(pipeScheme.Vertices);
            }

            var pipe = _modelItem.View.PlatformObject as Pipe;
            if (pipe == null)
            {
                return;
            }

            SetDisplayLength(pipe.Orientation == Orientation.Horizontal ? pipe.Width : pipe.Height);
            Orientation = pipe.Orientation;
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

        private void SetControlVertice(IVertex vertex, string property)
        {
            var propertyIdentifier = new Microsoft.Windows.Design.Metadata.PropertyIdentifier(typeof(PipeSchemeEx), property);
            _modelItem.Properties[propertyIdentifier].SetValue(new Binding(vertex.Name /* TODO */));

        }

        #endregion
    }
}