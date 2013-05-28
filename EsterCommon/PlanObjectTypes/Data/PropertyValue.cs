using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.ViewModel;

namespace EsterCommon.PlanObjectTypes.Data
{
	public class PropertyValue : NotificationObject
	{
		private double _value;
		private bool _present;
		private DateTime _updated;
		private object _alarm;

		public double Value
		{
			get { return _value; }
			set
			{
				if (value.Equals(_value)) return;
				_value = value;
				RaisePropertyChanged("Value");
			}
		}

		public bool Present
		{
			get { return _present; }
			set
			{
				if (value.Equals(_present)) return;
				_present = value;
				RaisePropertyChanged("Present");
			}
		}

		public DateTime Updated
		{
			get { return _updated; }
			set
			{
				if (value.Equals(_updated)) return;
				_updated = value;
				RaisePropertyChanged("Updated");
			}
		}

		public object Alarm
		{
			get { return _alarm; }
			set
			{
				if (Equals(value, _alarm)) return;
				_alarm = value;
				RaisePropertyChanged("Alarm");
			}
		}

		public virtual void Update(double value)
		{
			Value = value;
			Updated = DateTime.Now;
			Present = true;
		}
	}
}
