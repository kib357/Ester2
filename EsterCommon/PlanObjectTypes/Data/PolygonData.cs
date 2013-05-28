using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EsterCommon.PlanObjectTypes.Data
{
    public class PolygonData : GeometryData
    {
        public PolygonData()
        {
            GeometryType = "polygon";
        }

        public PolygonData(XElement xElement) : this()
        {
            FillProperties(xElement);
            if (xElement.Attribute("points") != null)
                Points = xElement.Attribute("points").Value;
        }

        public string Points { get; set; }

        public override XElement ToSvg()
        {
            var element = base.ToSvg();
            if (Points != null)
                element.Add(new XAttribute("points", Points));
            return element;
        }
    }
}
