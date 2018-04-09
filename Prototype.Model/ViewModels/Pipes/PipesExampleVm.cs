using MugenMvvmToolkit.ViewModels;
using Prototype.Core.Interfaces;
using Prototype.Core.Interfaces.GasPanel;
using Prototype.Core.Models;
using Prototype.Core.Models.GasPanel;
using Prototype.Interfaces;

namespace Prototype.ViewModels.Pipes
{
    public class PipesExampleVm : ViewModelBase, IScreenViewModel
    {
        public PipesExampleVm()
        {
            OpenValve = new ValveVm {State = ValveState.Opened};
            ClosedValve = new ValveVm {State = ValveState.Closed};
            NotPresentedValve = new ValveVm {IsPresent = false};
        }

        public IValveVm OpenValve { get; private set; }

        public IValveVm ClosedValve { get; private set; }

        public IValveVm NotPresentedValve { get; private set; }

        public string DisplayName
        {
            get { return "Pipes example"; }
        }
    }
}
