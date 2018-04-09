using System.Windows.Input;
using MugenMvvmToolkit.Models;
using MugenMvvmToolkit.ViewModels;
using Prototype.Interfaces;

namespace Prototype.ViewModels
{
    public class CfmApplicationVm : MultiViewModel<SelectorVm>, IScreenViewModel
    {
        #region Constructors

        public CfmApplicationVm()
        {
            AddTabCommand = new RelayCommand(AddTab);

            if (IsDesignMode)
            {
                AddTab();
            }
        }

        #endregion

        #region Commands

        public ICommand AddTabCommand { get; private set; }

        private void AddTab()
        {
            AddViewModel(GetViewModel<SelectorVm>());
        }

        #endregion

        #region Properties

        public string DisplayName
        {
            get { return "CFM Application"; }
        }

        #endregion
    }
}