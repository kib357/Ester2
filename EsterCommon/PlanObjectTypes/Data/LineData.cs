using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EsterCommon.PlanObjectTypes.Data
{
    public class LineData : GeometryData
    {
        public LineData()
        {
            GeometryType = "line";
        }

        public LineData(XElement xElement) : this()
        {
            FillProperties(xElement);
            double x1;
            if (xElement.Attribute("x1") != null && double.TryParse(xElement.Attribute("x1").Value, out x1))
                X1 = x1;
            double y1;
            if (xElement.Attribute("y1") != null && double.TryParse(xElement.Attribute("y1").Value, out y1))
                Y1 = y1;
            double x2;
            if (xElement.Attribute("x2") != null && double.TryParse(xElement.Attribute("x2").Value, out x2))
                X2 = x2;
            double y2;
            if (xElement.Attribute("y2") != null && double.TryParse(xElement.Attribute("y2").Value, out y2))
                Y2 = y2;
        }

        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }

        public override XElement ToSvg()
        {
            var element = base.ToSvg();
            element.Add(new XAttribute("x1", X1));
            element.Add(new XAttribute("y1", Y1));
            element.Add(new XAttribute("x2", X2));
            element.Add(new XAttribute("y2", Y2));
            return element;
        }
    }
}
