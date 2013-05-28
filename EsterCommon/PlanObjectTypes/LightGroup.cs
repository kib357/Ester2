using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using EsterCommon.Enums;
using EsterCommon.PlanObjectTypes.Abstract;
using EsterCommon.PlanObjectTypes.Interfaces;

namespace EsterCommon.PlanObjectTypes
{
	public class LightGroup : Subsystem, IContainerObject
	{
		private ObservableCollection<BaseObject> _children;

		private bool _isOn;
		private int _lightLevel;
		private int _lightLevelSetpoint;

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
		public int LightLevelSetpoint
		{
			get { return _lightLevelSetpoint; }
			set
			{
				if (value == _lightLevelSetpoint) return;
				_lightLevelSetpoint = value;
				RaisePropertyChanged("LightLevelSetpoint");
			}
		}

		public ObservableCollection<BaseObject> Children
		{
			get { return _children; }
			private set { _children = value; RaisePropertyChanged("Children"); }
		}

		public LightGroup()
		{
			_children = new ObservableCollection<BaseObject>();
		}

		public override void Update(BaseObject newObject)
		{
			base.Update(newObject);
			IsOn = ((LightGroup)newObject).IsOn;
			LightLevel = ((LightGroup)newObject).LightLevel;
			LightLevelSetpoint = ((LightGroup)newObject).LightLevelSetpoint;
		}

		public override List<Tuple<PropertyTypes, string>> GetChanges(BaseObject newObj)
		{
			var changes = new List<Tuple<PropertyTypes, string>>();

			var value = newObj as LightGroup;
			if (value == null)
				throw new ArgumentException("Типы объектов не совпадают", "newObj");

			if (IsOn != value.IsOn)
				changes.Add(new Tuple<PropertyTypes, string>(PropertyTypes.LightGroup_State, value.IsOn.ToString()));

			if (LightLevelSetpoint != value.LightLevelSetpoint)
				changes.Add(new Tuple<PropertyTypes, string>(PropertyTypes.LightGroup_Setpoint, value.LightLevelSetpoint.ToString()));

			return changes;
		}
	}
}
