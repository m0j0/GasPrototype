using MugenMvvmToolkit.Interfaces.ViewModels;
using MugenMvvmToolkit.ViewModels;
using Prototype.Interfaces;

namespace Prototype.ViewModels
{
    public class DisplayWrapperVm<T> : WrapperViewModelBase<T>, IDisplayWrapperVm where T : class, IViewModel
    {
    }
}
