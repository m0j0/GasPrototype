using MugenMvvmToolkit.Models;
using Prototype.Core.Interfaces.GasPanel;

namespace Prototype.Core.Models.GasPanel
{
    internal class PipeVm : NotifyPropertyChangedBase, IPipeVm
    {
        #region Fields

        private bool _hasFlow;
        private SubstanceType _substanceType;

        #endregion

        #region Properties

        public bool HasFlow
        {
            get => _hasFlow;
            set
            {
                if (_hasFlow == value)
                {
                    return;
                }
                _hasFlow = value;
                OnPropertyChanged();
            }
        }

        public SubstanceType SubstanceType
        {
            get => _substanceType;
            set
            {
                if (_substanceType == value)
                {
                    return;
                }
                _substanceType = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}