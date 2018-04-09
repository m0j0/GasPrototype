using System;
using System.Collections.Generic;
using System.Linq;
using MugenMvvmToolkit.Models;
using Prototype.Interfaces;

namespace Prototype.Infrastructure
{
    internal class AccessProvider : NotifyPropertyChangedBase, IAccessProvider
    {
        #region Fields

        private readonly Dictionary<Guid, Access> _accesses;

        #endregion

        #region Constructors

        public AccessProvider()
        {
            _accesses = new Dictionary<Guid, Access>
            {
                {AccessObject.RestrictedVm, Access.ReadWrite}
            };
        }

        #endregion

        #region Methods

        public Access GetAccess(Guid accessObject)
        {
            Access value;
            return _accesses.TryGetValue(accessObject, out value) ? value : Access.None;
        }

        public void UpdateAccess(Access access)
        {
            foreach (var pair in _accesses.ToArray())
            {
                _accesses[pair.Key] = access;
            }
            OnPropertyChanged(String.Empty);
        }

        #endregion
    }
}