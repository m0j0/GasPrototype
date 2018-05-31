using MugenMvvmToolkit.ViewModels;
using Prototype.Core.Models.GasPanel;
using Prototype.Interfaces;

namespace Prototype.ViewModels.Pipes
{
    public class Manifold2Vm : ViewModelBase, IScreenViewModel
    {
        #region Constructors

        public Manifold2Vm()
        {
            ValveVm1 = new ValveVm("Valve1") {State = ValveState.Opened};
            ValveVm2 = new ValveVm("Valve2") {State = ValveState.Opened};
        }

        #endregion

        #region Properties

        public string DisplayName
        {
            get { return "Manifold 2"; }
        }

        public ValveVm ValveVm1 { get; }
        public ValveVm ValveVm2 { get; }

        #endregion
    }
}