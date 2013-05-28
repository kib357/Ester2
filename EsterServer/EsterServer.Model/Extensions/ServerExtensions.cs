using System;
using System.Linq;
using EsterCommon.Data;
using EsterCommon.Enums;
using EsterCommon.Exceptions;
using EsterCommon.PlanObjectTypes;
using EsterCommon.PlanObjectTypes.Abstract;
using EsterCommon.PlanObjectTypes.Data;
using EsterCommon.PlanObjectTypes.Interfaces;
using EsterCommon.Services;

namespace EsterServer.Model.Extensions
{
	public static class ServerExtensions
	{
		public static void Update(this BaseObject baseObject, PropertyTypes addressType, double doubleValue)
		{
			switch (addressType)
			{
				case PropertyTypes.LightGroup_Setpoint:
					((LightGroup)baseObject).LightLevelSetpoint = (int)doubleValue;
					break;
				case PropertyTypes.LightGroup_State:
					((LightGroup)baseObject).IsOn = ((int)doubleValue) == 1;
					break;
				case PropertyTypes.LightGroup_Level:
					((LightGroup)baseObject).LightLevel = (int)doubleValue;
					break;
				case PropertyTypes.Light_Level:
					((Lamp)baseObject).LightLevel = (int)doubleValue;
					break;
				case PropertyTypes.Thermometer_Level:
					((TemperatureSensor)baseObject).Temperature.Update(doubleValue);
					break;
				case PropertyTypes.Thermometer_Setpoint:
					((TemperatureSensor)baseObject).TemperatureSetpoint.Update(doubleValue);
					break;
				case PropertyTypes.Thermometer_AllowManual:
					((TemperatureSensor)baseObject).AllowManual.Update(doubleValue);
					break;
				case PropertyTypes.AC_FanLevel:
					((Conditioner)baseObject).FanLevel = (int)doubleValue;
					break;
				case PropertyTypes.Heater_Mode:
					((Heater)baseObject).IsBacstatAllowed = ((int)doubleValue) == 1;
					break;
				case PropertyTypes.Heater_Manual:
					((Heater)baseObject).ManualLevel = doubleValue;
					break;
				case PropertyTypes.LightSensor_Level:
					((LightSensor)baseObject).LightLevel.Update(doubleValue);
					break;
				case PropertyTypes.WdSensor_Alarm:
					((WdSensor)baseObject).IsLeaked.Update(doubleValue);
					break;
				default:
					throw new ArgumentOutOfRangeException("addressType");
			}
		}

	    public static BaseObject FromDbObject(PlanObject plan)
	    {
	        BaseObject result = GetBaseObjectByTypeId(plan.TypeId);
	        if (result == null) return null;

	        result.Id = plan.Id;
	        result.ParentId = plan.ParentId;
	        result.Type = plan.PlanObjectType.Name;
	        result.TypeId = plan.TypeId;
	        result.Name = plan.Name;
	        result.Description = plan.Description;
	        result.Top = plan.Top;
	        result.Left = plan.Left;
	        result.Width = plan.Width ?? 700;
	        result.Height = plan.Height ?? 700;
	        result.IsContainer = plan.PlanObjectType.IsContainer;

	        if (plan.Geometry != null)
	            result.Path = GeometryData.Parse(plan.Geometry);

	        if (!plan.PlanObjectType.IsContainer || !(result is IContainerObject)) return result;

	        var containerObject = result as IContainerObject;
	        foreach (var planObject in plan.PlanObjects.OrderBy(p => p.Order))
	            containerObject.Children.Add(FromDbObject(planObject));
	        return result;
	    }

	    public static void ExportToDbObject(this BaseObject baseObject, PlanObject dbObject, PlansDc context, int order = 0)
	    {
	        dbObject.Id = baseObject.Id >= 0 ? baseObject.Id : GetNewPlanObjectId(context);
            
            if (baseObject.ParentId != null && baseObject.ParentId >= 0)
				dbObject.ParentId = baseObject.ParentId;

            dbObject.Order = order;
			dbObject.Name = baseObject.Name;
			dbObject.Description = baseObject.Description;
			dbObject.Left = baseObject.Left;
			dbObject.Top = baseObject.Top;
			dbObject.Width = baseObject.Width;
			dbObject.Height = baseObject.Height;
			dbObject.TypeId = baseObject.TypeId;
			if (baseObject.Path != null)
				dbObject.Geometry = baseObject.Path.ToSvg();

			if (baseObject.Properties != null)
			{
				dbObject.Properties.Clear();
                foreach (var baseObjectProperty in baseObject.Properties.Where(p => !string.IsNullOrWhiteSpace(p.Path)))
				{
						var newProperty = new Property
						{
							AddressTypeId = baseObjectProperty.TypeId,
							Path = baseObjectProperty.Path
						};
						dbObject.Properties.Add(newProperty);
				}
			}

			if (!baseObject.IsContainer) return;

			var baseContainer = baseObject as IContainerObject;
			if (baseContainer == null) throw new BadRequestException("Children cannot be null in ContainerObject");

            for (var i = dbObject.PlanObjects.Count - 1; i >= 0; i--)
            {
                if (baseContainer.Children.All(s => s.Id != dbObject.PlanObjects[i].Id))
                    dbObject.PlanObjects.RemoveAt(i);
            }
	        var childOrder = 0;
			foreach (var childObject in baseContainer.Children)
			{
			    var dbChildObject = dbObject.PlanObjects.FirstOrDefault(p => p.Id == childObject.Id);
			    if (dbChildObject == null)
			    {
			        dbChildObject = new PlanObject();
			        dbObject.PlanObjects.Add(dbChildObject);
			    }
			    childObject.ExportToDbObject(dbChildObject, context, childOrder++);
			}
		}

	    private static int GetNewPlanObjectId(PlansDc context)
	    {
	        var lastPlanObject = context.PlanObjects.OrderByDescending(p => p.Id).FirstOrDefault();
	        return lastPlanObject == null ? 0 : lastPlanObject.Id + 1;
	    }

	    private static BaseObject GetBaseObjectByTypeId(int typeId)
        {
            BaseObject result = null;
            var planObjectTypes = PlansDictionaries.PlanObjectTypes;
            if (planObjectTypes.ContainsKey(typeId))
            {
                var type = Type.GetType("EsterCommon.PlanObjectTypes." + planObjectTypes[typeId] + ", EsterCommon");
                if (type != null)
                    result = (BaseObject) Activator.CreateInstance(type);
            }
	        return result;
        }
	}
}
