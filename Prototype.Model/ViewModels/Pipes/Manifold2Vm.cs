using MugenMvvmToolkit.ViewModels;
using Prototype.Core.Models;
using Prototype.Core.Models.GasPanel;
using Prototype.Interfaces;

namespace Prototype.ViewModels.Pipes
{
    public class Manifold2Vm : ViewModelBase, IScreenViewModel
    {
        #region Constructors

        public Manifold2Vm()
        {
            ValveVm1 = new ValveVm("Valve1") {State = ValveState.Opened};
            ValveVm2 = new ValveVm("Valve2") {State = ValveState.Opened};

            ValveVertex1 = new ValveVertex(ValveVm1);
            ValveVertex2 = new ValveVertex(ValveVm2);

            SourceVertex1 = new SourceVertex();
            SourceVertex2 = new SourceVertex();

            DestinationVertex1 = new DestinationVertex();

            ConnectionVertex1 = new Vertex();
            ConnectionVertex2 = new Vertex();
            ConnectionVertex3 = new Vertex();

            Scheme = new PipeScheme();
            Scheme.CreatePipe(SourceVertex1, ConnectionVertex1);
            Scheme.CreatePipe(ConnectionVertex1, ValveVertex1);
            Scheme.CreatePipe(ValveVertex1, ConnectionVertex3);

            Scheme.CreatePipe(SourceVertex2, ConnectionVertex2);
            Scheme.CreatePipe(ConnectionVertex2, ConnectionVertex1);
            Scheme.CreatePipe(ConnectionVertex2, ValveVertex2);
            Scheme.CreatePipe(ValveVertex2, ConnectionVertex3);

            Scheme.CreatePipe(ConnectionVertex3, DestinationVertex1);

            Scheme.Initialize();
        }

        #endregion

        #region Properties

        public string DisplayName
        {
            get { return "Manifold 2"; }
        }

        public ValveVm ValveVm1 { get; }
        public ValveVm ValveVm2 { get; }

        public PipeScheme Scheme { get; }

        public ValveVertex ValveVertex1 { get; }
        public ValveVertex ValveVertex2 { get; }

        public SourceVertex SourceVertex1 { get; }
        public SourceVertex SourceVertex2 { get; }

        public DestinationVertex DestinationVertex1 { get; }

        public Vertex ConnectionVertex1 { get; }
        public Vertex ConnectionVertex2 { get; }
        public Vertex ConnectionVertex3 { get; }

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