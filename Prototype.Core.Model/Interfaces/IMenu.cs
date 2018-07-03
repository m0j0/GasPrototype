using System.Collections.Generic;
using System.Windows.Input;

namespace Prototype.Core.Interfaces
{
    public interface IMenu
    {
        string Header { get; }

        IReadOnlyCollection<IMenuItem> Items { get; }
    }

    public interface IMenuItem
    {
        string Text { get; }

        ICommand Command { get; }

        IMenu Submenu { get; }
    }
}