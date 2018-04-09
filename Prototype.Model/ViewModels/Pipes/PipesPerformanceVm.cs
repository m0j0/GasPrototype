using System;
using System.Collections.Generic;
using MugenMvvmToolkit.ViewModels;
using Prototype.Core.Interfaces;
using Prototype.Core.Interfaces.GasPanel;
using Prototype.Core.Models;
using Prototype.Core.Models.GasPanel;
using Prototype.Interfaces;

namespace Prototype.ViewModels.Pipes
{
    public class PipesPerformanceVm : ViewModelBase, IScreenViewModel
    {
        private readonly ValveVm[] _valves;

        public PipesPerformanceVm()
        {
            var random = new Random();

            const int count = 500;
            _valves = new ValveVm[count];
            for (int i = 0; i < count; i++)
            {
                _valves[i] = new ValveVm
                {
                    State = random.Next() % 2 == 0 ? ValveState.Opened : ValveState.Closed,
                    IsPresent = random.Next() % 10 != 0
                };
            }
        }

        public IReadOnlyCollection<IValveVm> Valves
        {
            get { return _valves; }
        }

        public string DisplayName
        {
            get { return "Pipes performance"; }
        }
    }
}
