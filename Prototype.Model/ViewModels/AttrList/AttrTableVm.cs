using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MugenMvvmToolkit.Collections;
using MugenMvvmToolkit.ViewModels;

namespace Prototype.ViewModels.AttrList
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
