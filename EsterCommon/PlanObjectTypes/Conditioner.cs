using System;
using EsterCommon.PlanObjectTypes.Abstract;

namespace EsterCommon.PlanObjectTypes
{
	public class Conditioner : Subsystem
	{
		private int _fanLevel;
		private bool _isOn;

		public int FanLevel
		{
			get { return _fanLevel; }
			set
			{
				if (value == _fanLevel) return;
				_fanLevel = value;
				RaisePropertyChanged("FanLevel");
			}
		}
		public bool IsOn
		{
			get { return _isOn; }
			set
			{
				if (value.Equals(_isOn)) return;
				_isOn = value;
				RaisePropertyChanged("IsOn");
			}
		}

		public override void Update(BaseObject newObject)
		{
			base.Update(newObject);
			FanLevel = ((Conditioner)newObject).FanLevel;
			IsOn = ((Conditioner)newObject).IsOn;
		}
	}
}
