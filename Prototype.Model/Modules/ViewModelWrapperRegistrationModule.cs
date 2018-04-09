using MugenMvvmToolkit.Interfaces;
using MugenMvvmToolkit.Interfaces.ViewModels;
using MugenMvvmToolkit.Modules;
using Prototype.Interfaces;
using Prototype.ViewModels;

namespace Prototype.Modules
{
    public class ViewModelWrapperRegistrationModule : WrapperRegistrationModuleBase
    {
        #region Overrides of WrapperRegistrationModuleBase

        protected override void RegisterWrappers(IConfigurableWrapperManager wrapperManager)
        {
            wrapperManager.AddWrapper<IDisplayWrapperVm, DisplayWrapperVm<IViewModel>>();
        }

        #endregion
    }
}
