using MugenMvvmToolkit;
using MugenMvvmToolkit.Interfaces.Models;
using MugenMvvmToolkit.ViewModels;
using Prototype.Interfaces;

namespace Prototype.ViewModels
{
    public class CompositeParentVm : CloseableViewModel, IHasDisplayName
    {
        #region Fields

        private readonly CompositeNestedVm _firstNestedVm;
        private readonly CompositeNestedVm _secondNestedVm;
        private readonly CompositeNestedVm _thirdNestedVm;

        #endregion

        #region Constructors

        public CompositeParentVm(IParameterProvider parameterProvider)
        {
            Should.NotBeNull(parameterProvider, "parameterProvider");

            _firstNestedVm = GetViewModel<CompositeNestedVm>();
            _firstNestedVm.DisplayName = "First nested view model. Parameter: " + parameterProvider.Parameter;

            _secondNestedVm = GetViewModel(container => new CompositeNestedVm
            {
                DisplayName = "Second nested view model. Parameter: " + parameterProvider.Parameter
            });

            _thirdNestedVm = new CompositeNestedVm
            {
                DisplayName = "Third nested view model. Parameter: " + parameterProvider.Parameter
            };
            this.InitializeChildViewModel(_thirdNestedVm);
        }

        #endregion

        #region Properties

        public string DisplayName
        {
            get { return "Composite view model"; }
        }

        public CompositeNestedVm FirstNestedVm
        {
            get { return _firstNestedVm; }
        }

        public CompositeNestedVm SecondNestedVm
        {
            get { return _secondNestedVm; }
        }

        public CompositeNestedVm ThirdNestedVm
        {
            get { return _thirdNestedVm; }
        }

        #endregion
    }
}
