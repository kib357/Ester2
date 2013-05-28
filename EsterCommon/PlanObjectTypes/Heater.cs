using EsterCommon.PlanObjectTypes.Abstract;

namespace EsterCommon.PlanObjectTypes
{
	public class Heater : Subsystem
	{
		private double _manualLevel;
		private bool _isBacstatAllowed;

		public double ManualLevel
		{
			get { return _manualLevel; }
			set
			{
				if (value.Equals(_manualLevel)) return;
				_manualLevel = value;
				RaisePropertyChanged("ManualLevel");
			}
		}
		public bool IsBacstatAllowed
		{
			get { return _isBacstatAllowed; }
			set
			{
				if (value.Equals(_isBacstatAllowed)) return;
				_isBacstatAllowed = value;
				RaisePropertyChanged("IsBacstatAllowed");
			}
		}

		public override void Update(BaseObject newObject)
		{
			base.Update(newObject);
			ManualLevel = ((Heater)newObject).ManualLevel;
			IsBacstatAllowed = ((Heater)newObject).IsBacstatAllowed;
		}
	}
}
