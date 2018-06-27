using System.Collections.Generic;
using System.Windows.Input;
using MugenMvvmToolkit.Models;
using Prototype.Core.Interfaces;
using Prototype.Core.Interfaces.Controls;

namespace Prototype.Core.Models.GasPanel
{
    public class ValveVm : NotifyPropertyChangedBase, IValveVm
    {
        #region Fields

        private readonly NamedCommand[] _commands;
        private readonly string _name;
        private bool _isPresent = true;
        private ValveState _state = ValveState.Unknown;

        #endregion

        #region Constructors

        public ValveVm(string name = "")
        {
            _name = name;

            OpenCommand = new RelayCommand(Open, CanOpen, this);
            CloseCommand = new RelayCommand(Close, CanClose, this);
            HideCommand = new RelayCommand(Hide, CanHide, this);

            _commands = new[]
            {
                new NamedCommand("Open", OpenCommand),
                new NamedCommand("Close", CloseCommand),
                new NamedCommand("Hide", HideCommand),
            };
        }

        #endregion

        #region Commands

        public ICommand OpenCommand { get; }

        private void Open()
        {
            State = ValveState.Open;
        }

        private bool CanOpen()
        {
            return State != ValveState.Open;
        }

        public ICommand CloseCommand { get; }

        private void Close()
        {
            State = ValveState.Closed;
        }

        private bool CanClose()
        {
            return State != ValveState.Closed;
        }

        public ICommand HideCommand { get; }

        private void Hide()
        {
            IsPresent = false;
        }

        private bool CanHide()
        {
            return IsPresent;
        }

        #endregion

        #region Properties

        public ValveState State
        {
            get { return _state; }
            set
            {
                if (_state == value)
                {
                    return;
                }
                _state = value;
                OnPropertyChanged();
            }
        }

        public bool IsPresent
        {
            get { return _isPresent; }
            set
            {
                if (value == _isPresent)
                {
                    return;
                }
                _isPresent = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return _name; }
        }

        public IReadOnlyCollection<INamedCommand> Commands
        {
            get { return _commands; }
        }

        #endregion
    }
}