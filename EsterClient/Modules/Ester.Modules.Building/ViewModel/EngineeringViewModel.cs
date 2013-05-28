using System.Collections.Generic;
using Ester.Model.Enums;
using Ester.Model.Interfaces;
using Ester.Model.Repositories;
using Ester.Modules.Building.Model;
using EsterCommon.Data;
using EsterCommon.Enums;
using EsterCommon.PlanObjectTypes.Abstract;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Unity;
using System.Linq;
using System;

namespace Ester.Modules.Building.ViewModel
{
	public class EngineeringViewModel : NotificationObject, IEsterViewModel
	{
		private IUnityContainer _container;
		private IRegionManager _regionManager;
		private PlanObjectsRepository _planObjects;
		private readonly List<BaseObject> _systems;

		public event ViewModelConfiguredEventHandler ViewModelConfiguredEvent;
		public bool IsReady { get; private set; }
		public string Title { get; private set; }

		public List<BaseObject> Systems
		{
			get { return _systems; }
		}

		public DelegateCommand NavigateCommand { get; set; }

		public EngineeringViewModel(IUnityContainer container, IRegionManager manager)
		{
			_systems = new List<BaseObject>();

			_container = container;
			_regionManager = manager;
			_planObjects = container.Resolve<PlanObjectsRepository>();

			Title = "Инженерные системы";
			NavigateCommand = new DelegateCommand(() => _regionManager.RequestNavigate(RegionNames.MainRegion, new Uri(BuildingViewNames.BuildingView, UriKind.Relative)));
		}

		public void Configure()
		{
			_planObjects.DataUpdatedEvent += PrepareEngineeringData;
			PrepareEngineeringData(null);
			IsReady = true;
			if (ViewModelConfiguredEvent != null)
				ViewModelConfiguredEvent(this);
		}

		private void PrepareEngineeringData(Repository repo)
		{
			_systems.Clear();
			_systems.AddRange(_planObjects.PlanObjects.Values.Select(m => new Microclimate()));
			_systems.AddRange(_planObjects.PlanObjects.Values.Where(v => v.TypeId == (int)ObjectTypes.DHSPlan));
			_systems.AddRange(_planObjects.PlanObjects.Values.Where(v => v.TypeId == (int)ObjectTypes.ElectricalGridPlan));
			_systems.AddRange(_planObjects.PlanObjects.Values.Where(v => v.TypeId == (int)ObjectTypes.VentilationPlan));
		}
	}
}
