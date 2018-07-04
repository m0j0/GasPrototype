using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace Prototype.Core.Interfaces
{
    public interface IMenu : INotifyPropertyChanged
    {
        string Header { get; }

        IReadOnlyCollection<IMenuItem> Items { get; }
    }

    public interface IMenuItem : INotifyPropertyChanged
    {
        string Text { get; }

        ICommand Command { get; }

        IMenu Submenu { get; }
    }
}