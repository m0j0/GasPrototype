using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Windows.Design.Model;
using MugenMvvmToolkit.Models;
using Prototype.Core.Controls;

namespace Prototype.Core.Design.Pipes
{
    public class TaskPanelVm : NotifyPropertyChangedBase
    {
        private double _length;
        private ModelItem _modelItem;
        private Orientation _orientation;

        public TaskPanelVm()
        {
            DuplicateControlCommand = new RelayCommand(DuplicateControl);
        }

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

        public ICommand DuplicateControlCommand { get; }

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

            var pipe = _modelItem.View.PlatformObject as Pipe;
            if (pipe == null)
            {
                return;
            }

            UpdateDisplayLength(pipe.Orientation == Orientation.Horizontal ? pipe.Width : pipe.Height);
            Orientation = pipe.Orientation;
        }

        private void UpdateDisplayLength(double newLength)
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
    }
}