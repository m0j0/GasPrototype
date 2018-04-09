using MugenMvvmToolkit;
using MugenMvvmToolkit.Interfaces;
using MugenMvvmToolkit.Interfaces.Models;
using MugenMvvmToolkit.Models;
using MugenMvvmToolkit.Models.IoC;
using Prototype.Interfaces;

namespace Prototype.Modules
{
    public class ViewModule : IModule
    {
        #region Nested types

        private class ParameterProvider : IParameterProvider
        {
            public string Parameter
            {
                get { return "DesignTime"; }
            }
        }

        #endregion

        #region Implementation of IParameterProvider

        public bool Load(IModuleContext context)
        {
            if (context.Mode == LoadMode.Design)
            {
                context.IocContainer.Bind<IParameterProvider, ParameterProvider>(DependencyLifecycle.SingleInstance);
            }

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
