using System;
using System.ComponentModel;

namespace EsterCommon.Enums
{
	[Description("Разновидности расписаний")]
	public enum ScheduleTypes
	{
		[Description("СКУД")]
		SKUD,
		[Description("Освещение")]
		Light,
		[Description("Отопление")]
		Heat,
		[Description("Вентилляция")]
		Ventilation,
		[Description("Кондиционирование")]
		AC
	}

	public static class Ext
	{
		public static double DefaultValue(this ScheduleTypes val)
		{
			switch (val)
			{
				case ScheduleTypes.SKUD:
					return 0.0;
				case ScheduleTypes.Light:
					return 0;
				case ScheduleTypes.Heat:
					return 21.0;
				case ScheduleTypes.Ventilation:
					return 0;
				case ScheduleTypes.AC:
					return 0;
				default:
					throw new ArgumentOutOfRangeException("val");
			}
		}
	}
}
