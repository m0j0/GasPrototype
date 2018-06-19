using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MugenMvvmToolkit.ViewModels;
using Prototype.Core.Models;
using Prototype.Core.Models.GasPanel;
using Prototype.Interfaces;

namespace Prototype.ViewModels.Pipes
{
    public class Manifold4Vm : ViewModelBase, IScreenViewModel
    {
        public Manifold4Vm()
        {
            ValveVm1 = new ValveVm("Valve1") { State = ValveState.Opened };
            ValveVm2 = new ValveVm("Valve2") { State = ValveState.Closed };
            ValveVm3 = new ValveVm("Valve3") { State = ValveState.Closed };
        }

        public string DisplayName => "Manifold 4";

        public ValveVm ValveVm1 { get; }
        public ValveVm ValveVm2 { get; }
        public ValveVm ValveVm3 { get; }
    }
}
