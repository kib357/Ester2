using System;
using EsterCommon.Enums;
using EsterCommon.PlanObjectTypes;
using EsterCommon.PlanObjectTypes.Abstract;
using Newtonsoft.Json.Linq;

namespace EsterCommon.Services
{
    public class PlanObjectConverter : JsonCreationConverter<BaseObject>
    {
        protected override Type GetType(Type objectType, JObject jObject)
        {
            var planObjectTypes = PlansDictionaries.PlanObjectTypes;
            var typeId = (int) jObject.Property("TypeId");
            if (planObjectTypes.ContainsKey(typeId))
                return Type.GetType("EsterCommon.PlanObjectTypes." + planObjectTypes[typeId]);
            throw new ApplicationException(String.Format(
                "The given planobject type {0} is not supported!", typeId));
        }
    }
}