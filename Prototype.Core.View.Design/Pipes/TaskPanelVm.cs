using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Metadata;
using MugenMvvmToolkit.Models;
using Prototype.Core.Controls.PipeFlowScheme;

namespace Prototype.Core.Design.Pipes
{
    internal class TaskPanelVm : NotifyPropertyChangedBase
    {
        #region Fields

        private static readonly PropertyIdentifier IsSourcePropertyIdentifier =
            new PropertyIdentifier(typeof(PipeFlowScheme), PipeFlowScheme.IsSourcePropertyName);

        private static readonly PropertyIdentifier IsDestinationPropertyIdentifier =
            new PropertyIdentifier(typeof(PipeFlowScheme), PipeFlowScheme.IsDestinationPropertyName);

        private ModelItem _modelItem;
        private IPipe _pipe;
        private ISchemeContainer _container;

        private FailType _failType;
        private PipeType _pipeType;

        #endregion

        #region Properties

        public FailType FailType
        {
            get => _failType;
            private set
            {
                if (value == _failType)
                {
                    return;
                }

                _failType = value;
                OnPropertyChanged();
            }
        }

        public PipeType PipeType
        {
            get => _pipeType;
            set
            {
                if (value == _pipeType)
                {
                    return;
                }

                _pipeType = value;
                UpdatePipeType(value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region Methods

        public void Activate(ModelItem modelItem)
        {
            _modelItem = modelItem;
            _modelItem.PropertyChanged += ModelItemOnPropertyChanged;
            _pipe = (IPipe) _modelItem.View.PlatformObject;
            _pipe.SchemeChanged += OnSchemeChanged;
            _container = _pipe.GetContainer();
            _container.SchemeChanged += OnSchemeChanged;

            SynchronizeValues();
        }

        public void Deactivate()
        {
            _modelItem.PropertyChanged -= ModelItemOnPropertyChanged;
            _modelItem = null;
            _pipe.SchemeChanged -= OnSchemeChanged;
            _pipe = null;
            _container.SchemeChanged -= OnSchemeChanged;
            _container = null;
        }

        private void OnSchemeChanged(object sender, EventArgs e)
        {
            SynchronizeValues();
        }

        private void ModelItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SynchronizeValues();
        }

        private void SynchronizeValues()
        {
            if (_pipe.Segments != null)
            {
                var failedSegment = _pipe.Segments.OfType<FailedSegment>().SingleOrDefault();
                SetFailType(_pipe.Segments.Count == 1 && failedSegment != null ? failedSegment.FailType : FailType.None);
            }

            var isSource = _modelItem.Properties[IsSourcePropertyIdentifier];
            var isDestination = _modelItem.Properties[IsDestinationPropertyIdentifier];
            if (isSource.IsSet && (bool) isSource.ComputedValue)
            {
                SetPipeType(PipeType.Source);
            }
            else if (isDestination.IsSet && (bool) isDestination.ComputedValue)
            {
                SetPipeType(PipeType.Destination);
            }
            else
            {
                SetPipeType(PipeType.Regular);
            }

            OnPropertyChanged(nameof(PipeType));
        }

        private void SetFailType(FailType failType)
        {
            _failType = failType;
            OnPropertyChanged(nameof(FailType));
        }

        private void SetPipeType(PipeType pipeType)
        {
            _pipeType = pipeType;
            OnPropertyChanged(nameof(PipeType));
        }

        private void UpdatePipeType(PipeType pipeType)
        {
            switch (pipeType)
            {
                case PipeType.Regular:
                    _modelItem.Properties[IsSourcePropertyIdentifier].ClearValue();
                    _modelItem.Properties[IsDestinationPropertyIdentifier].ClearValue();
                    break;
                case PipeType.Source:
                    _modelItem.Properties[IsSourcePropertyIdentifier].SetValue(true);
                    _modelItem.Properties[IsDestinationPropertyIdentifier].ClearValue();
                    break;
                case PipeType.Destination:
                    _modelItem.Properties[IsSourcePropertyIdentifier].ClearValue();
                    _modelItem.Properties[IsDestinationPropertyIdentifier].SetValue(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pipeType), pipeType, null);
            }
        }

        #endregion
    }
}