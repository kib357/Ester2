using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EsterCommon.PlanObjectTypes.Data
{
    public class PathData : GeometryData
    {
        public PathData()
        {
            GeometryType = "path";
        }

        public PathData(XElement xElement) : this()
        {
            FillProperties(xElement);
            if (xElement.Attribute("d") != null)
                Data = xElement.Attribute("d").Value;  
        }

        private string _data;
        public string Data
        {
            get { return _data; }
            set { _data = value; RaisePropertyChanged("Data"); }
        }

        public override XElement ToSvg()
        {
            var element = base.ToSvg();            
            if (Data != null)
                element.Add(new XAttribute("d", Data));
            return element;
        }
    }
}
