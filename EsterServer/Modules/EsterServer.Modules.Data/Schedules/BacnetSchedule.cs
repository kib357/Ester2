using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BACsharp;
using BACsharp.Types;
using BACsharp.Types.Constructed;
using BACsharp.Types.Primitive;
using EsterCommon.BaseClasses;

namespace EsterServer.Modules.Data.Schedules
{
	class BacnetSchedule
	{
		#region Read schedules
		public ScheduleClass ReadSchedule(uint deviceInstance, int objectAddress)
		{
			var res = new ScheduleClass { Days = new List<ValueTimeRange>[7] };

			res.Id = objectAddress;

			//var resName = BacNetServer.Network[deviceInstance].Objects["SCH" + objectAddress].Get(BacnetPropertyId.ObjectName);
			//if (resName != null)
			//	res.Name = resName.ToString();

			//var schedule = BacNetServer.Network[deviceInstance].Objects["SCH" + objectAddress].Get(BacnetPropertyId.WeeklySchedule) as List<BACnetDataType>;
			//if (schedule == null) return null;

			//var dayNumber = 0;
			//foreach (var dailySchedule in schedule)
			//{
			//	var daySchedule = new List<ValueTimeRange>();
			//	var day = dailySchedule as BACnetDailySchedule;
			//	if (day != null)
			//		foreach (var timeValue in day.Values)
			//		{
			//			var startTime = timeValue.Time.Value;
			//			double val;
			//			daySchedule.Add(double.TryParse(timeValue.Value.ToString(), out val)
			//								? new ValueTimeRange(startTime, val)
			//								: new ValueTimeRange(startTime, null));
			//		}

			//	res.Days[dayNumber] = daySchedule;
			//	dayNumber++;
			//}
			//res.ControlledObjects = GetControlledObjects(objectAddress);
			return res;
		}

		private List<string> GetControlledObjects(int objectAddress)
		{
			var res = new List<string>();
			//var scheduleDevices = BacNetServer.Network.SubscribedDevices.Where(d => d.ObjectList.Contains("SCH" + objectAddress));
			//foreach (var deviceWithSchedule in scheduleDevices)
			//{
			//	var controlledObjects = BacNetServer.Network[deviceWithSchedule.Id].Objects["SCH" + objectAddress].Get(BacnetPropertyId.ListOfObjectPropertyReferences);
			//	if (controlledObjects == null) return null;
			//	if (controlledObjects is BACnetDataType)
			//	{
			//		res.Add(ControlledObjectToString(deviceWithSchedule.Id, controlledObjects as BACnetDeviceObjectPropertyReference));
			//	}
			//	if (controlledObjects is List<BACnetDataType>)
			//	{
			//		foreach (var co in controlledObjects as List<BACnetDataType>)
			//		{
			//			res.Add(ControlledObjectToString(deviceWithSchedule.Id, co as BACnetDeviceObjectPropertyReference));
			//		}
			//	}
			//}

			return res;
		}

		#endregion

		#region Write schedules
		public bool WriteSchedule(uint instance, ScheduleClass schedule)
		{
			//var BACnetSchedule = GetBacNetWeeklySchedule(schedule);

			//var values = new Dictionary<string, Dictionary<BacnetPropertyId, object>>();
			//var val = new Dictionary<BacnetPropertyId, object>();
			//val.Add(BacnetPropertyId.WeeklySchedule, BACnetSchedule);
			//val.Add(BacnetPropertyId.ObjectName, schedule.Name);
			//if (schedule.ControlledObjects != null && schedule.ControlledObjects.Count > 0)
			//{
			//	var controlledObjects = schedule.ControlledObjects.Select(GetPropertyReferensFromString).ToList();
			//	val.Add(BacnetPropertyId.ListOfObjectPropertyReferences, controlledObjects);
			//}
			//values.Add("SCH" + schedule.Id, val);

			//return BacNetServer.Network[instance].WritePropertyMultiple(values);
			return false;
		}

