using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MugenMvvmToolkit;
using MugenMvvmToolkit.DataConstants;
using MugenMvvmToolkit.Interfaces;
using MugenMvvmToolkit.Interfaces.Navigation;
using MugenMvvmToolkit.Interfaces.ViewModels;
using MugenMvvmToolkit.Models;
using MugenMvvmToolkit.Models.EventArg;
using MugenMvvmToolkit.ViewModels;
using Prototype.Interfaces;
using Prototype.ViewModels;

namespace Prototype.Infrastructure
{
    internal class ScreenManager : IScreenManager
    {
        #region Fields

        private readonly MainVm _mainVm;
        private readonly INavigationDispatcher _navigationDispatcher;
        private readonly IViewModelProvider _viewModelProvider;

        #endregion

        #region Constructors

        public ScreenManager(MainVm mainVm, IViewModelProvider viewModelProvider = null, INavigationDispatcher navigationDispatcher = null)
        {
            Should.NotBeNull(mainVm, "mainVm");
            _mainVm = mainVm;

            _viewModelProvider = viewModelProvider ?? mainVm.GetIocContainer(true).Get<IViewModelProvider>();
            _navigationDispatcher = navigationDispatcher ?? mainVm.GetIocContainer(true).Get<INavigationDispatcher>();
            _navigationDispatcher.Navigated += NavigationDispatcherOnNavigated;
        }

        #endregion

        #region Implementation of IScreenManager

        public async Task ShowScreenAsync<T>() where T : IScreenViewModel
        {
            var existignVm = _mainVm.ItemsSource.FirstOrDefault(vm => vm is T);
            if (existignVm != null)
            {
                await existignVm.ShowAsync();
                return;
            }

            using (var vm = _viewModelProvider.GetViewModel<T>(_mainVm))
            {
                await vm.ShowAsync();
            }
        }

        public async Task ShowPopupAsync<T>(bool unique) where T : IScreenViewModel
        {
            if (unique)
            {
                var openedViewModelInfo = _navigationDispatcher.GetOpenedViewModels(NavigationType.Window).FirstOrDefault(info =>
                {
                    if (info.ViewModel is T)
                    {
                        return true;
                    }

                    var wrapperVm = info.ViewModel as IWrapperViewModel;
                    return wrapperVm != null && wrapperVm.ViewModel is T;
                });

                if (openedViewModelInfo != null)
                {
                    await openedViewModelInfo.ViewModel.ShowAsync();
                    return;
                }
            }

            using (var vm = _viewModelProvider.GetViewModel<T>(_mainVm))
            using (var wrapper = vm.Wrap<IDisplayWrapperVm>())
            {
                await wrapper.ShowAsync(NavigationConstants.IsDialog.ToValue(false));
            }
        }

        #endregion

        #region Methods

        private void NavigationDispatcherOnNavigated(INavigationDispatcher sender, NavigatedEventArgs args)
        {
            if (args.Context.NavigationType == NavigationTypeEx.ParentChild)
            {
                return;
            }

            if ((args.Context.ViewModelFrom == _mainVm
                 || args.Context.ViewModelTo == _mainVm) &&
                args.Context.NavigationType == NavigationType.Window)
            {
                return;
            }

            var createdViewModels = _viewModelProvider.GetCreatedViewModels();

            var viewModelFrom = args.Context.ViewModelFrom;
            if (viewModelFrom != null)
            {
                NavigateChildren(viewModelFrom, createdViewModels,
                    navigableVm => navigableVm.OnNavigatedFrom(new NavigationContext(NavigationTypeEx.ParentChild, NavigationMode.Refresh, null, null, this)));
            }

            var viewModelTo = args.Context.ViewModelTo;
            if (viewModelTo != null)
            {
                NavigateChildren(viewModelTo, createdViewModels,
                    navigableVm => navigableVm.OnNavigatedTo(new NavigationContext(NavigationTypeEx.ParentChild, NavigationMode.Refresh, null, null, this)));
            }
        }

        private void NavigateChildren(IViewModel parentViewModel, IList<IViewModel> createdViewModels, Action<INavigableViewModel> action)
        {
            foreach (var viewModel in createdViewModels)
            {
                if (viewModel.GetParentViewModel() != parentViewModel)
                {
                    continue;
                }

                var navigableViewModel = viewModel as INavigableViewModel;
                if (navigableViewModel != null)
                {
                    action(navigableViewModel);
                }
                NavigateChildren(viewModel, createdViewModels, action);
            }
        }

        #endregion
    }
}