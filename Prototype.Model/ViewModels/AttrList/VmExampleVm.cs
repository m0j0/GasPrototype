using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MugenMvvmToolkit.Interfaces.Models;
using MugenMvvmToolkit.ViewModels;
using Prototype.Core.Models.AttrList;
using Prototype.Interfaces;

namespace Prototype.ViewModels.AttrList
{
    public class VmExampleVm : CloseableViewModel, IScreenViewModel
    {
        public VmExampleVm()
        {
            PumpVm = new AttrTableVm
            {
                Title = "Pump",
                Attrs =
                {
                    new AttrItemModel("Status", "rStatus"),
                    new AttrItemModel("Command", "wCommand", true),
                    new AttrItemModel("N2 flow low", "sN2FlowLow"),
                    new AttrItemModel("Pump fault", "sFault"),
                    new AttrItemModel("Pump warning", "sWarning"),
                    new AttrItemModel("Pump running", "sPumpOn")
                }
            };

            ArinnaTurboThrottleVm = new AttrTableVm
            {
                Title = "Arinna turbo throttle valve",
                Attrs =
                {
                    new AttrItemModel("Pressure", "rPressure"),
                    new AttrItemModel("Pressure setpoint", "wSetpointPressure", true),
                    new AttrItemModel("Ramp rate", "wPressRampRate"),
                    new AttrItemModel("Position", "rPosition", true),
                    new AttrItemModel("Position setpoint", "wSetpointPosition", true)
                }
            };

            // TODO init children
        }

        public string DisplayName => "Tie list frames VM example";

        public AttrTableVm PumpVm { get; }

        public AttrTableVm ArinnaTurboThrottleVm { get; }
    }
}