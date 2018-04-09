using System;
using System.Threading.Tasks;
using MugenMvvmToolkit;
using MugenMvvmToolkit.Interfaces.Models;
using MugenMvvmToolkit.Interfaces.Navigation;
using MugenMvvmToolkit.Interfaces.ViewModels;
using MugenMvvmToolkit.ViewModels;

namespace Prototype.ViewModels
{
    public class NavigationLogVm : ViewModelBase, IHasDisplayName, INavigableViewModel
    {
        #region Fields

        private string _log;

        #endregion

        #region Properties

        public string DisplayName
        {
            get { return "Navigation log view model"; }
        }

        public string Log
        {
            get { return _log; }
            private set
            {
                if (value == _log)
                {
                    return;
                }
                _log = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Implementation of INavigableViewModel

        void INavigableViewModel.OnNavigatedTo(INavigationContext context)
        {
            Log += ModelExtensions.GetNavigationTrace(this, context) + Environment.NewLine;
        }

        Task<bool> INavigableViewModel.OnNavigatingFromAsync(INavigationContext context)
        {
            Log += ModelExtensions.GetNavigationTrace(this, context) + Environment.NewLine;
            return Empty.TrueTask;
        }

        void INavigableViewModel.OnNavigatedFrom(INavigationContext context)
        {
            Log += ModelExtensions.GetNavigationTrace(this, context) + Environment.NewLine;
        }

        #endregion
    }
}