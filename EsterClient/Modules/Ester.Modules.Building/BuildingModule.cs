using System;
using Ester.Model.Enums;
using Ester.Model.Interfaces;
using Ester.Modules.Building.View;
using Ester.Modules.Building.ViewModel;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace Ester.Modules.Building
{
	[Module(ModuleName = "BuildingModule")]
	public class BuildingModule : IModule
	{
		private readonly IUnityContainer _container;
		private readonly IRegionManager _regionManager;

		public BuildingModule(IUnityContainer container, IRegionManager manager)
		{
			_container = container;
			_regionManager = manager;

			_container.RegisterInstance(_container.Resolve<BuildingViewModel>());
			_container.RegisterInstance<IEsterViewModel>("building", _container.Resolve<BuildingViewModel>());
		}

		#region IModule Members

		/// <summary>
		/// Инициализация представления и его модели представления
		/// </summary>
		public void Initialize()
		{
			var engineeringViewModel = _container.Resolve<EngineeringViewModel>();
			var engineeringView = _container.Resolve<EngineeringView>();

			_container.RegisterInstance(BuildingViewNames.EngineeringView, engineeringView);
			_container.RegisterType<object, EngineeringView>(BuildingViewNames.EngineeringView);

			engineeringView.DataContext = engineeringViewModel;
			_regionManager.AddToRegion(RegionNames.ModulesRegion, engineeringView);

			var buildingViewModel = _container.Resolve<BuildingViewModel>();
			var buildingView = _container.Resolve<BuildingView>();

			_container.RegisterInstance(BuildingViewNames.BuildingView, buildingView);
			_container.RegisterType<object, BuildingView>(BuildingViewNames.BuildingView);

			buildingView.DataContext = buildingViewModel;
		}

		#endregion
	}
}
