using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EsterServer.Modules.Data
{
	public delegate void FakeValueChangedEventHandler(Dictionary<string, string> values);
	public class FakeObject
	{
		private readonly Random _random = new Random(DateTime.Now.Millisecond);
		public event FakeValueChangedEventHandler FakeValueChangedEvent;
		protected virtual void OnFakeValueChangedEvent(Dictionary<string, string> values)
		{
			FakeValueChangedEventHandler handler = FakeValueChangedEvent;
			if (handler != null) handler(values);
		}

		public string Address { get; set; }
        public string Value { get; set; }
	    public double MinValue { get; set; }
		public double MaxValue { get; set; }
		public TimeSpan ValueChangeInterval { get; set; }
		public double MinAlarmValue { get; set; }
		public double MaxAlarmValue { get; set; }
		public int AlarmValueInterval { get; set; }
        public bool IsWriteable { get; set; }
		private volatile bool _generateValues;
		private int _generatedValueCount;

	    public void Start()
		{
	        _generateValues = true;
			_generatedValueCount = 0;
			Task.Factory.StartNew(GenerateValues, TaskCreationOptions.LongRunning);
		}

		public void Stop()
		{
			_generateValues = false;
		}

		private void GenerateValues()
		{
			while (_generateValues)
			{
                if (IsWriteable)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                    OnFakeValueChangedEvent(new Dictionary<string, string> { { Address, Value } });
                    _generateValues = false;
                    return;
                }
				if (AlarmValueInterval == 0 || AlarmValueInterval != _generatedValueCount)
				{
                    Value = (MinValue + (MaxValue - MinValue) * _random.NextDouble()).ToString();
					_generatedValueCount++;
				}
				else
				{
                    Value = (MinAlarmValue + (MaxAlarmValue - MinAlarmValue) * _random.NextDouble()).ToString();
					_generatedValueCount = 0;
				}
				OnFakeValueChangedEvent(new Dictionary<string, string> { { Address, Value } });
				Thread.Sleep(ValueChangeInterval + new TimeSpan((long)(_random.NextDouble() * 0.15 * ValueChangeInterval.Milliseconds)));
			}
		}
	}
}