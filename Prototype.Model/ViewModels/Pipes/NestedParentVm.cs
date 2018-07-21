using MugenMvvmToolkit.ViewModels;
using Prototype.Core.Models;
using Prototype.Core.Models.GasPanel;
using Prototype.Interfaces;

namespace Prototype.ViewModels.Pipes
{
    public class NestedParentVm : ViewModelBase, IScreenViewModel
    {
        public NestedParentVm()
        {
            SourceValveVm = new ValveVm("SourceValve") {State = ValveState.Open};
            DestinationValveVm = new ValveVm("SourceValve") {State = ValveState.Open};

            ChildVm1 = GetViewModel<NestedChildVm>();
            ChildVm1.ValveVm.OpenCommand.Execute(null);
            ChildVm2 = GetViewModel<NestedChildVm>();
            ChildVm2.ValveVm.CloseCommand.Execute(null);
            ChildVm3 = GetViewModel<NestedChildVm>();
            ChildVm3.ValveVm.OpenCommand.Execute(null);
        }

        public string DisplayName => "Nested schemes";

        public ValveVm SourceValveVm { get; }

        public ValveVm DestinationValveVm { get; }

        public NestedChildVm ChildVm1 { get; }

        public NestedChildVm ChildVm2 { get; }

        public NestedChildVm ChildVm3 { get; }
    }
}