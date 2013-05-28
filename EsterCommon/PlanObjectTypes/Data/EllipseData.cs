using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EsterCommon.PlanObjectTypes.Data
{
    public class EllipseData : GeometryData
    {
        public EllipseData()
        {
            GeometryType = "ellipse";
        }

        public EllipseData(XElement xElement) : this()
        {
            FillProperties(xElement);
            double cx;
            if (xElement.Attribute("cx") != null && double.TryParse(xElement.Attribute("cx").Value, out cx))
                CX = cx;
            double cy;
            if (xElement.Attribute("cy") != null && double.TryParse(xElement.Attribute("cy").Value, out cy))
                CY = cy;
            double rx;
            if (xElement.Attribute("rx") != null && double.TryParse(xElement.Attribute("rx").Value, out rx))
                RX = rx;
            double ry;
            if (xElement.Attribute("ry") != null && double.TryParse(xElement.Attribute("ry").Value, out ry))
                RY = ry;
        }

        public double CX { get; set; }
        public double CY { get; set; }
        public double RX { get; set; }
        public double RY { get; set; }

        public override XElement ToSvg()
        {
            var element = base.ToSvg();
            element.Add(new XAttribute("cx", CX));
            element.Add(new XAttribute("cy", CY));
            element.Add(new XAttribute("rx", RX));
            element.Add(new XAttribute("ry", RY));
            return element;
        }
    }
}
