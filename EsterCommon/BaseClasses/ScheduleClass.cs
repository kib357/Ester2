using System.Collections.Generic;
using System.Collections.ObjectModel;
using EsterCommon.Data;
using EsterCommon.Enums;
using Microsoft.Practices.Prism.ViewModel;

namespace EsterCommon.BaseClasses
{
	public class ScheduleClass : NotificationObject
	{
		public override string ToString()
		{
			return Name ?? base.ToString();
		}

		public ScheduleClass(int id, string name, List<ValueTimeRange>[] items)
		{
			Id = id;
			Name = name;
			Days = items;
		}

		public ScheduleClass()
		{
		}

		public ScheduleClass(bool initEmpty)
		{
			Name = "Не используется";
		}

		public bool OverrideController { get; set; }
		public bool DeleteOnSync { get; set; }

		private int _id;
		public int Id
		{
			get { return _id; }
			set
			{
				_id = value;
				RaisePropertyChanged("Id");
			}
		}

		public int? NullableId
		{
			get { return Id > 0 ? Id : (int?)null; }
		}

		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				RaisePropertyChanged("Name");
			}
		}

	    public ScheduleTypes Type
		{
			get { return GetScheduleType(Id); }
		}

		public void SetType(ScheduleTypes scheduleType)
		{
			if (scheduleType == ScheduleTypes.SKUD) Id = (int)MaxScudTypeNumber;
			if (scheduleType == ScheduleTypes.Heat) Id = (int)MaxHeatTypeNumber;
			if (scheduleType == ScheduleTypes.Ventilation) Id = (int)MaxVentialtionTypeNumber;
			if (scheduleType == ScheduleTypes.Light) Id = (int)MaxLightTypeNumber;
			if (scheduleType == ScheduleTypes.AC) Id = (int)MaxACTypeNumber;
		}

		private ScheduleTypes GetScheduleType(int objNum)
		{
			if (objNum >= MinScudTypeNumber && objNum <= MaxScudTypeNumber)
				return ScheduleTypes.SKUD;
			if (objNum >= MinHeatTypeNumber && objNum <= MaxHeatTypeNumber)
				return ScheduleTypes.Heat;
			if (objNum >= MinVentialtionTypeNumber && objNum <= MaxVentialtionTypeNumber)
				return ScheduleTypes.Ventilation;
			if (objNum >= MinLightTypeNumber && objNum <= MaxLightTypeNumber)
				return ScheduleTypes.Light;
			if (objNum >= MinACTypeNumber && objNum <= MaxACTypeNumber)
				return ScheduleTypes.AC;
			return new ScheduleTypes();
		}

		private List<ValueTimeRange>[] _days;
		public List<ValueTimeRange>[] Days
		{
			get { return _days; }
			set
			{
				_days = value;
				RaisePropertyChanged("Days");
			}
		}

		private List<Property> _controlledObjects = new List<Property>();

		public List<Property> ControlledObjects
		{
			get { return _controlledObjects; }
			set { _controlledObjects = value; }
		}

		public List<uint> Controllers
		{
			get
			{
				var res = new List<uint>();
				foreach (var controlledObject in _controlledObjects)
				{
					uint device;
					if (uint.TryParse(controlledObject.Path.Split('.')[0], out device) && !res.Contains(device))
						res.Add(device);
				}
				return res;
			}
		}

		public const uint MinScudTypeNumber = 1;
		public const uint MaxScudTypeNumber = 100;
		public const uint MinHeatTypeNumber = 101;
		public const uint MaxHeatTypeNumber = 200;
		public const uint MinVentialtionTypeNumber = 201;
		public const uint MaxVentialtionTypeNumber = 300;
		public const uint MinLightTypeNumber = 301;
		public const uint MaxLightTypeNumber = 400;
		public const uint MinACTypeNumber = 401;
		public const uint MaxACTypeNumber = 500;
	}
}
