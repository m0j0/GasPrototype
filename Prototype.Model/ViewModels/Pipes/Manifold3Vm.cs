using System;
using System.Collections.Generic;
using MugenMvvmToolkit.Collections;
using MugenMvvmToolkit.ViewModels;
using Prototype.Core.Interfaces.GasPanel;
using Prototype.Core.Models.GasPanel;
using Prototype.Interfaces;

namespace Prototype.ViewModels.Pipes
{
    public class Manifold3Vm : ViewModelBase, IScreenViewModel
    {
        #region Constructors

        public Manifold3Vm()
        {
            Scheme = new PipeScheme();

            //

            ValveFinal1Vm = new ValveVm("ValveFinal1") {State = ValveState.Opened};
            ValveFinal2Vm = new ValveVm("ValveFinal2") {State = ValveState.Opened};
            ValveFinal3Vm = new ValveVm("ValveFinal3") {State = ValveState.Opened};
            ValveFinal4Vm = new ValveVm("ValveFinal4") {State = ValveState.Opened};
            ValveFinal5Vm = new ValveVm("ValveFinal5") {State = ValveState.Opened};
            ValveFinal6Vm = new ValveVm("ValveFinal6") {State = ValveState.Opened};
            ValveFinal7Vm = new ValveVm("ValveFinal7") {State = ValveState.Opened};

            ValveFinal1Vertex = new ValveVertex(ValveFinal1Vm);
            ValveFinal2Vertex = new ValveVertex(ValveFinal2Vm);
            ValveFinal3Vertex = new ValveVertex(ValveFinal3Vm);
            ValveFinal4Vertex = new ValveVertex(ValveFinal4Vm);
            ValveFinal5Vertex = new ValveVertex(ValveFinal5Vm);
            ValveFinal6Vertex = new ValveVertex(ValveFinal6Vm);
            ValveFinal7Vertex = new ValveVertex(ValveFinal7Vm);

            SourceFinal1Vertex = new SourceVertex();
            SourceFinal2Vertex = new SourceVertex();
            SourceFinal3Vertex = new SourceVertex();
            SourceFinal4Vertex = new SourceVertex();
            SourceFinal5Vertex = new SourceVertex();
            SourceFinal6Vertex = new SourceVertex();
            SourceFinal7Vertex = new SourceVertex();

            ChamberDestinationVertex = new DestinationVertex();

            Connection_V1V2_Vertex = new Vertex();
            Connection_V2Dst_Vertex = new Vertex();
            Connection_V4_Vertex = new Vertex();
            Connection_V5_Vertex = new Vertex();
            Connection_V6_Vertex = new Vertex();

            Scheme.AddVertices(SourceFinal1Vertex, ValveFinal1Vertex, Connection_V1V2_Vertex, Connection_V2Dst_Vertex, ChamberDestinationVertex);
            Scheme.AddVertices(SourceFinal2Vertex, ValveFinal2Vertex, Connection_V1V2_Vertex);
            Scheme.AddVertices(SourceFinal3Vertex, ValveFinal3Vertex, Connection_V2Dst_Vertex);
            Scheme.AddVertices(SourceFinal4Vertex, ValveFinal4Vertex, Connection_V4_Vertex, Connection_V2Dst_Vertex);
            Scheme.AddVertices(SourceFinal5Vertex, ValveFinal5Vertex, Connection_V5_Vertex, Connection_V4_Vertex);
            Scheme.AddVertices(SourceFinal6Vertex, ValveFinal6Vertex, Connection_V6_Vertex, Connection_V5_Vertex);
            Scheme.AddVertices(SourceFinal7Vertex, ValveFinal7Vertex, Connection_V6_Vertex);

            //

            N2BottomPurgeValveVm = new ValveVm("N2BottomPurge") {State = ValveState.Closed};

            N2BottomPurgeValveVertex = new ValveVertex(N2BottomPurgeValveVm);
            N2BottomPurgeSourceVertex = new SourceVertex();
            N2BottomPurgeDestinationVertex = new DestinationVertex();

            Scheme.AddVertices(N2BottomPurgeSourceVertex, N2BottomPurgeValveVertex, N2BottomPurgeDestinationVertex);

            //

            UpStreamValveVm = new ValveVm("UpStream") {State = ValveState.Closed};

            UpStreamValveVertex = new ValveVertex(UpStreamValveVm);
            UpStreamSourceVertex = new SourceVertex();
            UpStreamDestinationVertex = new DestinationVertex();

            Scheme.AddVertices(UpStreamSourceVertex, UpStreamValveVertex, UpStreamDestinationVertex);

            //

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

            VacChuckShutOffLeftValveVertex = new ValveVertex(VacChuckShutOffLeftValveVm);
            VacChuckShutOffRightValveVertex = new ValveVertex(VacChuckShutOffRightValveVm);
            TurboGateValveVertex = new ValveVertex(TurboGateValveVm);
            DownStreamInvValveVertex = new ValveVertex(DownStreamInvValveVm);
            DownStreamValveVertex = new ValveVertex(DownStreamValveVm);
            VacuumValveVertex = new ValveVertex(VacuumValveVm);
            ByPassValveVertex = new ValveVertex(ByPassValveVm);
            ByPassValve2Vertex = new ValveVertex(ByPassValve2Vm);
            VacuumValve2Vertex = new ValveVertex(VacuumValve2Vm);
            NdirEndpointUpValveVertex = new ValveVertex(NdirEndpointUpValveVm);
            IsoValveVertex = new ValveVertex(IsoValveVm);
            TurboTvValveVertex = new ValveVertex(TurboTvValveVm);
            BksidePurgeValveVertex = new ValveVertex(BksidePurgeValveVm);
            NdirEndpointDownValveVertex = new ValveVertex(NdirEndpointDownValveVm);
            ThrottleValveVertex = new ValveVertex(ThrottleValveVm);
            BallastValveVertex = new ValveVertex(BallastValveVm);
            TurboValveVertex = new ValveVertex(TurboValveVm);
            DivertValveVertex = new ValveVertex(DivertValveVm);

            VfmSourceVertex = new SourceVertex();
            ChamberSourceVertex = new SourceVertex();
            TurboPumpSourceVertex = new SourceVertex();

            GasLineDestinationVertex = new DestinationVertex();
            VacChuckDestinationVertex = new DestinationVertex();
            VacChuck2DestinationVertex = new DestinationVertex();
            PumpDestinationVertex = new DestinationVertex();
            TurboPumpDestinationVertex = new DestinationVertex();
            DivertDestinationVertex = new DestinationVertex();
            BksidePurgeDestinationVertex = new DestinationVertex();
        }

