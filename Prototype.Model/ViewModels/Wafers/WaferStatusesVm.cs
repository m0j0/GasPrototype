using System.Collections.Generic;
using MugenMvvmToolkit.ViewModels;
using Prototype.Core.Interfaces;
using Prototype.Core.Models;
using Prototype.Interfaces;

namespace Prototype.ViewModels.Wafers
{
    public class WaferStatusesVm : ViewModelBase, IScreenViewModel
    {
        private readonly IReadOnlyCollection<IWaferVm> _wafers1;
        private readonly IReadOnlyCollection<IWaferVm> _wafers2;

        public WaferStatusesVm()
        {
            var wafers1 = new List<IWaferVm>();
            var wafers2 = new List<IWaferVm>();
            for (int i = 0; i < 20; i++)
            {
                wafers1.Add(new WaferVm(i > 10 ? "ABCDEFEDCBA" + i : "A" + i, (WaferStatus)i, WaferWorkingMode.Standard));
                wafers2.Add(new WaferVm(i > 10 ? "ABCDEFEDCBA" + i : "B" + i, (WaferStatus)i, WaferWorkingMode.Gating));
            }
            _wafers1 = wafers1;
            _wafers2 = wafers2;
        }

        public IReadOnlyCollection<IWaferVm> Wafers1
        {
            get { return _wafers1; }
        }
        
        public IReadOnlyCollection<IWaferVm> Wafers2
        {
            get { return _wafers2; }
        }

        public string DisplayName
        {
            get { return "Wafer statuses"; }
        }
    }
}
