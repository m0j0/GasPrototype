using System.Collections.Generic;
using System.ComponentModel;
using Prototype.Core.Models.GasPanel;

namespace Prototype.Core.Interfaces.GasPanel
{
    public interface IValveVm : INotifyPropertyChanged
    {
        ValveState State { get; }

        bool IsPresent { get; }

        string Name { get; }

        IReadOnlyCollection<INamedCommand> Commands { get; }
    }
}
