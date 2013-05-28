using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using EsterCommon.PlanObjectTypes;
using EsterCommon.PlanObjectTypes.Abstract;
using EsterCommon.Services;
using EsterServer.Modules.Data.Notifications;
using Newtonsoft.Json;

namespace EsterServer.Modules.Data
{
	public class ValuesPusherComet
	{
		private static readonly Dictionary<string, ValuesPusher> ClientList = new Dictionary<string, ValuesPusher>();
		public static TimeSpan TimeOut = new TimeSpan(0, 0, 60);

		internal static void RegisterCometInstance(string clientId, ValuesPusher cometSvc)
		{
			lock (typeof(ValuesPusherComet))
			{
				ClientList[clientId] = cometSvc;
			}
		}

		internal static void UnregisterCometInstance(string clientId)
		{
			lock (typeof(ValuesPusherComet))
			{
				if (ClientList.ContainsKey(clientId))
					ClientList.Remove(clientId);
			}
		}

		public static void SetEvent(UpdateInfo info)
		{
			lock (typeof(ValuesPusherComet))
			{
				foreach (var valuesPusher in ClientList)
				{
					valuesPusher.Value.SetEvent(info);
				}
			}
		}
	}

	[ServiceContract(SessionMode = SessionMode.NotAllowed)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall)]
	public class ValuesPusher
	{
		private readonly AutoResetEvent _ev = new AutoResetEvent(false);
		private UpdateInfo _message = new UpdateInfo();

		//Ждем время, указанное в Comet.TimeOut, если в течении этого времени не был вызван метод Comet.SetEvent(clientId), то выдаем ошибку по тайм ауту
		[OperationContract]
		[WebGet(UriTemplate = "/subscribe", ResponseFormat = WebMessageFormat.Json)]
		public Stream GetChanges()
		{
			var arg = new UpdateInfo();
			string clientId = Guid.NewGuid().ToString();
			ValuesPusherComet.RegisterCometInstance(clientId, this);

			if (_ev.WaitOne(Comet.TimeOut))
			{
				lock (typeof(Comet))
				{
					arg = _message;
				}
			}

			Comet.UnregisterCometInstance(clientId);
			var myResponseBody = JsonConvert.SerializeObject(arg);
			WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
			return new MemoryStream(Encoding.UTF8.GetBytes(myResponseBody));
		}

		internal void SetEvent(UpdateInfo message)
		{
			_message = message;
			_ev.Set();
		}
	}
}