        #endregion

        #region Properties

        public string DisplayName => "Manifold XZ";

        public PipeScheme Scheme { get; }

        #region Upper part

        public ValveVm ValveFinal1Vm { get; }
        public ValveVm ValveFinal2Vm { get; }
        public ValveVm ValveFinal3Vm { get; }
        public ValveVm ValveFinal4Vm { get; }
        public ValveVm ValveFinal5Vm { get; }
        public ValveVm ValveFinal6Vm { get; }
        public ValveVm ValveFinal7Vm { get; }

        public ValveVertex ValveFinal1Vertex { get; }
        public ValveVertex ValveFinal2Vertex { get; }
        public ValveVertex ValveFinal3Vertex { get; }
        public ValveVertex ValveFinal4Vertex { get; }
        public ValveVertex ValveFinal5Vertex { get; }
        public ValveVertex ValveFinal6Vertex { get; }
        public ValveVertex ValveFinal7Vertex { get; }

        public SourceVertex SourceFinal1Vertex { get; }
        public SourceVertex SourceFinal2Vertex { get; }
        public SourceVertex SourceFinal3Vertex { get; }
        public SourceVertex SourceFinal4Vertex { get; }
        public SourceVertex SourceFinal5Vertex { get; }
        public SourceVertex SourceFinal6Vertex { get; }
        public SourceVertex SourceFinal7Vertex { get; }

        public DestinationVertex ChamberDestinationVertex { get; }

        public Vertex Connection_V1V2_Vertex { get; }
        public Vertex Connection_V2Dst_Vertex { get; }
        public Vertex Connection_V4_Vertex { get; }
        public Vertex Connection_V5_Vertex { get; }
        public Vertex Connection_V6_Vertex { get; }

        #endregion

        #region N2 bottom purge

        public ValveVm N2BottomPurgeValveVm { get; }
        public ValveVertex N2BottomPurgeValveVertex { get; }
        public SourceVertex N2BottomPurgeSourceVertex { get; }
        public DestinationVertex N2BottomPurgeDestinationVertex { get; }

        #endregion

        #region UpStream

        public ValveVm UpStreamValveVm { get; }
        public ValveVertex UpStreamValveVertex { get; }
        public SourceVertex UpStreamSourceVertex { get; }
        public DestinationVertex UpStreamDestinationVertex { get; }

        #endregion

        #region Lower part

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

        public ValveVertex VacChuckShutOffLeftValveVertex { get; }
        public ValveVertex VacChuckShutOffRightValveVertex { get; }
        public ValveVertex TurboGateValveVertex { get; }
        public ValveVertex DownStreamInvValveVertex { get; }
        public ValveVertex DownStreamValveVertex { get; }
        public ValveVertex VacuumValveVertex { get; }
        public ValveVertex ByPassValveVertex { get; }
        public ValveVertex ByPassValve2Vertex { get; }
        public ValveVertex VacuumValve2Vertex { get; }
        public ValveVertex NdirEndpointUpValveVertex { get; }
        public ValveVertex IsoValveVertex { get; }
        public ValveVertex TurboTvValveVertex { get; }
        public ValveVertex BksidePurgeValveVertex { get; }
        public ValveVertex NdirEndpointDownValveVertex { get; }
        public ValveVertex ThrottleValveVertex { get; }
        public ValveVertex BallastValveVertex { get; }
        public ValveVertex TurboValveVertex { get; }
        public ValveVertex DivertValveVertex { get; }

        public SourceVertex VfmSourceVertex { get; }
        public SourceVertex ChamberSourceVertex { get; }
        public SourceVertex TurboPumpSourceVertex { get; }

        public DestinationVertex GasLineDestinationVertex { get; }
        public DestinationVertex VacChuckDestinationVertex { get; }
        public DestinationVertex VacChuck2DestinationVertex { get; }
        public DestinationVertex PumpDestinationVertex { get; }
        public DestinationVertex TurboPumpDestinationVertex { get; }
        public DestinationVertex DivertDestinationVertex { get; }
        public DestinationVertex BksidePurgeDestinationVertex { get; }

        #endregion

        #endregion

        #region Methods

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                Scheme?.Dispose();
            }

            base.OnDispose(disposing);
        }

        #endregion
    }
}