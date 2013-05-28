using System;
using EsterCommon.PlanObjectTypes.Data;
using Newtonsoft.Json.Linq;

namespace EsterCommon.Services
{
    public class GeometryDataConverter : JsonCreationConverter<GeometryData>
    {
        protected override Type GetType(Type objectType, JObject jObject)
        {
            var type = jObject.Property("GeometryType").Value.ToString();
            switch (type)
            {
                case "circle":
                    return typeof (CircleData);
                case "ellipse":
                    return typeof(EllipseData);
                case "line":
                    return typeof(LineData);
                case "path":
                    return typeof(PathData);
                case "polygon":
                    return typeof(PolygonData);
                case "rect":
                    return typeof(RectangleData);
            }
            throw new ApplicationException(String.Format(
                "The given geometry type {0} is not supported!", type));
        }
    }
}
