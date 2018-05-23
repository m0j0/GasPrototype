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
    public class CbExampleVm : CloseableViewModel, IScreenViewModel
    {
        public string DisplayName => "Tie list frames code-behind example";
    }
}