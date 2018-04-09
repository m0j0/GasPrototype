using System.Windows.Input;
using MugenMvvmToolkit.Interfaces.Models;

namespace Prototype.Core.Interfaces
{
    public interface INamedCommand : IHasDisplayName
    {
        ICommand Command { get; }
    }
}
