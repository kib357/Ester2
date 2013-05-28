using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ester.Model;
using Ester.Model.Enums;
using Ester.Model.Events;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Ester.Modules.Users.Model;
using Ester.Modules.Users.View;
using Ester.Modules.Users.ViewModel;

namespace Ester.Modules.Users
{
    public class UsersModule: IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public UsersModule(IUnityContainer container, IRegionManager manager)
        {
            _container = container;
            _container.RegisterType<IUsersRepository, AdFakeRepository>();
            _regionManager = manager;
        }

        #region IModule Members

        /// <summary>
        /// Инициализация представления и его модели представления
        /// </summary>
        public void Initialize()
        {          
            var viewModel = _container.Resolve<UsersViewModel>();
            var view = _container.Resolve<ImportAdUsersView>();
            view.DataContext = viewModel;         

            _container.Resolve<EventAggregator>().GetEvent<ShowAdUsersImportEvent>().Subscribe(OnShowAdUsers);

            _regionManager.Regions[RegionNames.PopupRegion].Add(view, "ImportAdUsersView");
            _regionManager.Regions[RegionNames.PopupRegion].Deactivate(view);
        }

        private void OnShowAdUsers(object obj)
        {
            _regionManager.Regions[RegionNames.PopupRegion].Activate("ImportAdUsersView");
        }

        #endregion
    }
}
