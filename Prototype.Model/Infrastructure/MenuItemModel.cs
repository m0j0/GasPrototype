using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using MugenMvvmToolkit;
using MugenMvvmToolkit.Models;

namespace Prototype.Infrastructure
{
    public class MenuItemModel
    {
        #region Fields

        private readonly string _caption;
        private readonly IReadOnlyCollection<MenuItemModel> _children;
        private readonly ICommand _command;

        #endregion

        #region Constructors

        public MenuItemModel(string caption, IReadOnlyCollection<MenuItemModel> children)
        {
            _caption = caption;
            _children = children;
        }

        public MenuItemModel(string caption, ICommand command)
        {
            _caption = caption;
            _command = command;
            _children = Empty.Array<MenuItemModel>();
        }

        public MenuItemModel(string caption, Func<Task> method)
            : this(caption, new AsyncRelayCommand(method))
        {
        }

        #endregion

        #region Properties

        public string Caption
        {
            get { return _caption; }
        }

        public ICommand Command
        {
            get { return _command; }
        }

        public IReadOnlyCollection<MenuItemModel> Children
        {
            get { return _children; }
        }

        #endregion
    }
}