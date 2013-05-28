using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EsterCommon.PlanObjectTypes.Data
{
    public class RectangleData : GeometryData
    {
        public RectangleData()
        {
            GeometryType = "rect";
        }

        public RectangleData(XElement xElement) : this()
        {
            FillProperties(xElement);
            double x;
            if (xElement.Attribute("x") != null && double.TryParse(xElement.Attribute("x").Value, out x))
                X = x;
            double y;
            if (xElement.Attribute("y") != null && double.TryParse(xElement.Attribute("y").Value, out y))
                Y = y;
            double width;
            if (xElement.Attribute("width") != null && double.TryParse(xElement.Attribute("width").Value, out width))
                Width = width;
            double height;
            if (xElement.Attribute("height") != null && double.TryParse(xElement.Attribute("height").Value, out height))
                Height = height;            
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public override XElement ToSvg()
        {
            var element = base.ToSvg();
            element.Add(new XAttribute("x", X));
            element.Add(new XAttribute("y", Y));
            element.Add(new XAttribute("width", Width));
            element.Add(new XAttribute("height", Height));
            return element;
        }
    }
}
