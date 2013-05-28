using System.Collections.Generic;

namespace EsterServer.Modules.Data
{
	public delegate void DataProviderInitializedEventHandler(IDataProvider sender);
	public delegate void DataRecievedEventHandler(Dictionary<string, string> values);
	public delegate void DataWrittenEventHandler(Dictionary<string, string> values, bool success);

	public interface IDataProvider
	{
		event DataProviderInitializedEventHandler DataProviderInitializedEvent;
		event DataRecievedEventHandler DataRecievedEvent;
		event DataWrittenEventHandler DataWrittenEvent;
		DataProviderState State { get; }
		void Initialize(Dictionary<string, object> configuration);

		bool SaveValuesToNetwork(Dictionary<string, string> values);
		IEnumerable<NetworkDevice> GetDevices();

		void Stop();

		void UpdateSubscription(List<string> list);
	}

	public enum DataProviderState
	{
		Uninitialized,
		Initializing,
		Working,
		Fault
	}
}
