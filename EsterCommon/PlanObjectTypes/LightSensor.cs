using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EsterCommon.PlanObjectTypes.Abstract;
using EsterCommon.PlanObjectTypes.Data;

namespace EsterCommon.PlanObjectTypes
{
	public class LightSensor : Subsystem
	{
		private PropertyValue _lightLevel = new PropertyValue();
		public PropertyValue LightLevel
		{
			get { return _lightLevel; }
			set
			{
				if (value == _lightLevel) return;
				_lightLevel = value;
				RaisePropertyChanged("LightLevel");
			}
		}

		public override void Update(BaseObject newObject)
		{
			base.Update(newObject);
			LightLevel = ((LightSensor)newObject).LightLevel;
		}
	}
}
