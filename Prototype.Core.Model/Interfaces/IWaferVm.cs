using System.Collections.Generic;
using System.ComponentModel;
using Prototype.Core.Models;

namespace Prototype.Core.Interfaces
{
    public interface IWaferVm : INotifyPropertyChanged
    {
        string Label { get; }

        string DetailedLabel { get; }

        WaferStatus Status { get; }

        WaferWorkingMode WorkingMode { get; }
    }

    public interface IWaferMovableVm : INotifyPropertyChanged
    {
        IWaferVm WaferVm { get; }

        MoveSourceMode MoveSourceMode { get; }

        bool IsStartRange { get; }

        WaferPopupType PopupType { get; }

        IReadOnlyCollection<INamedCommand> Commands { get; }

        IWaferModel WaferModel { get; }
    }
}
