using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MugenMvvmToolkit.Interfaces.Models;
using MugenMvvmToolkit.ViewModels;
using Prototype.Interfaces;

namespace Prototype.ViewModels.AttrList
{
    public class VmExampleVm : CloseableViewModel, IScreenViewModel
    {
        public VmExampleVm()
        {
            PumpVm = GetViewModel<AttrTableVm>();
            PumpVm.Title = "Pump";
            PumpVm.Attrs.Add(new AttrItemModel("Status", "rStatus"));
            PumpVm.Attrs.Add(new AttrItemModel("Command", "wCommand", true));
            PumpVm.Attrs.Add(new AttrItemModel("N2 flow low", "sN2FlowLow"));
            PumpVm.Attrs.Add(new AttrItemModel("Pump fault", "sFault"));
            PumpVm.Attrs.Add(new AttrItemModel("Pump warning", "sWarning"));
            PumpVm.Attrs.Add(new AttrItemModel("Pump running", "sPumpOn"));

            ArinnaTurboThrottleVm = GetViewModel<AttrTableVm>();
            ArinnaTurboThrottleVm.Title = "Arinna turbo throttle valve";
            ArinnaTurboThrottleVm.Attrs.Add(new AttrItemModel("Pressure", "rPressure"));
            ArinnaTurboThrottleVm.Attrs.Add(new AttrItemModel("Pressure setpoint", "wSetpointPressure"));
            ArinnaTurboThrottleVm.Attrs.Add(new AttrItemModel("Ramp rate", "wPressRampRate"));
            ArinnaTurboThrottleVm.Attrs.Add(new AttrItemModel("Position", "rPosition"));
            ArinnaTurboThrottleVm.Attrs.Add(new AttrItemModel("Position setpoint", "wSetpointPosition"));
        }

        public string DisplayName => "Tie list frames VM example";

        public AttrTableVm PumpVm { get; }

        public AttrTableVm ArinnaTurboThrottleVm { get; }
    }
}