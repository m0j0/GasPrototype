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
using Prototype.Core.Interfaces;
using Prototype.Core.Models;
using Prototype.Infrastructure;
using Prototype.Interfaces;
using Prototype.ViewModels.Pipes;

namespace Prototype.ViewModels
{
    public class MainVm : MultiViewModel<IScreenViewModel>, IHasDisplayName, INavigableViewModel
    {
        #region Fields

        private readonly IScreenManager _screenManager;

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

            BackCommand = new RelayCommand(Back, CanBack, this);
            ForwardCommand = new RelayCommand(Forward, CanForward, this);
            CloseSelectedCommand = new RelayCommand(CloseSelected, CanCloseSelected, this);

            MenuItems = new List<IMenu>
            {
                new Menu("Screens",
                    new MenuItem("Cfm application", new AsyncRelayCommand(_screenManager.ShowScreenAsync<CfmApplicationVm>)),
                    new MenuItem("Settings", new AsyncRelayCommand(_screenManager.ShowScreenAsync<SettingsVm>)),
                    new MenuItem("Pipes", new AsyncRelayCommand(_screenManager.ShowScreenAsync<PipesExampleVm>)),
                    new MenuItem("Pipes performance", new AsyncRelayCommand(_screenManager.ShowScreenAsync<PipesPerformanceVm>)),
                    new MenuItem("Pipes scaling", new AsyncRelayCommand(_screenManager.ShowScreenAsync<PipesScalingVm>)),
                    new MenuItem("Pipes connections", new AsyncRelayCommand(_screenManager.ShowScreenAsync<PipesConnectionsVm>)),
                    new MenuItem("Manifold", new AsyncRelayCommand(_screenManager.ShowScreenAsync<ManifoldVm>)),
                    new MenuItem("Manifold 2", new AsyncRelayCommand(_screenManager.ShowScreenAsync<Manifold2Vm>)),
                    new MenuItem("Manifold 3", new AsyncRelayCommand(_screenManager.ShowScreenAsync<Manifold3Vm>)),
                    new MenuItem("Manifold 4", new AsyncRelayCommand(_screenManager.ShowScreenAsync<Manifold4Vm>)),
                    new MenuItem("Valve3Way example", new AsyncRelayCommand(_screenManager.ShowScreenAsync<Valve3WayExampleVm>))),
                new Menu("Popups",
                    new MenuItem("Cfm application", new AsyncRelayCommand(() => _screenManager.ShowPopupAsync<CfmApplicationVm>())),
                    new MenuItem("Settings", new AsyncRelayCommand(() => _screenManager.ShowPopupAsync<SettingsVm>()))),
                new Menu("Unique popups",
                    new MenuItem("Cfm application", new AsyncRelayCommand(() => _screenManager.ShowPopupAsync<CfmApplicationVm>(true))),
                    new MenuItem("Settings", new AsyncRelayCommand(() => _screenManager.ShowPopupAsync<SettingsVm>(true))))
            };

            if (IsDesignMode)
            {
                _screenManager.ShowScreenAsync<CfmApplicationVm>();
            }
        }

        #endregion

        #region Commands

        public ICommand BackCommand { get; }

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

        public ICommand ForwardCommand { get; }

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

        public ICommand CloseSelectedCommand { get; }

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

        public IList<IMenu> MenuItems { get; }

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

            _screenManager.ShowScreenAsync<Valve3WayExampleVm>();
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