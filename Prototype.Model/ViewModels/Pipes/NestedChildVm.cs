using MugenMvvmToolkit.ViewModels;
using Prototype.Core.Models.GasPanel;

namespace Prototype.ViewModels.Pipes
{
    public class NestedChildVm : ViewModelBase
    {
        public NestedChildVm()
        {
            ValveVm = new ValveVm("Valve");
        }

        public ValveVm ValveVm { get; }
    }
}
