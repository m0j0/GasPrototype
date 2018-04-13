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
            ValveVm1 = new ValveVm("Valve1") { State = ValveState.Opened };
            ValveVm2 = new ValveVm("Valve2") { State = ValveState.Opened };
            ValveVm3 = new ValveVm("Valve3") { State = ValveState.Opened };
            ValveVm4 = new ValveVm("Valve4") { State = ValveState.Opened };
            ValveVm5 = new ValveVm("Valve5") { State = ValveState.Opened };
            ValveVm6 = new ValveVm("Valve6") { State = ValveState.Opened };
            ValveVm7 = new ValveVm("Valve7") { State = ValveState.Opened };

            ValveVertex1 = new ValveVertex(ValveVm1);
            ValveVertex2 = new ValveVertex(ValveVm2);
            ValveVertex3 = new ValveVertex(ValveVm3);
            ValveVertex4 = new ValveVertex(ValveVm4);
            ValveVertex5 = new ValveVertex(ValveVm5);
            ValveVertex6 = new ValveVertex(ValveVm6);
            ValveVertex7 = new ValveVertex(ValveVm7);

            SourceVertex1 = new SourceVertex();
            SourceVertex2 = new SourceVertex();
            SourceVertex3 = new SourceVertex();
            SourceVertex4 = new SourceVertex();
            SourceVertex5 = new SourceVertex();
            SourceVertex6 = new SourceVertex();
            SourceVertex7 = new SourceVertex();

            DestinationVertex1 = new DestinationVertex();

            ConnectionVertex1 = new Vertex();
            ConnectionVertex2 = new Vertex();
            ConnectionVertex3 = new Vertex();
            ConnectionVertex4 = new Vertex();
            ConnectionVertex5 = new Vertex();

            Scheme = new PipeScheme(
                new VerticesPair(SourceVertex1, ValveVertex1),
                new VerticesPair(SourceVertex2, ValveVertex2),
                new VerticesPair(SourceVertex3, ValveVertex3),
                new VerticesPair(SourceVertex4, ValveVertex4),
                new VerticesPair(SourceVertex5, ValveVertex5),
                new VerticesPair(SourceVertex6, ValveVertex6),
                new VerticesPair(SourceVertex7, ValveVertex7),
                new VerticesPair(ValveVertex1, ConnectionVertex1),
                new VerticesPair(ValveVertex2, ConnectionVertex1),
                new VerticesPair(ConnectionVertex1, ConnectionVertex2),
                new VerticesPair(ValveVertex3, ConnectionVertex2),
                new VerticesPair(ConnectionVertex2, DestinationVertex1),
                new VerticesPair(ConnectionVertex2, ConnectionVertex3),
                new VerticesPair(ValveVertex4, ConnectionVertex3),
                new VerticesPair(ConnectionVertex3, ConnectionVertex4),
                new VerticesPair(ValveVertex5, ConnectionVertex4),
                new VerticesPair(ConnectionVertex4, ConnectionVertex5),
                new VerticesPair(ValveVertex6, ConnectionVertex5),
                new VerticesPair(ValveVertex7, ConnectionVertex5)
            );

            Vertices = new SynchronizedNotifiableCollection<Tuple<string, IVertex>>
            {
                new Tuple<string, IVertex>(nameof(SourceVertex1), SourceVertex1),
                new Tuple<string, IVertex>(nameof(SourceVertex2), SourceVertex2),
                new Tuple<string, IVertex>(nameof(SourceVertex3), SourceVertex3),
                new Tuple<string, IVertex>(nameof(SourceVertex4), SourceVertex4),
                new Tuple<string, IVertex>(nameof(SourceVertex5), SourceVertex5),
                new Tuple<string, IVertex>(nameof(SourceVertex6), SourceVertex6),
                new Tuple<string, IVertex>(nameof(SourceVertex7), SourceVertex7),
                new Tuple<string, IVertex>(nameof(ValveVertex1), ValveVertex1),
                new Tuple<string, IVertex>(nameof(ValveVertex2), ValveVertex2),
                new Tuple<string, IVertex>(nameof(ValveVertex3), ValveVertex3),
                new Tuple<string, IVertex>(nameof(ValveVertex4), ValveVertex4),
                new Tuple<string, IVertex>(nameof(ValveVertex5), ValveVertex5),
                new Tuple<string, IVertex>(nameof(ValveVertex6), ValveVertex6),
                new Tuple<string, IVertex>(nameof(ValveVertex7), ValveVertex7),
                new Tuple<string, IVertex>(nameof(DestinationVertex1), DestinationVertex1),
            };
        }

        #endregion

        #region Properties

        public string DisplayName => "Manifold 3";

        public ValveVm ValveVm1 { get; }
        public ValveVm ValveVm2 { get; }
        public ValveVm ValveVm3 { get; }
        public ValveVm ValveVm4 { get; }
        public ValveVm ValveVm5 { get; }
        public ValveVm ValveVm6 { get; }
        public ValveVm ValveVm7 { get; }

        public PipeScheme Scheme { get; }

        public ValveVertex ValveVertex1 { get; }
        public ValveVertex ValveVertex2 { get; }
        public ValveVertex ValveVertex3 { get; }
        public ValveVertex ValveVertex4 { get; }
        public ValveVertex ValveVertex5 { get; }
        public ValveVertex ValveVertex6 { get; }
        public ValveVertex ValveVertex7 { get; }

        public SourceVertex SourceVertex1 { get; }
        public SourceVertex SourceVertex2 { get; }
        public SourceVertex SourceVertex3 { get; }
        public SourceVertex SourceVertex4 { get; }
        public SourceVertex SourceVertex5 { get; }
        public SourceVertex SourceVertex6 { get; }
        public SourceVertex SourceVertex7 { get; }

        public DestinationVertex DestinationVertex1 { get; }

        public Vertex ConnectionVertex1 { get; }
        public Vertex ConnectionVertex2 { get; }
        public Vertex ConnectionVertex3 { get; }
        public Vertex ConnectionVertex4 { get; }
        public Vertex ConnectionVertex5 { get; }

        public SynchronizedNotifiableCollection<Tuple<string, IVertex>> Vertices { get; }

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
