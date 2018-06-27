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
            ValveVm1 = new ValveVm("Valve1") { State = ValveState.Opened };
            ValveVm2 = new ValveVm("Valve2") { State = ValveState.Opened };
            ValveVm3 = new ValveVm("Valve3") { State = ValveState.Opened };
            ValveVm4 = new ValveVm("Valve4") { State = ValveState.Opened };
            ValveVm5 = new ValveVm("Valve5") { State = ValveState.Closed };
            ValveVm6 = new ValveVm("Valve6") { State = ValveState.Closed };
            ValveVm7 = new ValveVm("Valve7") { State = ValveState.Closed };
            ValveVm8 = new ValveVm("Valve8") { State = ValveState.Closed };
            ValveVm9 = new ValveVm("Valve9") { State = ValveState.Opened };
            ValveVm10 = new ValveVm("Valve10") { State = ValveState.Opened };
            ValveVm11 = new ValveVm("Valve11") { State = ValveState.Opened };
            ValveVm12 = new ValveVm("Valve12") { State = ValveState.Opened };
            ValveVm13 = new ValveVm("Valve13") { State = ValveState.Closed };
            ValveVm14 = new ValveVm("Valve14") { State = ValveState.Closed };
            ValveVm15 = new ValveVm("Valve15") { State = ValveState.Closed };
            ValveVm16 = new ValveVm("Valve16") { State = ValveState.Closed };
        }

        public string DisplayName => "Valve3Way example";

        public ValveVm ValveVm1 { get; }
        public ValveVm ValveVm2 { get; }
        public ValveVm ValveVm3 { get; }
        public ValveVm ValveVm4 { get; }
        public ValveVm ValveVm5 { get; }
        public ValveVm ValveVm6 { get; }
        public ValveVm ValveVm7 { get; }
        public ValveVm ValveVm8 { get; }
        public ValveVm ValveVm9 { get; }
        public ValveVm ValveVm10 { get; }
        public ValveVm ValveVm11 { get; }
        public ValveVm ValveVm12 { get; }
        public ValveVm ValveVm13 { get; }
        public ValveVm ValveVm14 { get; }
        public ValveVm ValveVm15 { get; }
        public ValveVm ValveVm16 { get; }
    }
}