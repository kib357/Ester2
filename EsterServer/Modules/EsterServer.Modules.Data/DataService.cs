using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using EsterCommon.Events;
using EsterCommon.PlanObjectTypes.Abstract;
using EsterCommon.Services;
using EsterServer.Model.Ioc;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;

namespace EsterServer.Modules.Data
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall)]
	public class DataService : IDataService
	{
	    private IUnityContainer _container;

        public DataService(IUnityContainer container)
        {
            _container = container;
        }

		public Stream Get()
		{
            var dataManager = _container.Resolve<DataManager>();
			return ConstructResponceStream(dataManager.RootObjects);
		}

		public Stream GetDevices()
		{
            var dataManager = _container.Resolve<DataManager>();
			return ConstructResponceStream(dataManager.GetDevices());
		}

		public Stream GetItem(string id)
		{
			return null;
		}

		public Stream Edit(string id, Stream stream)
		{
			var sr = new StreamReader(stream);
			var message = sr.ReadToEnd();
			var obj = JsonConvert.DeserializeObject<BaseObject>(message, new PlanObjectConverter());
            var dataManager = _container.Resolve<DataManager>();
			return ConstructResponceStream(dataManager.UpdateObjectProperties(obj));
		}

		private Stream ConstructResponceStream(object data)
		{
			var myResponseBody = JsonConvert.SerializeObject(data, Formatting.Indented);
			if (WebOperationContext.Current != null)
				WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
			return new MemoryStream(Encoding.UTF8.GetBytes(myResponseBody));
		}
	}
}
