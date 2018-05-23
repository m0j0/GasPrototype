using MugenMvvmToolkit.Collections;
using MugenMvvmToolkit.ViewModels;

namespace Prototype.Core.Models.AttrList
{
   public class AttrTableVm : ViewModelBase
    {
        public AttrTableVm()
        {
            Attrs = new SynchronizedNotifiableCollection<AttrItemModel>();
        }

        public string Title { get; set; }

        public SynchronizedNotifiableCollection<AttrItemModel> Attrs { get; }
    }
}
