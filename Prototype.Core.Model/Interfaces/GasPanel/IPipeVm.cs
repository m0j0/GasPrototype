using System.ComponentModel;
using Prototype.Core.Models.GasPanel;

namespace Prototype.Core.Interfaces.GasPanel
{
    internal interface IPipeVm : INotifyPropertyChanged
    {
        bool IsPresent { get; set; }

        bool HasFlow { get; }

        SubstanceType SubstanceType { get; }
    }
}
