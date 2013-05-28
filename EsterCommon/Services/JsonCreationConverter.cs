using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EsterCommon.Services
{
	public abstract class JsonCreationConverter<T> : JsonConverter
	{
		/// <summary>
		/// Create an instance of objectType, based properties in the JSON object
		/// </summary>
		protected abstract Type GetType(Type objectType, JObject jObject);

		public override bool CanConvert(Type objectType)
		{
			return typeof(T).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType,
			object existingValue, JsonSerializer serializer)
		{
		    if (reader.TokenType == JsonToken.Null) return null;
			JObject jObject = JObject.Load(reader);

			Type targetType = GetType(objectType, jObject);

			var target = Activator.CreateInstance(targetType);
			serializer.Populate(jObject.CreateReader(), target);

			return target;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			serializer.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
			serializer.Serialize(writer, value);
		}
	}
}
