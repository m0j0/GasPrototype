using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MugenMvvmToolkit;
using MugenMvvmToolkit.Infrastructure.Presenters;
using MugenMvvmToolkit.Interfaces.Models;
using MugenMvvmToolkit.Interfaces.Navigation;
using MugenMvvmToolkit.Interfaces.Presenters;
using MugenMvvmToolkit.Interfaces.ViewModels;
using MugenMvvmToolkit.Models;
using MugenMvvmToolkit.ViewModels;
using Prototype.Infrastructure;
using Prototype.Interfaces;
using Prototype.ViewModels.AttrList;
using Prototype.ViewModels.Pipes;
using Prototype.ViewModels.Wafers;

namespace Prototype.ViewModels
{
    public class MainVm : MultiViewModel<IScreenViewModel>, IHasDisplayName, INavigableViewModel
    {
        #region Fields

        private readonly IScreenManager _screenManager;

        private readonly IList<MenuItemModel> _menuItems;
        private readonly ICommand _closeSelectedCommand;
        private readonly ICommand _forwardCommand;
        private readonly ICommand _backCommand;

        private readonly List<IScreenViewModel> _backViewModels = new List<IScreenViewModel>();
        private readonly List<IScreenViewModel> _forwardViewModels = new List<IScreenViewModel>();
        private bool _isCommandSelectionChanging;

        #endregion

        #region Constructors

        public MainVm(IViewModelPresenter viewModelPresenter)
        {
            Should.NotBeNull(viewModelPresenter, "viewModelPresenter");

            _screenManager = new ScreenManager(this);
            IocContainer.BindToConstant(_screenManager);

            var presenter = new DynamicMultiViewModelPresenter(this);
            viewModelPresenter.DynamicPresenters.Add(presenter);

            _backCommand = new RelayCommand(Back, CanBack, this);
            _forwardCommand = new RelayCommand(Forward, CanForward, this);
            _closeSelectedCommand = new RelayCommand(CloseSelected, CanCloseSelected, this);

            _menuItems = new List<MenuItemModel>
            {
                new MenuItemModel("Screens", new List<MenuItemModel>
                {
                    new MenuItemModel("Cfm application", _screenManager.ShowScreenAsync<CfmApplicationVm>),
                    new MenuItemModel("Settings", _screenManager.ShowScreenAsync<SettingsVm>),
                    new MenuItemModel("Pipes", _screenManager.ShowScreenAsync<PipesExampleVm>),
                    new MenuItemModel("Pipes performance", _screenManager.ShowScreenAsync<PipesPerformanceVm>),
                    new MenuItemModel("Pipes scaling", _screenManager.ShowScreenAsync<PipesScalingVm>),
                    new MenuItemModel("Pipes connections", _screenManager.ShowScreenAsync<PipesConnectionsVm>),
                    new MenuItemModel("Manifold", _screenManager.ShowScreenAsync<ManifoldVm>),
                    new MenuItemModel("Manifold 2", _screenManager.ShowScreenAsync<Manifold2Vm>),
                    new MenuItemModel("Manifold 3", _screenManager.ShowScreenAsync<Manifold3Vm>),
                    new MenuItemModel("Wafer statuses", _screenManager.ShowScreenAsync<WaferStatusesVm>),
                    new MenuItemModel("TieListFrame example VM", _screenManager.ShowScreenAsync<VmExampleVm>),
                    new MenuItemModel("TieListFrame example CB", _screenManager.ShowScreenAsync<CbExampleVm>),
                }),
                new MenuItemModel("Popups", new List<MenuItemModel>
                {
                    new MenuItemModel("Cfm application", () => _screenManager.ShowPopupAsync<CfmApplicationVm>()),
                    new MenuItemModel("Settings", () => _screenManager.ShowPopupAsync<SettingsVm>())
                }),
                new MenuItemModel("Unique popups", new List<MenuItemModel>
                {
                    new MenuItemModel("Cfm application", () => _screenManager.ShowPopupAsync<CfmApplicationVm>(true)),
                    new MenuItemModel("Settings", () => _screenManager.ShowPopupAsync<SettingsVm>(true))
                })
            };

            if (IsDesignMode)
            {
                _screenManager.ShowScreenAsync<CfmApplicationVm>();
            }
        }

