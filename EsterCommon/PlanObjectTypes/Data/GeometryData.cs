using System.Windows;
using System.Xml.Linq;
using Microsoft.Practices.Prism.ViewModel;

namespace EsterCommon.PlanObjectTypes.Data
{
    public abstract class GeometryData : NotificationObject
    {
        public string GeometryType { get; set; }

        public string Fill { get; set; }
        public string FillRule { get; set; }
        public string FillOpacity { get; set; }

        public string Stroke { get; set; }
        public string StrokeWidth { get; set; }
        public string StrokeLineCap { get; set; }
        public string StrokeLineJoin { get; set; }
        public string StrokeMiterLimit { get; set; }
        public string StrokeDashArray { get; set; }
        public string StrokeDashOffset { get; set; }
        public string StrokeOpacity { get; set; }

        public virtual XElement ToSvg()
        {
            if (string.IsNullOrEmpty(GeometryType))
                return null;
            var res = new XElement(GeometryType);

            if (Fill != null)
                res.Add(new XAttribute("fill", Fill));
            if (FillRule != null)
                res.Add(new XAttribute("fill-rule", FillRule));
            if (FillOpacity != null)
                res.Add(new XAttribute("fill-opacity", FillOpacity));

            if (Stroke != null)
                res.Add(new XAttribute("stroke", Stroke));
            if (StrokeWidth != null)
                res.Add(new XAttribute("stroke", StrokeWidth));
            if (StrokeLineCap != null)
                res.Add(new XAttribute("stroke", StrokeLineCap));
            if (StrokeLineJoin != null)
                res.Add(new XAttribute("stroke", StrokeLineJoin));
            if (StrokeMiterLimit != null)
                res.Add(new XAttribute("stroke", StrokeMiterLimit));
            if (StrokeDashArray != null)
                res.Add(new XAttribute("stroke", StrokeDashArray));
            if (StrokeDashOffset != null)
                res.Add(new XAttribute("stroke", StrokeDashOffset));
            if (StrokeOpacity != null)
                res.Add(new XAttribute("stroke", StrokeOpacity));
            return res;
        }

        protected void FillProperties(XElement xElement)
        {
            if (xElement.Attribute("fill") != null)
                Fill = xElement.Attribute("fill").Value;
            if (xElement.Attribute("fill-rule") != null)
                FillRule = xElement.Attribute("fill-rule").Value;
            if (xElement.Attribute("fill-opacity") != null)
                FillOpacity = xElement.Attribute("fill-opacity").Value;

            Stroke = xElement.Attribute("stroke") != null ? xElement.Attribute("stroke").Value : "#0000";
            //if (xElement.Attribute("stroke") != null)
            //    Stroke = xElement.Attribute("stroke").Value;
            if (xElement.Attribute("stroke-width") != null)
                StrokeWidth = xElement.Attribute("stroke-width").Value;
            if (xElement.Attribute("stroke-linecap") != null)
                StrokeLineCap = xElement.Attribute("stroke-linecap").Value;
            if (xElement.Attribute("stroke-linejoin") != null)
                StrokeLineJoin = xElement.Attribute("stroke-linejoin").Value;
            if (xElement.Attribute("stroke-miterlimit") != null)
                StrokeMiterLimit = xElement.Attribute("stroke-miterlimit").Value;
            if (xElement.Attribute("stroke-dasharray") != null)
                StrokeDashArray = xElement.Attribute("stroke-dasharray").Value;
            if (xElement.Attribute("stroke-dashoffset") != null)
                StrokeDashOffset = xElement.Attribute("stroke-dashoffset").Value;
            if (xElement.Attribute("stroke-opacity") != null)
                StrokeOpacity = xElement.Attribute("stroke-opacity").Value;
        }

        public static GeometryData Parse(XElement xElement)
        {
            GeometryData res = null;
            switch (xElement.Name.LocalName)
            {
                case "path":
                    res = new PathData(xElement);
                    break;
                case "rect":
                    res = new RectangleData(xElement);
                    break;
                case "circle":
                    res = new CircleData(xElement);
                    break;
                case "ellipse":
                    res = new EllipseData(xElement);
                    break;
                case "line":
                    res = new LineData(xElement);
                    break;
                case "polyline":
                    break;
                case "polygon":
                    res = new PolygonData(xElement);
                    break;
            }
            return res;
        }
    }
}
