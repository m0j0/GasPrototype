using MugenMvvmToolkit.Models;

namespace Prototype.Infrastructure
{
    public sealed class NavigationTypeEx : NavigationType
    {
        #region Fields

        public static readonly NavigationType ParentChild = new NavigationType("ParentChild", null);

        #endregion

        #region Constructors

        public NavigationTypeEx(string id, OperationType operation) : base(id, operation)
        {
        }

        #endregion
    }
}