		private List<BACnetDataType> GetBacNetWeeklySchedule(ScheduleClass schedule)
		{
			var res = new List<BACnetDataType>();
			for (int i = 0; i < 7; i++)
				res.Add(new BACnetDailySchedule());

			for (int i = 0; i < 7; i++)
			{
				var day = schedule.Days[i];
				var hourly = new List<BACnetTimeValue>();
				for (int j = 0; j < day.Count; j++)
				{
					if (j == 0 && day[j].Value == null) continue;
					BACnetPrimitiveDataType value;
					var startTime = day[j].Start;
					if (day[j].Value != null)
					{
						if (schedule.Id > ScheduleClass.MaxScudTypeNumber || schedule.Id < ScheduleClass.MinScudTypeNumber)
						{
							float floatValue;
							float.TryParse(day[j].Value.ToString(), out floatValue);
							value = new BACnetReal(floatValue);
						}
						else
						{
							int intValue;
							int.TryParse(day[j].Value.ToString(), out intValue);
							value = new BACnetEnumerated(intValue);
						}
					}
					else
						value = new BACnetNull();

					var start = new BACnetTimeValue { Time = new BACnetTime(startTime.Hour, startTime.Minute, startTime.Second, startTime.Millisecond / 10), Value = value };
					hourly.Add(start);
				}
				var daily = new BACnetDailySchedule(hourly);
				res[i] = daily;
			}
			return res;
		}

		private BACnetDeviceObjectPropertyReference GetPropertyReferensFromString(string obj)
		{
			var res = new BACnetDeviceObjectPropertyReference();
			string devAddr = string.Empty;
			string objAddress;
			int devAddress = 0;
			if (obj.Contains("."))
			{
				devAddr = obj.Split('.')[0];
				int.TryParse(devAddr, out devAddress);
				objAddress = obj.Split('.')[1];
			}
			else
				objAddress = obj;

			var objType = new Regex(@"[a-z\-A-Z]+").Match(objAddress).Value;
			var objNum = new Regex(@"[0-9]+").Match(objAddress).Value;

			int objNumber;
			int.TryParse(objNum, out objNumber);

			if (!string.IsNullOrWhiteSpace(devAddr))
				res.DeviceId = new BACnetObjectId((int)BacnetObjectType.Device, devAddress, 3);
			res.ObjectId.Instance = objNumber;
			res.PropertyId = new BACnetEnumerated((int)BacnetPropertyId.PresentValue, 1);

			objType = objType.ToUpper();
			switch (objType)
			{
				case "AI":
					res.ObjectId.ObjectType = (int)BacnetObjectType.AnalogInput;
					break;
				case "AO":
					res.ObjectId.ObjectType = (int)BacnetObjectType.AnalogOutput;
					break;
				case "AV":
					res.ObjectId.ObjectType = (int)BacnetObjectType.AnalogValue;
					break;
				case "BI":
					res.ObjectId.ObjectType = (int)BacnetObjectType.BinaryInput;
					break;
				case "BO":
					res.ObjectId.ObjectType = (int)BacnetObjectType.BinaryOutput;
					break;
				case "BV":
					res.ObjectId.ObjectType = (int)BacnetObjectType.BinaryValue;
					break;
				case "MI":
					res.ObjectId.ObjectType = (int)BacnetObjectType.MultiStateInput;
					break;
				case "MO":
					res.ObjectId.ObjectType = (int)BacnetObjectType.MultiStateOutput;
					break;
				case "MV":
					res.ObjectId.ObjectType = (int)BacnetObjectType.MultiStateValue;
					break;
			}
			return res;
		}

		#endregion

		#region Create/Delete Schedules
		public bool CreateSchedule(uint instance, int objectAddress, string name)
		{
			//var objName = new List<BACnetPropertyValue>
			//				  {
			//					  new BACnetPropertyValue((int) BacnetPropertyId.ObjectName,
			//											  new List<BACnetDataType> {new BACnetCharacterString(name)})
			//				  };
			//return BacNetServer.Network[instance].Objects["SCH" + objectAddress].Create(objName);
			return false;
		}

		public bool DeleteSchedule(uint instance, int objectAddress)
		{
			//return BacNetServer.Network[instance].Objects["SCH" + objectAddress].Delete();
			return false;
		}

		#endregion

		private string ControlledObjectToString(uint deviceAddress, BACnetDeviceObjectPropertyReference propertyReference)
		{
			var objId = new Regex(@"[a-z]+").Replace(propertyReference.ObjectId.ToString().Split('.')[0], "");
			if (propertyReference.DeviceId == null)
			{
				return deviceAddress + "." + objId + propertyReference.ObjectId.Instance;
			}
			return propertyReference.DeviceId.Instance + "." + objId + propertyReference.ObjectId.Instance;
		}
	}
}
