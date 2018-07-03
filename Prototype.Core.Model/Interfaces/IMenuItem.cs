using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    public sealed class MenuItem : IMenuItem
    {
        public MenuItem(string text, ICommand command)
        {
            Text = text;
            Command = command;
        }

        public string Text { get; }

        public ICommand Command { get; }

        public IMenu Submenu { get; set; }
    }

    public sealed class Menu : IMenu
    {
        public Menu(string header, params IMenuItem[] menuItems)
        {
            Header = header;
            Items = new ObservableCollection<IMenuItem>(menuItems);
        }

        public string Header { get; }

        public IReadOnlyCollection<IMenuItem> Items { get; }
    }
}