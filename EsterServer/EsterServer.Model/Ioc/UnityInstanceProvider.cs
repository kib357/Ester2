using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Microsoft.Practices.Unity;

namespace EsterServer.Model.Ioc
{
    public class UnityInstanceProvider : IInstanceProvider
    {
        private readonly Type _serviceType;
        private readonly IUnityContainer _container;

        public UnityInstanceProvider(IUnityContainer container, Type serviceType)
        {
            _container = container;
            _serviceType = serviceType;
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return _container.Resolve(_serviceType);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
        }
    }
}