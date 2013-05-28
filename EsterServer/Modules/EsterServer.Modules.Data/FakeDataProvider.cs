using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace EsterServer.Modules.Data
{
	public class FakeDataProvider : IDataProvider
	{
		private readonly List<FakeObject> _fakeObjects = new List<FakeObject>();

		public event DataProviderInitializedEventHandler DataProviderInitializedEvent;
		public event DataRecievedEventHandler DataRecievedEvent;

		protected virtual void OnDataRecievedEvent(Dictionary<string, string> values)
		{
			DataRecievedEventHandler handler = DataRecievedEvent;
			if (handler != null) handler(values);
		}

		public event DataWrittenEventHandler DataWrittenEvent;

		private DataProviderState _state;
	    public DataProviderState State
	    {
	        get { return _state; }
	        private set
	        {
	            _state = value;
	            Debug.WriteLine("FakeState: " + _state.ToString());
	        }
	    }

	    public FakeDataProvider()
		{
			State = DataProviderState.Uninitialized;
		}

		public void Initialize(Dictionary<string, object> configuration)
		{
			State = DataProviderState.Initializing;
			try
			{
				var addresses = (List<string>)configuration["Addresses"];
                _fakeObjects.Clear();
				addresses.ForEach(Subscribe);
				State = DataProviderState.Working;
				if (DataProviderInitializedEvent != null)
					DataProviderInitializedEvent(this);
			}
			catch (Exception)
			{
				State = DataProviderState.Fault;
			}
		}

		private void OnFakeValueChanged(Dictionary<string, string> values)
		{
			OnDataRecievedEvent(values);
		}

		public FakeObject ParseAddress(string address)
		{
			if (!address.StartsWith("F;"))
				return null;
			var fakeObject = new FakeObject { Address = address };
			foreach (var addressPart in address.ToLower().Split(';'))
			{
				var partValues = GetAddressPartValues(addressPart);
				if (addressPart.StartsWith("d") && partValues.Count == 3)
				{
					fakeObject.MinValue = partValues[0];
					fakeObject.MaxValue = partValues[1];
					fakeObject.ValueChangeInterval = TimeSpan.FromSeconds(partValues[2]);
				}
				if (addressPart.StartsWith("a") && partValues.Count == 3)
				{
					fakeObject.MinAlarmValue = partValues[0];
					fakeObject.MaxAlarmValue = partValues[1];
					fakeObject.AlarmValueInterval = (int)partValues[2];
				}
                if (addressPart.StartsWith("w") && partValues.Count == 1)
                {
                    fakeObject.IsWriteable = true;
                    fakeObject.Value = partValues[0].ToString();
                }
			}
			return fakeObject;
		}

		private List<double> GetAddressPartValues(string part)
		{
			var res = new List<double>();
			var firstIndex = part.LastIndexOf('(') + 1;
			var lastIndex = part.LastIndexOf(')');
			if (firstIndex <= 0 || lastIndex < 0 || firstIndex >= lastIndex)
				return res;
			part = part.Remove(lastIndex).Substring(firstIndex);
			var values = part.Split(',');
			foreach (var value in values)
			{
				double v;
				if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out v))
					res.Add(v);
			}
			return res;
		}

		public bool SaveValuesToNetwork(Dictionary<string, string> values)
		{
		    foreach (var value in values)
		    {
		        var fakeObject = ParseAddress(value.Key);
                if (fakeObject != null && fakeObject.IsWriteable)
                {
                    var oldObject = _fakeObjects.FirstOrDefault(f => f.Address == fakeObject.Address);
                    if (oldObject == null) return false;
                    oldObject.Value = value.Value;
                    OnFakeValueChanged(values);
                    return true;
                }
		    }
		    return false;
		}

		public IEnumerable<NetworkDevice> GetDevices()
		{
			return new List<NetworkDevice>() { new NetworkDevice { Id = 0, Title = "FakeDevice" } };
		}

		public void Stop()
		{
			foreach (var fakeObject in _fakeObjects)
			{
				fakeObject.Stop();
			}
		}

		public void UpdateSubscription(List<string> list)
		{
			_fakeObjects.ForEach(Unsubscribe);
            _fakeObjects.Clear();
			list.ForEach(Subscribe);
		}

		private void Subscribe(string address)
		{
			if (!address.StartsWith("F;")) return;
			if (_fakeObjects.Any(a => a.Address == address)) return;

			var fa = ParseAddress(address);
			if (fa == null) return;

			_fakeObjects.Add(fa);
			fa.FakeValueChangedEvent += OnFakeValueChanged;
			fa.Start();
            Debug.WriteLine("subscribed to " + fa.Address);
		}

		private void Unsubscribe(FakeObject fake)
		{
			fake.Stop();
			fake.FakeValueChangedEvent -= OnFakeValueChanged;
            Debug.WriteLine("unsubscribed from " + fake.Address);
		}
	}
}
