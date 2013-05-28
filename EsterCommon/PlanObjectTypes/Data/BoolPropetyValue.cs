using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsterCommon.PlanObjectTypes.Data
{
	public class BoolPropertyValue : PropertyValue
	{
		private bool _boolValue;

		public bool BoolValue
		{
			get { return _boolValue; }
			set
			{
				if (value.Equals(_boolValue)) return;
				_boolValue = value;
				RaisePropertyChanged("BoolValue");
			}
		}

		public override void Update(double value)
		{
			base.Update(value);
			BoolValue = value == 1;
		}
	}
}
