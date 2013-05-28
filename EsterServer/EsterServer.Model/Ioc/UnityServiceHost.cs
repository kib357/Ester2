using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Microsoft.Practices.Unity;

namespace EsterServer.Model.Ioc
{
    public class UnityServiceHost : ServiceHost
    {
        private readonly IUnityContainer _container;

        public UnityServiceHost(IUnityContainer container, Type contractType, Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            _container = container;
            AddServiceEndpoint(contractType, new WebHttpBinding(), "")
                .Behaviors.Add(new WebHttpBehavior());
        }

        protected override void OnOpening()
        {
            if (Description.Behaviors.Find<UnityServiceBehavior>() == null)
            {
                Description.Behaviors.Add(new UnityServiceBehavior(_container));
            }
            base.OnOpening();            
        }
    }
}