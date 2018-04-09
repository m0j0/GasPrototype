using MugenMvvmToolkit.Models;
using Prototype.Core.Interfaces;

namespace Prototype.Core.Models
{
    public class WaferVm : NotifyPropertyChangedBase, IWaferVm
    {
        private readonly string _label;
        private readonly WaferStatus _status;
        private readonly WaferWorkingMode _workingMode;

        public WaferVm(string label, WaferStatus status, WaferWorkingMode workingMode)
        {
            _label = label;
            _status = status;
            _workingMode = workingMode;
        }

        public string Label
        {
            get { return _label; }
        }

        public string DetailedLabel
        {
            get { return _label + " detailed"; }
        }

        public WaferStatus Status
        {
            get { return _status; }
        }

        public WaferWorkingMode WorkingMode
        {
            get { return _workingMode; }
        }
    }
}
