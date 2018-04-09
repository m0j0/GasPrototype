using System.Windows.Input;
using Prototype.Core.Interfaces;

namespace Prototype.Core.Models
{
    public class NamedCommand : INamedCommand
    {
        private readonly string _name;
        private readonly ICommand _command;

        public NamedCommand(string name, ICommand command)
        {
            _name = name;
            _command = command;
        }

        public string DisplayName
        {
            get { return _name; }
        }

        public ICommand Command
        {
            get { return _command; }
        }
    }
}
