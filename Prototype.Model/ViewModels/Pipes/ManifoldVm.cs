﻿using MugenMvvmToolkit.ViewModels;
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
            ValveVm2 = new ValveVm("Valve2") { State = ValveState.Opened };
            ValveVm3 = new ValveVm("Valve3") { State = ValveState.Opened };
            ValveVm4 = new ValveVm("Valve4") { State = ValveState.Opened };
            ValveVm5 = new ValveVm("Valve5") { State = ValveState.Opened };
            ValveVm6 = new ValveVm("Valve6") { State = ValveState.Opened };
            ValveVm7 = new ValveVm("Valve7") { State = ValveState.Opened };
            ValveVm8 = new ValveVm("Valve8") { State = ValveState.Opened };

            ValveVertex1 = new ValveVertex(ValveVm1);
            ValveVertex2 = new ValveVertex(ValveVm2);
            ValveVertex3 = new ValveVertex(ValveVm3);
            ValveVertex4 = new ValveVertex(ValveVm4);
            ValveVertex5 = new ValveVertex(ValveVm5);
            ValveVertex6 = new ValveVertex(ValveVm6);
            ValveVertex7 = new ValveVertex(ValveVm7);
            ValveVertex8 = new ValveVertex(ValveVm8);

            SourceVertex1 = new SourceVertex();
            SourceVertex2 = new SourceVertex();
            SourceVertex3 = new SourceVertex();

            DestinationVertex1 = new DestinationVertex();
            DestinationVertex2 = new DestinationVertex();

            ConnectionVertex1 = new Vertex();
            ConnectionVertex2 = new Vertex();
            ConnectionVertex3 = new Vertex();
            ConnectionVertex4 = new Vertex();

            Scheme = new PipeScheme(
                new VertexPair(SourceVertex1, ValveVertex1),
                new VertexPair(ValveVertex1, ConnectionVertex1),
                new VertexPair(ConnectionVertex1, ValveVertex7),
                new VertexPair(ValveVertex7, DestinationVertex1),
                new VertexPair(SourceVertex2, ConnectionVertex2),
                new VertexPair(ConnectionVertex2, ValveVertex2),
                new VertexPair(ConnectionVertex2, ValveVertex3),
                new VertexPair(ValveVertex2, ConnectionVertex1),
                new VertexPair(ValveVertex3, ConnectionVertex1),
                new VertexPair(ConnectionVertex2, ConnectionVertex3),
                new VertexPair(ConnectionVertex3, ValveVertex4),
                new VertexPair(ConnectionVertex3, ValveVertex5),
                new VertexPair(ValveVertex4, ConnectionVertex4),
                new VertexPair(ValveVertex5, ConnectionVertex4),
                new VertexPair(ConnectionVertex4, ValveVertex8),
                new VertexPair(ValveVertex8, DestinationVertex2),
                new VertexPair(SourceVertex3, ValveVertex6),
                new VertexPair(ValveVertex6, ConnectionVertex4)
            );
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

        public PipeScheme Scheme { get; }

        public ValveVertex ValveVertex1 { get; }
        public ValveVertex ValveVertex2 { get; }
        public ValveVertex ValveVertex3 { get; }
        public ValveVertex ValveVertex4 { get; }
        public ValveVertex ValveVertex5 { get; }
        public ValveVertex ValveVertex6 { get; }
        public ValveVertex ValveVertex7 { get; }
        public ValveVertex ValveVertex8 { get; }

        public SourceVertex SourceVertex1 { get; }
        public SourceVertex SourceVertex2 { get; }
        public SourceVertex SourceVertex3 { get; }

        public DestinationVertex DestinationVertex1 { get; }
        public DestinationVertex DestinationVertex2 { get; }

        public Vertex ConnectionVertex1 { get; }
        public Vertex ConnectionVertex2 { get; }
        public Vertex ConnectionVertex3 { get; }
        public Vertex ConnectionVertex4 { get; }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                Scheme?.Dispose();
            }

            base.OnDispose(disposing);
        }
    }
}
