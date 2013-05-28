using System;
using System.Windows;
using Microsoft.Practices.Prism.ViewModel;

namespace EsterCommon.BaseClasses
{
	public class ValueTimeRange : NotificationObject
	{
		public ValueTimeRange()
		{

		}

		public ValueTimeRange(DateTime start, double? value)
		{
			Start = start;
			Value = value;
		}

		private double? _value;
		public double? Value
		{
			get { return _value; }
			set { _value = value; RaisePropertyChanged("Value"); }
		}

		private DateTime _start;
		public DateTime Start
		{
			get { return _start; }
			set
			{
				_start = value;
				RaisePropertyChanged("Start");
			}
		}

		private TimeSpan _length;
		public TimeSpan Length
		{
			get { return _length; }
			set
			{
				if (_length == value) return;
				_length = value;
				RaisePropertyChanged("Length");
			}
		}
	}
}
