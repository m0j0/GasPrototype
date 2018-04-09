using System.Windows.Input;
using MugenMvvmToolkit;
using MugenMvvmToolkit.Models;
using MugenMvvmToolkit.ViewModels;
using Prototype.Infrastructure;
using Prototype.Interfaces;

namespace Prototype.ViewModels
{
    public class SettingsVm : ViewModelBase, IScreenViewModel
    {
        #region Fields

        private readonly IAccessProvider _accessProvider;
        private Access _selectedAccess;

        #endregion

        #region Constructors

        public SettingsVm(IAccessProvider accessProvider)
        {
            Should.NotBeNull(accessProvider, "accessProvider");
            _accessProvider = accessProvider;
            SelectedAccess = _accessProvider.GetAccess(AccessObject.RestrictedVm);
            UpdateSettingsCommand = new RelayCommand(UpdateSettings);
        }

        #endregion

        #region Commands

        public ICommand UpdateSettingsCommand { get; private set; }

        private void UpdateSettings()
        {
            _accessProvider.UpdateAccess(SelectedAccess);
            this.CloseAsync();
        }

        #endregion

        #region Properties

        public string DisplayName
        {
            get { return "Settings"; }
        }

        public Access SelectedAccess
        {
            get { return _selectedAccess; }
            set
            {
                if (value == _selectedAccess)
                {
                    return;
                }

                _selectedAccess = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}