using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Metadata;
using MugenMvvmToolkit.Models;
using Prototype.Core.Controls;
using Prototype.Core.Controls.PipeFlowScheme;

namespace Prototype.Core.Design.Pipes
{
    internal class TaskPanelVm : NotifyPropertyChangedBase
    {
        #region Fields
        
        private ModelItem _modelItem;
        private IPipe _pipe;

        private FailType _failType;

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

        #endregion

        #region Methods

        public void Activate(ModelItem modelItem)
        {
            _modelItem = modelItem;
            _modelItem.PropertyChanged += ModelItemOnPropertyChanged;
            _pipe = (IPipe) _modelItem.View.PlatformObject;
            _pipe.SchemeChanged += OnSchemeChanged;

            SynchronizeValues();
        }

        public void Deactivate()
        {
            _modelItem.PropertyChanged -= ModelItemOnPropertyChanged;
            _modelItem = null;
            _pipe.SchemeChanged -= OnSchemeChanged;
            _pipe = null;
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
            SynchronizeFailType();
        }

        private void SynchronizeFailType()
        {
            if (_pipe.Segments == null)
            {
                return;
            }

            var failedSegment = _pipe.Segments.OfType<FailedSegment>().SingleOrDefault();
            SetFailType(_pipe.Segments.Count == 1 && failedSegment != null ? failedSegment.FailType : FailType.None);
        }

        private void SetFailType(FailType failType)
        {
            _failType = failType;
            OnPropertyChanged(nameof(FailType));
        }

        #endregion
    }
}