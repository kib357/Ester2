using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using EsterServer.Model.Interfaces;
using EsterServer.Model.Ioc;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;

namespace EsterServer.Modules.TestPlugin
{
    [Module(ModuleName = "TestPlugin")]
    public class TestPlugin : IModule
    {
        private IUnityContainer _container;
        public TestPlugin(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            _container.RegisterType<IDataProvider, YaDataProvider>("YaDataProvider");
            _container.RegisterType<IRestService, YaWebService>("YaWebService");
            var s = new YaWebService(_container) {Number = 42};
            _container.RegisterInstance(typeof(YaWebService), s);            
            RouteTable.Routes.Add(new ServiceRoute("ya", new UnityServiceHostFactory(_container, typeof(IYaWebService)), typeof(YaWebService)));
        }
    }
}
