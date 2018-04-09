using System;
using System.Runtime.CompilerServices;
using MugenMvvmToolkit;
using MugenMvvmToolkit.DataConstants;
using MugenMvvmToolkit.Interfaces.Models;
using MugenMvvmToolkit.Interfaces.Navigation;
using MugenMvvmToolkit.Interfaces.ViewModels;
using MugenMvvmToolkit.Models;
using Prototype.Infrastructure;

namespace Prototype
{
    public static class ModelExtensions
    {
        public static bool CanRead(this Access access)
        {
            return access == Access.Read || access == Access.ReadWrite;
        }

        public static void InitializeChildViewModel(this IViewModel parentViewModel, IViewModel childViewModel)
        {
            Should.NotBeNull(parentViewModel, "parentViewModel");
            Should.NotBeNull(childViewModel, "childViewModel");
            ServiceProvider.ViewModelProvider.InitializeViewModel(childViewModel,
                new DataContext(InitializationConstants.ParentViewModel.ToValue(parentViewModel)));
        }

        public static string GetNavigationTrace(IViewModel viewModel, INavigationContext ctx, [CallerMemberName] string method = "")
        {
            return string.Format("Source '{0}', method '{1}', from '{2}' to '{3}' mode '{4}', time {5:T}",
                GetName(viewModel),
                method,
                GetName(ctx.ViewModelFrom),
                GetName(ctx.ViewModelTo),
                ctx.NavigationMode,
                DateTime.Now);
        }

        private static string GetName(IViewModel viewModel)
        {
            if (viewModel == null)
            {
                return "(null)";
            }
            var vmWithDisplayName = viewModel as IHasDisplayName;
            return vmWithDisplayName != null ? vmWithDisplayName.DisplayName : viewModel.GetType().Name;
        }
    }
}
