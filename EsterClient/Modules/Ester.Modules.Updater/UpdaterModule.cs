using Ester.Model.Enums;
using Ester.Model.Interfaces;
using Ester.Modules.Updater.ViewModel;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace Ester.Modules.Updater
{
	public class UpdaterModule : IModule
	{
		private readonly IUnityContainer _container;
		private readonly IRegionManager _regionManager;

		public UpdaterModule(IUnityContainer container, IRegionManager manager)
		{
			_container = container;
			_regionManager = manager;
			_container.RegisterInstance(_container.Resolve<UpdaterViewModel>());
			_container.RegisterInstance<IEsterViewModel>("updates", _container.Resolve<UpdaterViewModel>());
		}

		public void Initialize()
		{
			var viewModel = _container.Resolve<UpdaterViewModel>();
			var myView = _container.Resolve<View.UpdaterView>();
			myView.DataContext = viewModel;
			_regionManager.Regions[RegionNames.UpdaterRegion].Add(myView, "UpdaterView");
		}
	}
}
