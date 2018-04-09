using System.Collections.Generic;
using System.Windows.Input;
using MugenMvvmToolkit.Models;
using Prototype.Core.Interfaces;
using Prototype.Core.Interfaces.GasPanel;

namespace Prototype.Core.Models.GasPanel
{
    public class ValveVm : NotifyPropertyChangedBase, IValveVm
    {
        #region Fields

        private readonly NamedCommand[] _commands;
        private readonly ICommand _openCommand;
        private readonly ICommand _closeCommand;
        private readonly string _name;
        private bool _isPresent = true;
        private ValveState _state = ValveState.Unknown;

        #endregion

        #region Constructors

        public ValveVm(string name = "")
        {
            _name = name;

            _openCommand = new RelayCommand(Open, CanOpen, this);
            _closeCommand = new RelayCommand(Close, CanClose, this);

            _commands = new[]
            {
                new NamedCommand("Open", OpenCommand),
                new NamedCommand("Close", CloseCommand)
            };
        }

        #endregion

        #region Commands

        public ICommand OpenCommand
        {
            get { return _openCommand; }
        }

        private void Open()
        {
            State = ValveState.Opened;
        }

        private bool CanOpen()
        {
            return State != ValveState.Opened;
        }

        public ICommand CloseCommand
        {
            get { return _closeCommand; }
        }

        private void Close()
        {
            State = ValveState.Closed;
        }

        private bool CanClose()
        {
            return State != ValveState.Closed;
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