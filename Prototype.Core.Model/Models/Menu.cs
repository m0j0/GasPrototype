using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MugenMvvmToolkit.Models;
using Prototype.Core.Interfaces;

namespace Prototype.Core.Models
{
    public sealed class Menu : NotifyPropertyChangedBase, IMenu
    {
        public Menu(string header, params IMenuItem[] menuItems)
        {
            Header = header;
            Items = new ObservableCollection<IMenuItem>(menuItems);
        }

        public string Header { get; }

        public IReadOnlyCollection<IMenuItem> Items { get; }
    }

    public sealed class MenuItem : NotifyPropertyChangedBase, IMenuItem
    {
        public MenuItem(string text, ICommand command)
        {
            Text = text;
            Command = command;
        }

        public MenuItem(string text, ICommand command, IMenu submenu) : this(text, command)
        {
            Submenu = submenu;
        }

        public string Text { get; }

        public ICommand Command { get; }

        public IMenu Submenu { get; }
    }
}