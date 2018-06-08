using MugenMvvmToolkit.ViewModels;
using Prototype.Core.Models;
using Prototype.Core.Models.GasPanel;
using Prototype.Interfaces;

namespace Prototype.ViewModels.Pipes
{
    public class ManifoldVm : ViewModelBase, IScreenViewModel
    {
        public ManifoldVm()
        {
            ValveVm1 = new ValveVm("Valve1") {State = ValveState.Opened};
            ValveVm2 = new ValveVm("Valve2") {State = ValveState.Opened};
            ValveVm3 = new ValveVm("Valve3") {State = ValveState.Opened};
            ValveVm4 = new ValveVm("Valve4") {State = ValveState.Opened};
            ValveVm5 = new ValveVm("Valve5") {State = ValveState.Opened};
            ValveVm6 = new ValveVm("Valve6") {State = ValveState.Opened};
            ValveVm7 = new ValveVm("Valve7") {State = ValveState.Opened};
            ValveVm8 = new ValveVm("Valve8") {State = ValveState.Opened};
        }

        public string DisplayName => "Manifold";

        public ValveVm ValveVm1 { get; }
        public ValveVm ValveVm2 { get; }
        public ValveVm ValveVm3 { get; }
        public ValveVm ValveVm4 { get; }
        public ValveVm ValveVm5 { get; }
        public ValveVm ValveVm6 { get; }
        public ValveVm ValveVm7 { get; }
        public ValveVm ValveVm8 { get; }
    }
}