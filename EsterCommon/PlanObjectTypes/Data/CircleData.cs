using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EsterCommon.PlanObjectTypes.Data
{
    public class CircleData : GeometryData
    {
        public CircleData()
        {
            GeometryType = "circle";
        }

        public CircleData(XElement xElement) : this()
        {
            FillProperties(xElement);
            double ccx;
            if (xElement.Attribute("ccx") != null && double.TryParse(xElement.Attribute("ccx").Value, out ccx))
                CX = ccx;
            double ccy;
            if (xElement.Attribute("ccy") != null && double.TryParse(xElement.Attribute("ccy").Value, out ccy))
                CY = ccy;
            double r;
            if (xElement.Attribute("r") != null && double.TryParse(xElement.Attribute("r").Value, out r))
                R = r;
        }

        public double CX { get; set; }
        public double CY { get; set; }
        public double R { get; set; }

        public override XElement ToSvg()
        {
            var element = base.ToSvg();
            element.Add(new XAttribute("cx", CX));
            element.Add(new XAttribute("cy", CY));
            element.Add(new XAttribute("r", R));
            return element;
        }
    }
}
