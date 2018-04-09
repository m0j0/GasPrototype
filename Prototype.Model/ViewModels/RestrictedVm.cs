using MugenMvvmToolkit;
using MugenMvvmToolkit.Interfaces.Models;
using MugenMvvmToolkit.ViewModels;
using Prototype.Infrastructure;
using Prototype.Interfaces;

namespace Prototype.ViewModels
{
    public class RestrictedVm : CloseableViewModel, IHasDisplayName
    {
        #region Fields

        private readonly IAccessProvider _accessProvider;
        private bool _isReadOnly;

        #endregion

        #region Constructors

        public RestrictedVm(IAccessProvider accessProvider)
        {
            Should.NotBeNull(accessProvider, "accessProvider");

            var access = accessProvider.GetAccess(AccessObject.RestrictedVm);
            Should.BeValid("access", access != Access.None);
            _accessProvider = accessProvider;

            IsReadOnly = access == Access.Read;
            accessProvider.PropertyChanged += ReflectionExtensions.MakeWeakPropertyChangedHandler(this, (vm, o, arg3) =>
            {
                var closureAccess = vm._accessProvider.GetAccess(AccessObject.RestrictedVm);
                if (closureAccess == Access.None)
                {
                    vm.CloseAsync();
                }
                else
                {
                    vm.IsReadOnly = closureAccess == Access.Read;
                }
            });
        }

        #endregion

        #region Properties

        public string DisplayName
        {
            get { return "Restricted view model"; }
        }

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            private set
            {
                if (value == _isReadOnly)
                {
                    return;
                }

                _isReadOnly = value;
                OnPropertyChanged();
            }
        }

        public string Content { get; set; }

        #endregion
    }
}