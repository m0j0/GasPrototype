using System.ComponentModel;
using Prototype.Core.Models.GasPanel;

namespace Prototype.Core.Interfaces.GasPanel
{
    internal interface IPipeVm : INotifyPropertyChanged
    {
        bool HasFlow { get; }

        SubstanceType SubstanceType { get; }
    }
}
