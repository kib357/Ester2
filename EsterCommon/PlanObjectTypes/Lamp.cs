using EsterCommon.PlanObjectTypes.Abstract;

namespace EsterCommon.PlanObjectTypes
{
	public class Lamp : Subsystem
	{
		private int _lightLevel;

		public int LightLevel
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
			LightLevel = ((Lamp)newObject).LightLevel;
		}
	}
}
