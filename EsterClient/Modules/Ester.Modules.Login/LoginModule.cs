using Ester.Model;
using Ester.Model.Enums;
using Ester.Modules.Login.ViewModel;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace Ester.Modules.Login
{
	[Module(ModuleName = "LoginModule")]
    public class LoginModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public LoginModule(IUnityContainer container, IRegionManager manager)
        {
            _container = container;
            _regionManager = manager;
        }

        #region IModule Members

        /// <summary>
        /// Инициализация представления и его модели представления
        /// </summary>
        public void Initialize()
        {
            var viewModel = _container.Resolve<LoginViewModel>();
            var myView = _container.Resolve<LoginView>();
            myView.DataContext = viewModel;
            _regionManager.Regions[RegionNames.LoginRegion].Add(myView, "LoginView");
            _regionManager.Regions[RegionNames.LoginRegion].Activate(myView);
        }

        #endregion
    }
}