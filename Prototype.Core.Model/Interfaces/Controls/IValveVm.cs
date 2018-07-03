using System.Collections.Generic;
using System.ComponentModel;
using Prototype.Core.Models;

namespace Prototype.Core.Interfaces.Controls
{
    public interface IValveVm : INotifyPropertyChanged
    {
        ValveState State { get; }

        bool IsPresent { get; }

        string Name { get; }

        IMenu Menu { get; }
    }
}
