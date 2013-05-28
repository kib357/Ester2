using System;
using System.ComponentModel;

namespace EsterCommon.Enums
{
	[Description("������������� ����������")]
	public enum ScheduleTypes
	{
		[Description("����")]
		SKUD,
		[Description("���������")]
		Light,
		[Description("���������")]
		Heat,
		[Description("�����������")]
		Ventilation,
		[Description("�����������������")]
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
