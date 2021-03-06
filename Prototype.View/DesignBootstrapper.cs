﻿using System;
using System.Collections.Generic;
using System.Reflection;
using MugenMvvmToolkit;
using MugenMvvmToolkit.Interfaces;
using MugenMvvmToolkit.Interfaces.ViewModels;
using MugenMvvmToolkit.ViewModels;
using MugenMvvmToolkit.WPF.Infrastructure;
using Prototype.ViewModels;
using Prototype.ViewModels.Pipes;

namespace Prototype
{
    public class DesignBootstrapper : WpfDesignBootstrapperBase
    {
        #region Nested types

        private sealed class DefaultApp : MvvmApplication
        {
            #region Methods

            public override Type GetStartViewModelType()
            {
                return typeof(MainVm);
            }

            #endregion
        }

        #endregion

        #region Methods

        protected override IMvvmApplication CreateApplication()
        {
            return new DefaultApp();
        }

        protected override IIocContainer CreateIocContainer()
        {
            return new MugenContainer();
        }
        
        protected override void UpdateAssemblies(HashSet<Assembly> assemblies)
        {
            assemblies.Add(typeof(DesignBootstrapper).Assembly);
            assemblies.Add(typeof(MainVm).Assembly);
            assemblies.Add(typeof(MugenContainer).Assembly);
            assemblies.Add(typeof(WpfDesignBootstrapperBase).Assembly);
        }

        private static DesignBootstrapper CreateInstance()
        {
            if (!ServiceProvider.IsDesignMode)
            {
                return null;
            }
            var boot = new DesignBootstrapper();
            boot.Initialize();
            return boot;
        }

        #endregion

        #region Properties

        public static MainVm MainVm
        {
            get { return CreateInstance().GetDesignViewModel(provider => provider.GetViewModel<MainVm>()); }
        }

        public static DisplayWrapperVm<IViewModel> DisplayWrapperVm
        {
            get { return CreateInstance().GetDesignViewModel(provider => provider.GetViewModel<DisplayWrapperVm<IViewModel>>()); }
        }

        public static CompositeParentVm CompositeParentVm
        {
            get { return CreateInstance().GetDesignViewModel(provider => provider.GetViewModel<CompositeParentVm>()); }
        }

        public static CfmApplicationVm CfmApplicationVm
        {
            get { return CreateInstance().GetDesignViewModel(provider => provider.GetViewModel<CfmApplicationVm>()); }
        }

        public static PipesExampleVm PipesExampleVm
        {
            get { return CreateInstance().GetDesignViewModel(provider => provider.GetViewModel<PipesExampleVm>()); }
        }

        public static PipesPerformanceVm PipesPerformanceVm
        {
            get { return CreateInstance().GetDesignViewModel(provider => provider.GetViewModel<PipesPerformanceVm>()); }
        }

        public static PipesScalingVm PipesScalingVm
        {
            get { return CreateInstance().GetDesignViewModel(provider => provider.GetViewModel<PipesScalingVm>()); }
        }

        public static ManifoldVm ManifoldVm
        {
            get { return CreateInstance().GetDesignViewModel(provider => provider.GetViewModel<ManifoldVm>()); }
        }

        public static Manifold2Vm Manifold2Vm
        {
            get { return CreateInstance().GetDesignViewModel(provider => provider.GetViewModel<Manifold2Vm>()); }
        }

        public static Manifold3Vm Manifold3Vm
        {
            get { return CreateInstance().GetDesignViewModel(provider => provider.GetViewModel<Manifold3Vm>()); }
        }

        public static Manifold4Vm Manifold4Vm
        {
            get { return CreateInstance().GetDesignViewModel(provider => provider.GetViewModel<Manifold4Vm>()); }
        }
        
        public static PipesConnectionsVm PipesConnectionsVm
        {
            get { return CreateInstance().GetDesignViewModel(provider => provider.GetViewModel<PipesConnectionsVm>()); }
        }

        public static Valve3WayExampleVm Valve3WayExampleVm
        {
            get { return CreateInstance().GetDesignViewModel(provider => provider.GetViewModel<Valve3WayExampleVm>()); }
        }

        public static NestedParentVm NestedParentVm
        {
            get { return CreateInstance().GetDesignViewModel(provider => provider.GetViewModel<NestedParentVm>()); }
        }

        #endregion
    }
}