        #endregion

        #region Commands

        public ICommand BackCommand
        {
            get { return _backCommand; }
        }

        private void Back()
        {
            try
            {
                _isCommandSelectionChanging = true;

                _forwardViewModels.Add(SelectedItem);
                SelectedItem = _backViewModels.Last();
                _backViewModels.RemoveAt(_backViewModels.Count - 1);
            }
            finally
            {
                _isCommandSelectionChanging = false;
                InvalidateCommands();
            }
        }

        private bool CanBack()
        {
            return _backViewModels.Count > 0;
        }

        public ICommand ForwardCommand
        {
            get { return _forwardCommand; }
        }

        private void Forward()
        {
            try
            {
                _isCommandSelectionChanging = true;

                _backViewModels.Add(SelectedItem);
                SelectedItem = _forwardViewModels.Last();
                _forwardViewModels.RemoveAt(_forwardViewModels.Count - 1);
            }
            finally
            {
                _isCommandSelectionChanging = false;
                InvalidateCommands();
            }
        }

        private bool CanForward()
        {
            return _forwardViewModels.Count > 0;
        }

        public ICommand CloseSelectedCommand
        {
            get { return _closeSelectedCommand; }
        }

        private void CloseSelected()
        {
            SelectedItem.CloseAsync();
        }

        private bool CanCloseSelected()
        {
            return SelectedItem != null;
        }

        #endregion

        #region Properties

        public string DisplayName
        {
            get { return "Main view model"; }
        }

        public IList<MenuItemModel> MenuItems
        {
            get { return _menuItems; }
        }

        #endregion

        #region Methods

        protected override void OnSelectedItemChanged(IScreenViewModel oldValue, IScreenViewModel newValue)
        {
            base.OnSelectedItemChanged(oldValue, newValue);

            if (_isCommandSelectionChanging || oldValue == null)
            {
                return;
            }
            _backViewModels.Add(oldValue);
            _forwardViewModels.Clear();

            InvalidateCommands();
        }

        protected override void OnViewModelAdded(IScreenViewModel viewModel)
        {
            base.OnViewModelAdded(viewModel);

            _forwardViewModels.Clear();

            InvalidateCommands();
        }

        protected override void OnViewModelRemoved(IScreenViewModel viewModel)
        {
            base.OnViewModelRemoved(viewModel);

            _backViewModels.RemoveAll(vm => vm == viewModel);
            _forwardViewModels.RemoveAll(vm => vm == viewModel);
            CreanUpCollection(_backViewModels);
            CreanUpCollection(_forwardViewModels);

            InvalidateCommands();
        }

        private void CreanUpCollection(List<IScreenViewModel> viewModels)
        {
            for (var i = 0; i < viewModels.Count - 1; i++)
            {
                if (viewModels[i] != viewModels[i + 1])
                {
                    continue;
                }

                viewModels.RemoveAt(i);
                i--;
            }

            if (viewModels.Count == 1 && viewModels[0] == SelectedItem)
            {
                viewModels.Clear();
            }
        }

        #endregion

        #region Implementation of INavigableViewModel

        void INavigableViewModel.OnNavigatedTo(INavigationContext context)
        {
            if (context.ViewModelFrom != null)
            {
                return;
            }

            _screenManager.ShowScreenAsync<PipesConnectionsVm>();
        }

        Task<bool> INavigableViewModel.OnNavigatingFromAsync(INavigationContext context)
        {
            return Empty.TrueTask;
        }

        void INavigableViewModel.OnNavigatedFrom(INavigationContext context)
        {
        }

        #endregion
    }
}