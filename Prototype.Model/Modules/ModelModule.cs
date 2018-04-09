using MugenMvvmToolkit;
using MugenMvvmToolkit.Interfaces;
using MugenMvvmToolkit.Interfaces.Models;
using MugenMvvmToolkit.Models.IoC;
using Prototype.Infrastructure;
using Prototype.Interfaces;

namespace Prototype.Modules
{
    public class ModelModule : IModule
    {
        #region Implementation of IModule

        public bool Load(IModuleContext context)
        {
            context.IocContainer.Bind<IAccessProvider, AccessProvider>(DependencyLifecycle.SingleInstance);

            return true;
        }

        public void Unload(IModuleContext context)
        {
        }

        public int Priority
        {
            get { return ApplicationSettings.ModulePriorityDefault; }
        }

        #endregion
    }
}
