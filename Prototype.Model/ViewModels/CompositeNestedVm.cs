using MugenMvvmToolkit.Interfaces.Models;
using MugenMvvmToolkit.ViewModels;

namespace Prototype.ViewModels
{
    public class CompositeNestedVm : ViewModelBase, IHasDisplayName
    {
        #region Properties

        public string DisplayName { get; set; }

        #endregion
    }
}
