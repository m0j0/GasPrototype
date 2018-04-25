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

            ValveFinal1Vm = new ValveVm("ValveFinal1") { State = ValveState.Opened };
            ValveFinal2Vm = new ValveVm("ValveFinal2") { State = ValveState.Opened };
            ValveFinal3Vm = new ValveVm("ValveFinal3") { State = ValveState.Opened };
            ValveFinal4Vm = new ValveVm("ValveFinal4") { State = ValveState.Opened };
            ValveFinal5Vm = new ValveVm("ValveFinal5") { State = ValveState.Opened };
            ValveFinal6Vm = new ValveVm("ValveFinal6") { State = ValveState.Opened };
            ValveFinal7Vm = new ValveVm("ValveFinal7") { State = ValveState.Opened };

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

        public PipeScheme Scheme { get; }

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
