using System.Threading.Tasks;

namespace Prototype.Interfaces
{
    public interface IScreenManager
    {
        Task ShowScreenAsync<T>() where T : IScreenViewModel;

        Task ShowPopupAsync<T>(bool unique = false) where T : IScreenViewModel;
    }
}
