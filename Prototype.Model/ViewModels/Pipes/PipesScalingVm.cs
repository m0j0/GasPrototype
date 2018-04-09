using MugenMvvmToolkit.ViewModels;
using Prototype.Interfaces;

namespace Prototype.ViewModels.Pipes
{
    public class PipesScalingVm : ViewModelBase, IScreenViewModel
    {
        public string DisplayName
        {
            get { return "Pipes scaling example"; }
        }
    }
}
