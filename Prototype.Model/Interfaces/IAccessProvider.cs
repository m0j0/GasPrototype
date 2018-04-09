using System;
using System.ComponentModel;
using Prototype.Infrastructure;

namespace Prototype.Interfaces
{
    public interface IAccessProvider : INotifyPropertyChanged
    {
        Access GetAccess(Guid accessObject);

        void UpdateAccess(Access access);
    }
}
