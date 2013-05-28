using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Microsoft.Practices.Unity;

namespace EsterServer.Model.Ioc
{
    public class UnityServiceHostFactory : ServiceHostFactory
    {
        private readonly IUnityContainer _container;
        private readonly Type _contractType;

        public UnityServiceHostFactory(IUnityContainer container, Type contractType)
        {
            _container = container;
            _contractType = contractType;
        }

        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            var host = new UnityServiceHost(_container, _contractType, serviceType, baseAddresses);
            //For DEBUG
            //var sdb = host.Description.Behaviors.Find<ServiceDebugBehavior>();
            //sdb.IncludeExceptionDetailInFaults = true;
            return host;
        }
    }
}