﻿using System.Windows.Input;
using MugenMvvmToolkit.Models;
using MugenMvvmToolkit.ViewModels;
using Prototype.Core.Models;
using Prototype.Core.Models.GasPanel;
using Prototype.Interfaces;

namespace Prototype.ViewModels.Pipes
{
    public class Manifold2Vm : ViewModelBase, IScreenViewModel
    {
        #region Constructors

        public Manifold2Vm()
        {
            ValveVm1 = new ValveVm("Valve1") {State = ValveState.Closed};
            ValveVm2 = new ValveVm("Valve2") {State = ValveState.Open};
            InvertValvesCommand = new RelayCommand(InvertValves);
        }

        #endregion

        #region Commands

        public ICommand InvertValvesCommand { get; }

        private void InvertValves()
        {
            foreach (var valve in new[] {ValveVm1, ValveVm2})
            {
                if (valve.State == ValveState.Open)
                {
                    valve.Close();
                }
                else
                {
                    valve.Open();
                }
            }
        }

        #endregion

        #region Properties

        public string DisplayName
        {
            get { return "Manifold 2"; }
        }

        public ValveVm ValveVm1 { get; }
        public ValveVm ValveVm2 { get; }

        public bool IsValve2Present
        {
            get => ValveVm2.IsPresent;
            set
            {
                if (ValveVm2.IsPresent == value)
                {
                    return;
                }

                ValveVm2.IsPresent = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}