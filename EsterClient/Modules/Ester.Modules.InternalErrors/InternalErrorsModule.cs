using Ester.Model;
using Ester.Model.Enums;
using Ester.Modules.InternalErrors.View;
using Ester.Modules.InternalErrors.ViewModel;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace Ester.Modules.InternalErrors
{
    public class InternalErrorsModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public InternalErrorsModule(IUnityContainer container, IRegionManager manager)
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
            var viewModel = _container.Resolve<InternalErrorsViewModel>();
            var myView = _container.Resolve<InternalErrorsView>();
            myView.DataContext = viewModel;
            _regionManager.Regions[RegionNames.ErrorsRegion].Add(myView, "ErrorsView");
            _regionManager.Regions[RegionNames.ErrorsRegion].Deactivate(myView);
        }

        #endregion
    }
}
