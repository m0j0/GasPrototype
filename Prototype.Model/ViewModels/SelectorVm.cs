using System.Threading.Tasks;
using System.Windows.Input;
using MugenMvvmToolkit;
using MugenMvvmToolkit.DataConstants;
using MugenMvvmToolkit.Interfaces.Models;
using MugenMvvmToolkit.Interfaces.Navigation;
using MugenMvvmToolkit.Interfaces.ViewModels;
using MugenMvvmToolkit.Models;
using MugenMvvmToolkit.ViewModels;
using Prototype.Infrastructure;
using Prototype.Interfaces;

namespace Prototype.ViewModels
{
    public class SelectorVm : CloseableViewModel, IHasDisplayName, IParameterProvider
    {
        #region Fields

        private readonly IAccessProvider _accessProvider;
        private IViewModel _selectedVm;
        private string _displayName;

        #endregion

        #region Constructors

        public SelectorVm(IAccessProvider accessProvider)
        {
            Should.NotBeNull(accessProvider, "accessProvider");
            _accessProvider = accessProvider;

            DisplayName = "Selector view model";
            SelectCompositeVmCommand = new RelayCommand(SelectCompositeVm);
            SelectNavigationLogVmCommand = new RelayCommand(SelectNavigationLogVm);
            SelectRestrictedVmCommand = new RelayCommand(SelectRestrictedVm, CanSelectRestrictedVm, this, _accessProvider);

            ((IIocContainerOwnerViewModel)this).RequestOwnIocContainer();
            IocContainer.BindToConstant<IParameterProvider>(this);
        }

        #endregion

        #region Commands

        public ICommand SelectCompositeVmCommand { get; private set; }

        private void SelectCompositeVm()
        {
            SelectedVm = GetViewModel<CompositeParentVm>();
        }
        
        public ICommand SelectNavigationLogVmCommand { get; private set; }

        private void SelectNavigationLogVm()
        {
            SelectedVm = GetViewModel<NavigationLogVm>();
        }

        public ICommand SelectRestrictedVmCommand { get; private set; }

        private void SelectRestrictedVm()
        {
            var restrictedVm = GetViewModel<RestrictedVm>();
            restrictedVm.Settings.Metadata.AddOrUpdate(ViewModelConstants.CloseHandler, CloseViewModel);
            SelectedVm = restrictedVm;
        }

        private bool CanSelectRestrictedVm()
        {
            return _accessProvider.GetAccess(AccessObject.RestrictedVm).CanRead();
        }

        #endregion

        #region Properties

        public string DisplayName
        {
            get { return _displayName; }
            private set
            {
                if (value == _displayName)
                    return;

                _displayName = value;
                OnPropertyChanged();
            }
        }

        public IViewModel SelectedVm
        {
            get { return _selectedVm; }
            private set
            {
                if (Equals(value, _selectedVm))
                {
                    return;
                }

                _selectedVm = value;

                var hasDisplayName = _selectedVm as IHasDisplayName;
                DisplayName = hasDisplayName != null ? hasDisplayName.DisplayName : "Unknown view model";

                OnPropertyChanged();
            }
        }

        public string Parameter { get; set; }

        #endregion

        #region Methods

        private Task<bool> CloseViewModel(INavigationDispatcher navigationDispatcher, IViewModel viewModel, IDataContext ctx)
        {
            return this.CloseAsync();
        }

        #endregion
    }
}