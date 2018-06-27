using MugenMvvmToolkit.ViewModels;
using Prototype.Core.Models;
using Prototype.Core.Models.GasPanel;
using Prototype.Interfaces;

namespace Prototype.ViewModels.Pipes
{
    public class Manifold3Vm : ViewModelBase, IScreenViewModel
    {
        #region Constructors

        public Manifold3Vm()
        {
            ValveFinal1Vm = new ValveVm("ValveFinal1") {State = ValveState.Open};
            ValveFinal2Vm = new ValveVm("ValveFinal2") {State = ValveState.Open};
            ValveFinal3Vm = new ValveVm("ValveFinal3") {State = ValveState.Open};
            ValveFinal4Vm = new ValveVm("ValveFinal4") {State = ValveState.Open};
            ValveFinal5Vm = new ValveVm("ValveFinal5") {State = ValveState.Open};
            ValveFinal6Vm = new ValveVm("ValveFinal6") {State = ValveState.Open};
            ValveFinal7Vm = new ValveVm("ValveFinal7") {State = ValveState.Open};
            
            N2BottomPurgeValveVm = new ValveVm("N2BottomPurge") {State = ValveState.Closed};

            UpStreamValveVm = new ValveVm("UpStream") {State = ValveState.Closed};
            
            VacChuckShutOffLeftValveVm = new ValveVm("VacChuckShutOffLeftValveVm") {State = ValveState.Closed};
            VacChuckShutOffRightValveVm = new ValveVm("VacChuckShutOffRightValveVm") {State = ValveState.Closed};
            TurboGateValveVm = new ValveVm("TurboGateValveVm") {State = ValveState.Closed};
            DownStreamInvValveVm = new ValveVm("DownStreamInvValveVm") {State = ValveState.Closed};
            DownStreamValveVm = new ValveVm("DownStreamValveVm") {State = ValveState.Closed};
            VacuumValveVm = new ValveVm("VacuumValveVm") {State = ValveState.Closed};
            ByPassValveVm = new ValveVm("ByPassValveVm") {State = ValveState.Closed};
            ByPassValve2Vm = new ValveVm("ByPassValve2Vm") {State = ValveState.Closed};
            VacuumValve2Vm = new ValveVm("VacuumValve2Vm") {State = ValveState.Closed};
            NdirEndpointUpValveVm = new ValveVm("NdirEndpointUpValveVm") {State = ValveState.Closed};
            IsoValveVm = new ValveVm("IsoValveVm") {State = ValveState.Closed};
            TurboTvValveVm = new ValveVm("TurboTvValveVm") {State = ValveState.Closed};
            BksidePurgeValveVm = new ValveVm("BksidePurgeValveVm") {State = ValveState.Closed};
            NdirEndpointDownValveVm = new ValveVm("NdirEndpointDownValveVm") {State = ValveState.Closed};
            ThrottleValveVm = new ValveVm("ThrottleValveVm") {State = ValveState.Closed};
            BallastValveVm = new ValveVm("BallastValveVm") {State = ValveState.Closed};
            TurboValveVm = new ValveVm("TurboValveVm") {State = ValveState.Closed};
            DivertValveVm = new ValveVm("DivertValveVm") {State = ValveState.Closed};
        }

        #endregion

        #region Properties

        public string DisplayName => "Manifold XZ";

        public ValveVm ValveFinal1Vm { get; }
        public ValveVm ValveFinal2Vm { get; }
        public ValveVm ValveFinal3Vm { get; }
        public ValveVm ValveFinal4Vm { get; }
        public ValveVm ValveFinal5Vm { get; }
        public ValveVm ValveFinal6Vm { get; }
        public ValveVm ValveFinal7Vm { get; }

        public ValveVm N2BottomPurgeValveVm { get; }

        public ValveVm UpStreamValveVm { get; }

        public ValveVm VacChuckShutOffLeftValveVm { get; }
        public ValveVm VacChuckShutOffRightValveVm { get; }
        public ValveVm TurboGateValveVm { get; }
        public ValveVm DownStreamInvValveVm { get; } // TODO 3way valve
        public ValveVm DownStreamValveVm { get; }
        public ValveVm VacuumValveVm { get; }
        public ValveVm ByPassValveVm { get; }
        public ValveVm ByPassValve2Vm { get; }
        public ValveVm VacuumValve2Vm { get; }
        public ValveVm NdirEndpointUpValveVm { get; }
        public ValveVm IsoValveVm { get; }
        public ValveVm TurboTvValveVm { get; }
        public ValveVm BksidePurgeValveVm { get; }
        public ValveVm NdirEndpointDownValveVm { get; }
        public ValveVm ThrottleValveVm { get; }
        public ValveVm BallastValveVm { get; }
        public ValveVm TurboValveVm { get; }
        public ValveVm DivertValveVm { get; }

        #endregion
    }
}