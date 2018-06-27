using MugenMvvmToolkit.ViewModels;
using Prototype.Core.Models;
using Prototype.Core.Models.GasPanel;
using Prototype.Interfaces;

namespace Prototype.ViewModels.Pipes
{
    public class Valve3WayExampleVm : ViewModelBase, IScreenViewModel
    {
        public Valve3WayExampleVm()
        {
            OpenValveVm = new ValveVm("OpenValve") { State = ValveState.Opened };
            ClosedValveVm = new ValveVm("ClosedValve") { State = ValveState.Closed };
        }

        public string DisplayName => "Valve3Way example";

        public ValveVm OpenValveVm { get; }
        public ValveVm ClosedValveVm { get; }
    }
}