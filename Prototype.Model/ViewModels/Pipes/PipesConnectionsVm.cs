using MugenMvvmToolkit.ViewModels;
using Prototype.Interfaces;

namespace Prototype.ViewModels.Pipes
{
    public class PipesConnectionsVm : ViewModelBase, IScreenViewModel
    {
        #region Properties

        public string DisplayName
        {
            get { return "Pipes connections"; }
        }

        #endregion
    }
}